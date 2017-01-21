using UnityEngine;
using System.Collections;

public class SteeringAlign : SteeringAbstract {

	public float min_angle = 0.01f;
	public float slow_angle = 15.0f;
	public float time_to_target = 0.1f;

    Set_Move move;

    // --------------------------------------------
    void Start ()
    {
        move = GetComponent<Set_Move>();
    }

    // --------------------------------------------
    void Update () 
	{
        // Orientation we are trying to match
        float my_orientation = Mathf.Rad2Deg * Mathf.Atan2(transform.forward.x, transform.forward.z);
        float target_orientation = Mathf.Rad2Deg * Mathf.Atan2(move.movement.x, move.movement.z);
        float angle = Mathf.DeltaAngle(my_orientation, target_orientation);

        float angle_absolute = Mathf.Abs(angle);

        if (angle_absolute < min_angle)
        {
            move.SetRotationVelocity(0.0f, priority);
            return;
        }

        // Decide wich would be our ideal rotation velocity
        float ideal_rot_vel = 0.0f;

        if (angle_absolute > slow_angle)
            ideal_rot_vel = move.max_rot_velocity;
        else
            ideal_rot_vel = move.max_rot_velocity * angle_absolute / slow_angle;

        // Calculate acceleration needed to match that angle
        float angular_acceleration = ideal_rot_vel / time_to_target;

        if (angle < 0)
            angular_acceleration = -angular_acceleration;

        move.AccelerateRotation(Mathf.Clamp(angular_acceleration, -move.max_rot_acceleration, move.max_rot_acceleration), priority);
    }
}
