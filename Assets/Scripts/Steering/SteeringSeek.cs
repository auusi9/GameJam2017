using UnityEngine;
using System.Collections;

public class SteeringSeek : SteeringAbstract {

    Set_Move move;

    // --------------------------------------------
    void Start()
    {
        move = GetComponent<Set_Move>();
    }

    // --------------------------------------------
    void Update()
    {
        //Steer(target_seek, priority);
    }

    // --------------------------------------------
    public void Steer(Vector3 target, int other_priority)
    {
        Vector3 diff = target - transform.position;
        diff.Normalize();
        diff *= move.max_mov_acceleration;

        move.AccelerateMovement(diff, other_priority);
    }
}
