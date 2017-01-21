using UnityEngine;
using System.Collections;

public class SteeringPursue : SteeringAbstract {

	public float    max_prediction;

	Set_Move        move;
	SteeringArrive  arrive_sb;

    public GameObject target;

    // --------------------------------------------
    void Start ()
    {
        move = GetComponent<Set_Move>();
		arrive_sb = GetComponent<SteeringArrive>();
    }

    // --------------------------------------------
    void Update () 
	{
        if(target != null)
            Steer(target.transform.position, move.target.GetComponent<Set_Move>().movement);
	}

    // --------------------------------------------
    public void Steer(Vector3 target_pos, Vector3 target_v)
	{
		Vector3 diff = target_pos - transform.position;
		float distance = diff.magnitude;
		float my_speed = move.movement.magnitude;
		float prediction;

		// is the speed too small ?
		if(my_speed < distance / max_prediction)
			prediction = max_prediction;
		else
			prediction = distance / my_speed;

		arrive_sb.Steer(target_pos + (target_v * prediction), priority);
	}
}
