using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IA_Animation : MonoBehaviour
{
    Animator anim;
    NavMeshAgent agent;
    Vector2 smoothDeltaPosition = Vector2.zero;
    Vector2 velocity = Vector2.zero;

    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        // Don’t update position automatically
        agent.updatePosition = true;
    }

    void Update()
    {
        Vector3 worldDeltaPosition = agent.destination - transform.position;

        // Map 'worldDeltaPosition' to local space
        float dx = Vector3.Dot(transform.right, worldDeltaPosition);
        float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2(dx, dy);

        // Low-pass filter the deltaMove
        float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
        smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

        float m_TurnAmount = Mathf.Atan2(smoothDeltaPosition.x, smoothDeltaPosition.y);
        float m_ForwardAmount = smoothDeltaPosition.y;
        // Update animation parameters
        
        anim.SetFloat("Forward", m_ForwardAmount);
        anim.SetFloat("Turn", m_TurnAmount);
    }
}
