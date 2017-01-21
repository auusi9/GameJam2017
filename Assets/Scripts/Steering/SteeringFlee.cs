using UnityEngine;
using System.Collections;

public class SteeringFlee : SteeringAbstract {

    public float search_radius = .0f;
    public LayerMask mask;
    Set_Move move;

    // --------------------------------------------
    void Start ()
    {
		move = GetComponent<Set_Move>();
	}

    // --------------------------------------------
    void Update () 
	{
        Collider[] colliders = Physics.OverlapSphere(transform.position, search_radius, mask);
        Vector3 final = Vector3.zero;

        foreach (Collider col in colliders)
        {
            GameObject go = col.gameObject;

            if (go == gameObject)
                continue;

            Vector3 diff = transform.position - go.transform.position;
            float distance = diff.magnitude;

            final += diff.normalized * move.max_mov_acceleration;
        }

        float final_strength = final.magnitude;

        if (final_strength > 0.0f)
        {
            if (final_strength > move.max_mov_acceleration)
                final = final.normalized * move.max_mov_acceleration;

            move.AccelerateMovement(final, priority);
        }
    }
}
