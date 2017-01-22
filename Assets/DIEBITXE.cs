using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DIEBITXE : MonoBehaviour {

    float timer = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        timer += Time.deltaTime;

        if (timer > 4)
            Destroy(this.gameObject);
	}
}
