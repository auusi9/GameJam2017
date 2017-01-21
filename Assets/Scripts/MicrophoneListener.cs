using UnityEngine;
using System.Collections;
using UnityEngine.Audio; // required for dealing with audiomixers

[RequireComponent(typeof(AudioSource))]
public class MicrophoneListener : MonoBehaviour
{
    //Written in part by Benjamin Outram

    //option to toggle the microphone listenter on startup or not
    public bool startMicOnStartup = true;

    //allows start and stop of listener at run time within the unity editor
    public bool stopMicrophoneListener = false;
    public bool startMicrophoneListener = false;

    private bool microphoneListenerOn = false;

    //public to allow temporary listening over the speakers if you want of the mic output
    //but internally it toggles the output sound to the speakers of the audiosource depending
    //on if the microphone listener is on or off
    public bool disableOutputSound = false;

    //an audio source also attached to the same object as this script is
    public AudioSource src;

    //make an audio mixer from the "create" menu, then drag it into the public field on this script.
    //double click the audio mixer and next to the "groups" section, click the "+" icon to add a 
    //child to the master group, rename it to "microphone".  Then in the audio source, in the "output" option, 
    //select this child of the master you have just created.
    //go back to the audiomixer inspector window, and click the "microphone" you just created, then in the 
    //inspector window, right click "Volume" and select "Expose Volume (of Microphone)" to script,
    //then back in the audiomixer window, in the corner click "Exposed Parameters", click on the "MyExposedParameter"
    //and rename it to "Volume"
    public AudioMixer masterMixer;

    public float audioProfile;
    float timeSinceRestart = 0;

    public float value;
    float[] _samples = new float[256];
    float[] _freqBand = new float[8];
    float[] _bandBuffer = new float[8];
    float[] _bufferDecrease = new float[8];
    float[] _freqBandHighest = new float[8];
    float[] _audioBand = new float[8];
    public float[] _audioBandBuffer = new float[8];

    public float _Amplitude, _AmplitudeBuffer;
    float _amplitudeHighest;

    void Start()
    {
        //start the microphone listener
        if (startMicOnStartup)
        {
            RestartMicrophoneListener();
            StartMicrophoneListener();
        }
    }

    void Update()
    {

        //can use these variables that appear in the inspector, or can call the public functions directly from other scripts
        if (stopMicrophoneListener)
        {
            StopMicrophoneListener();
        }
        if (startMicrophoneListener)
        {
            StartMicrophoneListener();
        }
        //reset paramters to false because only want to execute once
        stopMicrophoneListener = false;
        startMicrophoneListener = false;

        //must run in update otherwise it doesnt seem to work
        MicrophoneIntoAudioSource(microphoneListenerOn);

        //can choose to unmute sound from inspector if desired
        DisableSound(!disableOutputSound);

        float[] spectrum = new float[256];
        float value = 0;

        AudioListener.GetOutputData(spectrum, 0);

        for (int i = 1; i < spectrum.Length - 1; i++)
        {
            value += spectrum[i] * spectrum[i];
            //Debug.DrawLine(new Vector3(i - 1, spectrum[i] + 10, 0), new Vector3(i, spectrum[i + 1] + 10, 0), Color.red);
            //Debug.DrawLine(new Vector3(i - 1, Mathf.Log(spectrum[i - 1]) + 10, 2), new Vector3(i, Mathf.Log(spectrum[i]) + 10, 2), Color.cyan);
            //Debug.DrawLine(new Vector3(Mathf.Log(i - 1), spectrum[i - 1] - 10, 1), new Vector3(Mathf.Log(i), spectrum[i] - 10, 1), Color.green);
            //Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(spectrum[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(spectrum[i]), 3), Color.blue);
        }

        //value = .5f + Mathf.Sqrt((value) / 256) * 100;
        this.value = value;
        GetSpectrumAudioSource();
        MakeFrequencyBand();
        BandBuffer();
        CreateAudioBand();
        GetAmplitude();
        AudioProfile(audioProfile);
    }


    //stops everything and returns audioclip to null
    public void StopMicrophoneListener()
    {
        //stop the microphone listener
        microphoneListenerOn = false;
        //reenable the master sound in mixer
        disableOutputSound = false;
        //remove mic from audiosource clip
        src.Stop();
        src.clip = null;

        Microphone.End(null);
    }


