using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class AudioFadeOut
{

    public static IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }

}

public class FXManager : MonoBehaviour {

    private AudioSource source;
    public AudioSource chase;
    public AudioClip danger;
    public AudioClip owl1;
    public AudioClip owl2;
    public AudioClip owl3;
    public AudioClip owl4;
    AudioClip[] owl_sounds;
    int random = 0;

    public static FXManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        StopChase();
    }

	// Use this for initialization
	void Start () {
        source = GetComponent<AudioSource>();

        owl_sounds = new AudioClip[4];
        owl_sounds[0] = owl1;
        owl_sounds[1] = owl2;
        owl_sounds[2] = owl3;
        owl_sounds[3] = owl4;
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void PlayChase()
    {
        chase.Play();
    }

    public void StopChase()
    {
        chase.Stop();
    }

    public void FadeChase()
    {
        IEnumerator fadeSound1 = AudioFadeOut.FadeOut(chase, 1.0f);
        StartCoroutine(fadeSound1);
    }

    public void PlayDanger()
    {
        source.PlayOneShot(danger, source.volume);
    }

    public void PlayOwlSounds()
    {
        random = Random.Range(0, 3);

        source.PlayOneShot(owl_sounds[random], source.volume);
    }
}
