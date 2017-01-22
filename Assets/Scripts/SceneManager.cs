using UnityEngine;
using UnityEngine.Audio;
using System;

public class SceneManager : MonoBehaviour
{
    public static SceneManager current;
    
    /// <summary>
    /// Mida del Array que passem al shader (ha de ser el mateix que al shader)
    /// </summary>
    public const int ArraySize = 32;

    public const int NumWaves = 64;

    public const float TimeBetweenAudioWaves = 1f;

    public const float AudioValueThreshold = 0.5f;

    /// <summary>
    /// Distancia minima a la que sempre produim soroll
    /// </summary>
    public float minDistance = 1;

    /// <summary>
    /// Distancia maxima a la que podem produir soroll
    /// </summary>
    public float maxDistance = 10;

    /// <summary>
    /// Objecte MicrophoneListener que llegeix el valor del micro
    /// </summary>
    public MicrophoneListener microphoneListener;

    /// <summary>
    /// Distancia actual a la que "pintem"
    /// </summary>
    public float distance;

    /// <summary>
    /// Transform del player
    /// </summary>
    public Transform player;

    /// <summary>
    /// Distancia del peu al centre del player
    /// </summary>
    public float footDistance = 0.25f;

    public AudioMixer masterMixer;

    /// <summary>
    /// Si es el primer pas o el segon que anem fent
    /// </summary>
    public bool firstStep = true;

    /// <summary>
    /// Nom del vector del centre de la posicio al shader
    /// </summary>
    private int waveVectorID;

    /// <summary>
    /// Nom del vector del centre de la posicio anterior al shader
    /// </summary>
    private int oldWaveVectorID;

    /// <summary>
    /// Nom del array de linies al shader
    /// </summary>
    private int linesArrayID;

    /// <summary>
    /// Llargada del vector de linies al shader
    /// </summary>
    private int lenghtID;

    /// <summary>
    /// Distancia a la que pintem linies al shader
    /// </summary>
    private int distanceID;

    /// <summary>
    /// Distancia a la que pintem les linies anterior al shader
    /// </summary>
    private int oldDistanceID;

    private Vector4[] positions;

    private float[] distances;

    private WaveInfo[] wavesInfo;

    private int currentWave = 0;

    private float lastWaveTime = 0;

    private void Awake()
    {
        if(current != null)
        {
            Debug.LogWarning("Trying to add more than one SceneManager");
            Destroy(this);
        }
        else
        {
            current = this;
        }

        wavesInfo = new WaveInfo[NumWaves];
        distances = new float[NumWaves];
        positions = new Vector4[NumWaves];

        waveVectorID = Shader.PropertyToID("_SonarWaveVector");
        oldWaveVectorID = Shader.PropertyToID("_OldSonarWaveVector");
        linesArrayID = Shader.PropertyToID("_LinesArray");
        lenghtID = Shader.PropertyToID("_Length");
        distanceID = Shader.PropertyToID("_Distance");
        oldDistanceID = Shader.PropertyToID("_oldDistance");

        CalculatePoints();
    }

