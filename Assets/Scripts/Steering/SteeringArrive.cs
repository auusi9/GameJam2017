using UnityEngine;
using System.Collections;

public class SteeringArrive : SteeringAbstract {

	public float min_distance = 0.1f;
	public float slow_distance = 5.0f;
	public float time_to_target = 0.1f;
    public float offset_radius = 5.0f;

    [Header("          -------- Read Only -------")]
    public Vector3 destination_offset = Vector3.zero;
    Set_Move move;
    
    Vector3 new_destination = Vector3.zero;

    // --------------------------------------------
    void Start ()
    { 
        move = GetComponent<Set_Move>();
        //DestinationOffset();
    }

    // --------------------------------------------
    void Update () 
	{
        // Check if it's a valid destination.
        //Vector3 tmp_dest = move.target.transform.position + destination_offset;
        //if (new_destination.x != tmp_dest.x && new_destination.z != tmp_dest.z)
        //    CheckDestination();

        Steer(move.target.transform.position, priority);
	}

    // --------------------------------------------
    public void Steer(Vector3 target, int other_priority)
	{
		if(!move)
			move = GetComponent<Set_Move>();

		// Velocity we are trying to match.
		float ideal_velocity = 0.0f;
		Vector3 diff = target - transform.position;

		if(diff.magnitude < min_distance)
        {
            move.SetMovementVelocity(Vector3.zero, other_priority);
            return;
        }

        // Decide wich would be our ideal velocity.
        if (diff.magnitude > slow_distance)
            ideal_velocity = move.max_mov_velocity;
        else
            ideal_velocity = move.max_mov_velocity * diff.magnitude / slow_distance;

        // Create a vector that describes the ideal velocity.
        Vector3 ideal_movement = diff.normalized * ideal_velocity;

        // Calculate acceleration needed to match that velocity.
        Vector3 acceleration = ideal_movement - move.movement;
        acceleration /= time_to_target;

        // Cap acceleration
        if (acceleration.magnitude > move.max_mov_acceleration)
        {
            acceleration.Normalize();
            acceleration *= move.max_mov_acceleration;
        }

        move.AccelerateMovement(acceleration, other_priority);             
	}

    // --------------------------------------------
    public void DestinationOffset()
    {
        // Offset position
        destination_offset = Random.insideUnitSphere;
        destination_offset *= offset_radius;

        //Invoke("DestinationOffset", 25);
    }

    // --------------------------------------------
    void CheckDestination()
    {
        bool ret = false;

        while (ret != true)
        {
            new_destination = move.target.transform.position + destination_offset;

            // Cast rays to see if it's a valid point.
            RaycastHit[] offset_hit;
            offset_hit = Physics.RaycastAll(new Vector3(new_destination.x, 200.0f, new_destination.z), new Vector3(.0f, -1.0f, .0f), 300.0f);

            if (offset_hit.Length == 1)
            {
                if (offset_hit[0].transform.gameObject.layer == 13)
                {
                    Debug.Log("<color=green>" + transform.name + "</color>: niceru position you have there!");
                    new_destination.y = offset_hit[0].point.y;
                    ret = true;
                }
                else
                {
                    Debug.Log("Not terrain");
                }
            }
            else
            {
                Debug.Log("<color=red>" + transform.name + "</color>: not walkable");
                DestinationOffset();
            }
        }
    }

    // --------------------------------------------
    void OnDrawGizmosSelected() 
	{
		// Display the explosion radius when selected
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(transform.position, min_distance);

		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(transform.position, slow_distance);
	}
}
