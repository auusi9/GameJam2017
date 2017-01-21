using UnityEngine;
using System.Collections;

public class SteeringWander : SteeringAbstract {

    public Vector3  offset_wander = Vector3.zero;
	public float    radius = 1.0f;
	public float    min_update = 1.0f;
	public float    max_update = 3.0f;

    Vector3         point_wander = Vector3.zero;
    public GameObject target;

    // --------------------------------------------
    void Start()
    {
	    WanderTarget();
	}

    // --------------------------------------------
    void Update()
    {}

    // --------------------------------------------
    void WanderTarget() 
	{
        bool ret = false;

        while (ret != true)
        {
            point_wander = Random.insideUnitSphere;
            point_wander *= radius;
            point_wander += target.transform.position + offset_wander;

            // Cast rays to see if it's a valid point.
            RaycastHit[] wander_hit;

            wander_hit = Physics.RaycastAll(new Vector3(point_wander.x, 200.0f, point_wander.z), new Vector3(.0f, -1.0f, .0f), 200.0f);

            if (wander_hit.Length == 1)
            {
                if(wander_hit[0].transform.gameObject.layer == 13)
                {
                    point_wander.y = wander_hit[0].point.y;
                    ret = true;
                }
                else
                {
                    Debug.Log("Not terrain");
                }
            }

            else
            {
                Debug.Log("Not walkable");
            }
        }

        target.transform.position = point_wander;

        if(this.enabled)
            Invoke("WanderTarget", Random.Range(min_update, max_update));
	}

    // --------------------------------------------
    void OnDrawGizmosSelected() 
	{
		if(this.isActiveAndEnabled)
		{
			// Display the explosion radius when selected
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(target.transform.position, radius);
		
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(point_wander, 0.2f);
		}
	}
}
