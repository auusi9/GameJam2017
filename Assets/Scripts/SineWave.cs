using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineWave : MonoBehaviour {

    public float amplitudeX = 10.0f;
    public float amplitudeY = 5.0f;
    public float omegaX = 1.0f;
    public float omegaY = 5.0f;
    float new_omegaY = 0.0f;
    public float index;
    public ParticleSystem particles;
    ParticleSystem.EmissionModule em;
    int enemies = 0;
	// Use this for initialization
	void Start () {
        em = particles.emission;
        new_omegaY = omegaY;
        enemies = SurvivorsManager.singleton.transform.childCount;

    }
	
	// Update is called once per frame
	void Update () {
        index += Time.deltaTime;

        int i = enemies - SurvivorsManager.singleton.transform.childCount;
    
        new_omegaY = omegaY + 2*i;
       

        if(index >= Mathf.PI)
        {
            index = 0;
            em.enabled = false;
            omegaY = new_omegaY;
        }
        else
        {
            if(index <= 0.02)
            {
                em.enabled = false;
            }
            else
            {
                em.enabled = true;
            }

            float x = amplitudeX * Mathf.Cos(omegaX * index);
            float y = (amplitudeY * Mathf.Sin(omegaY * index));
            transform.localPosition = new Vector3(-x, y, 0);
        }
	}
}
