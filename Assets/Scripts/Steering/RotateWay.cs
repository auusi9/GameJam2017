using UnityEngine;
using System.Collections;

public class RotateWay : MonoBehaviour {
    
    public float dps = 1.5f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        transform.RotateAround(Vector3.zero, Vector3.up, dps * Time.deltaTime);
	}
}
