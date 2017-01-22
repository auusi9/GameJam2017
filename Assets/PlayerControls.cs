using System;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerControls : MonoBehaviour {

    private PlayerController m_Character;
    private Transform m_Cam;
    private Vector3 m_CamForward;
    private Vector3 m_Move;
    private bool m_Jump;
    public float speed = 0.0f;
    Animator animator;
    new Rigidbody rigidbody;

	// Use this for initialization
	void Start () {
        m_Cam = Camera.main.transform;
        m_Character = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        // read inputs
        float h = CrossPlatformInputManager.GetAxis("Horizontal");
        float v = CrossPlatformInputManager.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.P) || CrossPlatformInputManager.GetButtonDown("Fire1"))
        {
            animator.SetBool("Attacking", true);
            rigidbody.velocity = Vector3.zero;
            
        }
        if (animator.GetBool("Attacking") == false)
        {
            // calculate camera relative direction to move:
            m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
            m_Move = v * m_CamForward + h * m_Cam.right;
            m_Move *= speed;
            // walk speed multiplier
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetAxisRaw("Right Trigger") != 0)
                m_Move *= 4;

            rigidbody.velocity = m_Move;
        }
       else
        {
            Debug.DrawLine(transform.position, SceneManager.current.GetLastWaveDistance() * Vector3.forward, Color.yellow);
            float a = SceneManager.current.GetLastWaveDistance();
            SurvivorsManager.singleton.DetectOnAttack(a);
        }

        

        // pass all parameters to the character control script
        m_Character.Move(m_Move);
	}
}