    public void StartMicrophoneListener()
    {
        //start the microphone listener
        microphoneListenerOn = true;
        //disable sound output (dont want to hear mic input on the output!)
        disableOutputSound = true;
        //reset the audiosource
        RestartMicrophoneListener();
    }


    //controls whether the volume is on or off, use "off" for mic input (dont want to hear your own voice input!) 
    //and "on" for music input
    public void DisableSound(bool SoundOn)
    {

        float volume = 0;

        if (SoundOn)
        {
            volume = 0.0f;
        }
        else
        {
            volume = -80.0f;
        }

        masterMixer.SetFloat("MasterVolume", volume);
    }



    // restart microphone removes the clip from the audiosource
    public void RestartMicrophoneListener()
    {

        src = GetComponent<AudioSource>();

        //remove any soundfile in the audiosource
        src.clip = null;

        timeSinceRestart = Time.time;

    }

    //puts the mic into the audiosource
    void MicrophoneIntoAudioSource(bool MicrophoneListenerOn)
    {

        if (MicrophoneListenerOn)
        {
            //pause a little before setting clip to avoid lag and bugginess
            if (Time.time - timeSinceRestart > 0.5f && !Microphone.IsRecording(null))
            {
                src.clip = Microphone.Start(null, true, 10, 44100);
                Invoke("RestartMicrophoneListenerCustom", 10.05f);

                //wait until microphone position is found (?)
                while (!(Microphone.GetPosition(null) > 0))
                {
                }

                src.Play(); // Play the audio source
            }
        }
    }

    void RestartMicrophoneListenerCustom()
    {
        Debug.Log("M'han invocat!");
        stopMicrophoneListener = true;
        startMicrophoneListener = true;
    }

    void GetSpectrumAudioSource()
    {
        src.GetSpectrumData(_samples, 0, FFTWindow.Blackman);

    }
    //Create 8 bands of frequency in order from bass to acute
    void MakeFrequencyBand()
    {
        int count = 0;

        for (int i = 0; i < 8; i++)
        {
            int sampleCount = (int)Mathf.Pow(2, i);
            float average = 0;
            if (i == 7)
                sampleCount += 2;
            for (int j = 0; j < sampleCount; j++)
            {
                average += _samples[i] * (i + 1);
                count++;
            }
            average /= sampleCount;
            _freqBand[i] = average * 10;
        }
    }
    //Creates a buffer to smoothly decrease the frequency value
    void BandBuffer()
    {
        for (int i = 0; i < 8; ++i)
        {
            if (_freqBand[i] > _bandBuffer[i])
            {
                _bandBuffer[i] = _freqBand[i];
                _bufferDecrease[i] = 0.005f;
            }

            if (_freqBand[i] < _bandBuffer[i])
            {
                _bandBuffer[i] -= _bufferDecrease[i];
                _bufferDecrease[i] *= 1.2f;
            }
        }
    }

    //Creates a value between 0 and 1 for each band
    void CreateAudioBand()
    {
        for (int i = 0; i < 8; ++i)
        {
            if (_freqBand[i] > _freqBandHighest[i])
            {
                _freqBandHighest[i] = _freqBand[i];
            }

            _audioBand[i] = (_freqBand[i] / _freqBandHighest[i]);
            _audioBandBuffer[i] = (_bandBuffer[i] / _freqBandHighest[i]);
        }
    }
    //Gets the amplitude of the sound
    void GetAmplitude()
    {
        float _CurrentAmplitude = 0;
        float _CurrentAmplitudeBuffer = 0;
        for (int i = 0; i < 8; ++i)
        {
            _CurrentAmplitude += _audioBand[i];
            _CurrentAmplitudeBuffer += _audioBandBuffer[i];
        }
        if (_CurrentAmplitude > _amplitudeHighest)
        {
            _amplitudeHighest = _CurrentAmplitude;
        }
        _Amplitude = _CurrentAmplitude / _amplitudeHighest;
        _AmplitudeBuffer = _CurrentAmplitudeBuffer / _amplitudeHighest;
    }
    //Set frequency highest value
    void AudioProfile(float audioProfile)
    {
        for (int i = 0; i < 8; i++)
        {
            _freqBandHighest[i] = audioProfile;

        }
    }
}

/*
 
     
     44100/512 = 86.13hz per sample

     */
