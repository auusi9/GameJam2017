using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    float m_MovingTurnSpeed = 360;
    float m_StationaryTurnSpeed = 180;
    float m_JumpPower = 12f;
    float m_GravityMultiplier = 2f;
    float m_RunCycleLegOffset = 0.2f;
    float m_MoveSpeedMultiplier = 1f;
    float m_AnimSpeedMultiplier = 1f;
    float m_GroundCheckDistance = 0.1f;
    Rigidbody m_Rigidbody;
    Animator m_Animator;
    bool m_IsGrounded;
    float m_OrigGroundCheckDistance;
    const float k_Half = 0.5f;
    float m_TurnAmount;
    float m_ForwardAmount;
    Vector3 m_GroundNormal;
    float m_CapsuleHeight;
    Vector3 m_CapsuleCenter;
    CapsuleCollider m_Capsule;

    // Use this for initialization
    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Capsule = GetComponent<CapsuleCollider>();
        m_CapsuleHeight = m_Capsule.height;
        m_CapsuleCenter = m_Capsule.center;
        m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    // --------------------------------------------------------------------------------------------------------------------------
    // --------------------------------------------------------------------------------------------------------------------------
    // --------------------------------------------------------------------------------------------------------------------------

    public void Move(Vector3 move)
    {
        if (move.magnitude > 1f) move.Normalize();
        move = transform.InverseTransformDirection(move);

        move = Vector3.ProjectOnPlane(move, m_GroundNormal);
        m_TurnAmount = Mathf.Atan2(move.x, move.z);
        m_ForwardAmount = move.z;



        ApplyExtraTurnRotation();        

        // send input and other state parameters to the animator
        UpdateAnimator(move);
    }

    // --------------------------------------------------------------------------------------------------------------------------
    // --------------------------------------------------------------------------------------------------------------------------
    // --------------------------------------------------------------------------------------------------------------------------

    void UpdateAnimator(Vector3 move)
    {
        // update the animator parameters
        m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);        

        m_Animator.speed = 1;
       
    }

    // --------------------------------------------------------------------------------------------------------------------------
    // --------------------------------------------------------------------------------------------------------------------------
    // --------------------------------------------------------------------------------------------------------------------------

    void ApplyExtraTurnRotation()
    {
        // help the character turn faster (this is in addition to root rotation in the animation)
        float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
        transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
    }

    // --------------------------------------------------------------------------------------------------------------------------
    // --------------------------------------------------------------------------------------------------------------------------
    // --------------------------------------------------------------------------------------------------------------------------

    public void OnAnimatorMove()
    {

    }

    // --------------------------------------------------------------------------------------------------------------------------
    // --------------------------------------------------------------------------------------------------------------------------
    // --------------------------------------------------------------------------------------------------------------------------

    void CheckGroundStatus()
    {

    }
}
