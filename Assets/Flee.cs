using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Flee : MonoBehaviour
{
    Vector3 point_wander;
    float angle = 0.0f;
    Vector3 target = Vector3.zero;
    NavMeshAgent agent;
    public GameObject player;

    // Use this for initialization
    void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed += 20;
        Fleee();
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(agent.destination, transform.position) <= 1)
        {
            Fleee();
        }
    }

    void Fleee()
    {
        point_wander = transform.position - player.transform.position;
        point_wander.Normalize();

        angle = Random.Range(-45.0f, 45.0f);

        point_wander *= 4;
        Quaternion q = Quaternion.AngleAxis(angle, new Vector3(0,1,0));
        point_wander = q * point_wander;
        agent.destination = point_wander;
    }
}