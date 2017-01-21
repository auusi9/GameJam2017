using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {

    public GameObject player;
    float last_angle = 0.0f;
    public int x = 10;
    public int y = 10;
    public int z = 10;
    Vector3 offset;

	// Use this for initialization
	void Start () {
        transform.position = new Vector3(player.transform.position.x + x, player.transform.position.y + y, player.transform.position.z + z);
        transform.LookAt(player.transform);
        offset = player.transform.position - transform.position;
	}
	
	// Update is called once per frame
	void Update() {

        transform.position = player.transform.position - offset;

        if(Input.GetButtonDown("Camera Left"))
        {
            last_angle += 45.0f;
            transform.RotateAround(player.transform.position, Vector3.up, 45.0f);
            transform.LookAt(player.transform);
            offset = player.transform.position - transform.position;
        }
        else if(Input.GetButtonDown("Camera Right"))
        {
            last_angle -= 45.0f;
            transform.RotateAround(player.transform.position, Vector3.up, -45.0f);
            transform.LookAt(player.transform);
            offset = player.transform.position - transform.position;
        }
	}
}