    private void Update()
    {
        if (microphoneListener._AmplitudeBuffer > AudioValueThreshold && lastWaveTime + TimeBetweenAudioWaves < Time.time)
        {
            lastWaveTime = Time.time;
            AddWave(player.position);
        }

        float desiredDistance = microphoneListener._AmplitudeBuffer;
        desiredDistance = Mathf.Max(desiredDistance, minDistance);
        desiredDistance = Mathf.Min(desiredDistance, maxDistance);

        for(int i = 0; i < wavesInfo.Length; i++)
        {
            // Mirem si la final distance de la wave actual es menor que la desiredDistance i si ho es la incrementem
            if (i == currentWave)
            {
                if(!wavesInfo[i].finalDistanceFixed && wavesInfo[i].finalDistance < desiredDistance)
                {
                    wavesInfo[i].finalDistance = desiredDistance;
                }
                else
                {
                    wavesInfo[i].finalDistanceFixed = true;
                }
            }

            if (wavesInfo[i].distance < wavesInfo[i].finalDistance && !Mathf.Approximately(wavesInfo[i].distance, wavesInfo[i].finalDistance))
            {
                wavesInfo[i].distance = Mathf.SmoothStep(wavesInfo[i].distance, wavesInfo[i].finalDistance, Time.deltaTime * 10);
            }
            else if(wavesInfo[i].timeInDistance < .1f)
            {
                wavesInfo[i].timeInDistance += Time.deltaTime;
            }
            else
            {
                wavesInfo[i].finalDistance = 0; // Fem reset per a que no entri al primer if
                wavesInfo[i].distance = Mathf.SmoothStep(wavesInfo[i].distance, 0, Time.deltaTime * 10);
            }

            distances[i] = wavesInfo[i].distance;
            positions[i] = wavesInfo[i].position;
        }

        MoveWithinArray(distances, currentWave, 0);
        MoveWithinArray(positions, currentWave, 0);

        Shader.SetGlobalInt("_CurrentWave", currentWave);
        Shader.SetGlobalFloatArray("_Distances", distances);
        Shader.SetGlobalVectorArray("_StartPositions", positions);
    }

    /// <summary>
    /// Funcio treta de stackoverflow per a canviar de posicio elements de l'array
    /// </summary>
    /// <param name="array">L'array al que li volem canviar els valors</param>
    /// <param name="source">Posicio a la que esta ara</param>
    /// <param name="dest">Posicio a la que ho volem moure</param>
    private void MoveWithinArray(Array array, int source, int dest)
    {
        System.Object temp = array.GetValue(source);
        Array.Copy(array, dest, array, dest + 1, source - dest);
        array.SetValue(temp, dest);
    }

    /// <summary>
    /// Afegim una nova wave. Descartem la mes vella i actualitzem el punter a l'actual
    /// </summary>
    public void AddWave(Vector3 position, float finalDistance = 0)
    {
        if(Vector3.Distance(position, player.position) < 15)
        {
            currentWave = (++currentWave) % wavesInfo.Length;
            wavesInfo[currentWave].Reset(position, finalDistance);
        }
    }

    private void CalculatePoints()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(1f);
        float[] array = new float[ArraySize];
        Vector3 lastPlayerPosition = player.position;

        int i;
        for (i = 0; i < ArraySize; i++)
        {
            array[i] = Mathf.Pow((i + 8) / 16f, 2) + Mathf.Pow((i + 8) / 16f, 4);

            if(array[i] > maxDistance)
            {
                break;
            }
        }

        for(int j = 0; j < positions.Length; j++)
        {
            positions[j] = player.position;
        }

        Shader.SetGlobalFloatArray(linesArrayID, array);
        Shader.SetGlobalInt(lenghtID, i);
    }

    //void OnDrawGizmos()
    //{
    //    Debug.DrawRay(player.position, Vector3.right * distance, Color.blue);
    //}

    /// <summary>
    /// Struct per a guardar info de les waves que hem de dibuixar al shader
    /// </summary>
    public struct WaveInfo
    {
        /// <summary>
        /// S'hi s'ha fixat ja la distancia final
        /// </summary>
        public bool finalDistanceFixed;

        /// <summary>
        /// Posicio on s'ha de crear la wae
        /// </summary>
        public Vector4 position;

        /// <summary>
        /// Distancia actual de la wave
        /// </summary>
        public float distance;

        /// <summary>
        /// Distancia final / maxima que tindra la wave
        /// </summary>
        public float finalDistance;

        /// <summary>
        /// Temps que passa a la distancia final abans de decreixer
        /// </summary>
        public float timeInDistance;

        public void Reset(Vector4 pos, float fDistance = 0f)
        {
            position = pos;

            finalDistanceFixed = !Mathf.Approximately(0, fDistance);

            distance = 0;

            finalDistance = fDistance;

            timeInDistance = 0;
        }
    }
}
