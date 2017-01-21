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
    float speed = 0;
    // Use this for initialization
    void OnEnable()
    {
        agent = GetComponent<NavMeshAgent>();
        speed = agent.speed;
        agent.speed = 10;
        Fleee();
    }

    // Update is called once per frame
    void Update()
    {
        point_wander = transform.position - player.transform.position;
        point_wander.Normalize();

        angle = Random.Range(-45.0f, 45.0f);


        Quaternion q = Quaternion.AngleAxis(angle, new Vector3(0, 1, 0));
        point_wander = q * point_wander;
        point_wander *= 4;
        agent.destination = point_wander + transform.position;

    }

    void Fleee()
    {
        
    }

    void OnDisable()
    {
        agent.speed = speed;
    }
}