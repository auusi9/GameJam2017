using UnityEngine;
using System.Collections;

[System.Serializable]
public class my_ray
{
    public float length = 2.0f;
    public Vector3 direction = Vector3.forward;
}

public class SteeringObstacleAvoidance : SteeringAbstract {

    public LayerMask        mask;
    public float            avoid_distance = 5.0f;
    public my_ray[]         rays;
    public AnimationCurve   falloff;

    Set_Move move;

    // --------------------------------------------
    void Start ()
    {
        move = GetComponent<Set_Move>(); 
    }

    // --------------------------------------------
    void Update () 
    {
        float angle = Mathf.Atan2(move.movement.x, move.movement.z);

        foreach(my_ray ray in rays)
        {
            RaycastHit hit;

            if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z), transform.forward, out hit, ray.length, mask) == true)
            {
                //Debug.Log("<color=blue>" + transform.name + "</color> try to make a collision");
                Vector3 hit_point = new Vector3(hit.point.x, this.transform.position.y, hit.point.z) + hit.normal * avoid_distance;

                Vector3 diff = hit_point - transform.position;
                float acceleration = (1.0f - falloff.Evaluate(diff.magnitude / avoid_distance)) * move.max_mov_acceleration;

                move.AccelerateMovement(diff.normalized * acceleration, priority);
            }
        }
    }

    // --------------------------------------------
    void OnDrawGizmosSelected() 
    {
        if(move && this.isActiveAndEnabled)
        {
            // Display the explosion radius when selected
            Gizmos.color = Color.red;
            float angle = Mathf.Atan2(move.movement.x, move.movement.z);
            Quaternion quat = Quaternion.AngleAxis(Mathf.Rad2Deg * angle, Vector3.up);

            foreach(my_ray ray in rays)
                Gizmos.DrawLine(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), (new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z)) + (quat * ray.direction.normalized) * ray.length);
        }
    }
}

