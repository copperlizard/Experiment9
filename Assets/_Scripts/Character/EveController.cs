using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class EveController : MonoBehaviour
{
    [SerializeField]
    private float m_groundCheckDist = 0.01f, m_lerpToGround = 0.1f, m_StationaryTurnSpeed = 180.0f, m_MovingTurnSpeed = 360.0f, m_RunCycleLegOffset = 0.0f;

    private Animator m_animator;

    private Camera m_camera;

    private CapsuleCollider m_capCollider;

    private Rigidbody m_rigidBody;

    private RaycastHit m_groundAt;

    private Vector3 m_move = Vector3.zero;

    private float m_forward = 0.0f, m_turn = 0.0f, m_jumpLeg = -1.0f, m_dance = 0.0f;

    private bool m_grounded = true, m_jumping = false, m_crouching = false, m_aboutFace = false, m_dancing = false;

    // Use this for initialization
    void Start()
    {   
        m_animator = GetComponent<Animator>();
        if (m_animator == null)
        {
            Debug.Log("m_animator not found!");
        }

        m_rigidBody = GetComponent<Rigidbody>();
        if (m_rigidBody == null)
        {
            Debug.Log("m_rigidBody not found!");
        }
        else
        {
            m_rigidBody.freezeRotation = true;
        }

        //m_camera = GetComponent<Camera>();
        m_camera = Camera.main;
        if (m_camera == null)
        {
            Debug.Log("m_camera not found!");
        }

        m_capCollider = GetComponent<CapsuleCollider>();
        if (m_capCollider == null)
        {
            Debug.Log("m_capCollider not found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAnimator();

        Debug.DrawLine(transform.position, transform.position + transform.forward);
    }

    private void FixedUpdate()
    {
        checkGround();
    }

    private void OnAnimatorMove()
    {
        if (Time.timeScale <= 0.0f)
        {
            return;
        }
        
        Vector3 v = m_animator.deltaPosition / Time.deltaTime;

        if (m_rigidBody.useGravity)
        {
            // preserve velocity
            v = m_rigidBody.velocity;
        }

        transform.rotation *= m_animator.deltaRotation;
        

        const float _velocityLerpRate = 20.0f;
        m_rigidBody.velocity = Vector3.Lerp(m_rigidBody.velocity, v, _velocityLerpRate * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(m_groundAt.point, 0.1f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }

    private void UpdateAnimator()
    {
        m_animator.SetFloat("Forward", m_forward);
        m_animator.SetFloat("Turn", m_turn);

        if (m_grounded && m_forward > 0.0f)
        {
            float runCycle = Mathf.Repeat(m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
            m_jumpLeg = (runCycle < 0.5f ? 1 : -1);
        }
        m_animator.SetFloat("JumpLeg", m_jumpLeg);

        m_animator.SetBool("Grounded", m_grounded);
        m_animator.SetBool("Jumping", m_jumping);
        m_animator.SetBool("Crouching", m_crouching);

        m_animator.SetBool("Dancing", m_dancing);
        m_animator.SetFloat("Dance", m_dance);
    }

    private void checkGround()
    {
        Debug.DrawLine(transform.position + Vector3.up * 0.5f, transform.position + Vector3.up * 0.25f + Vector3.down, Color.yellow);
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out m_groundAt, m_groundCheckDist + 0.5f, ~LayerMask.NameToLayer("Default")))
        {            
            //Debug.Log("ground");
            m_grounded = true;
            m_rigidBody.useGravity = false;
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, m_groundAt.point.y, m_lerpToGround), transform.position.z);
        }
        else
        {
            //Debug.Log("air");
            m_grounded = false;
            m_rigidBody.useGravity = true;
            //m_rigidBody.velocity += Vector3.down * 9.8f * Time.deltaTime;
        }
    }

    private void AboutFace(Vector3 dir)
    {
        if (!m_aboutFace)
        {
            StartCoroutine(FacingDirection(dir));
        }
    }

    private IEnumerator FacingDirection(Vector3 dir)
    {
        m_aboutFace = true;

        AnimatorStateInfo animState = m_animator.GetCurrentAnimatorStateInfo(0);

        while (!animState.IsName("QuickTurnLeftFoot") && !animState.IsName("QuickTurnRightFoot"))
        {
            //wait for animation
            animState = m_animator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }

        /*dir = Vector3.ProjectOnPlane(dir, Vector3.up).normalized;
        Quaternion tarRot = Quaternion.LookRotation(dir, transform.up);
        Quaternion startRot = transform.rotation;
        
        float ang = Quaternion.Angle(startRot, tarRot);
        if (m_jumpLeg > 0.0f)
        {
            ang = ang - 360.0f;
        }*/
        
        //Quaternion tarRot = Quaternion.LookRotation(dir, transform.up);
        while (animState.normalizedTime < 1.0f && (animState.IsName("QuickTurnLeftFoot") || animState.IsName("QuickTurnRightFoot")))
        {
            
            //transform.rotation = startRot * Quaternion.Euler(0.0f, -ang * Mathf.SmoothStep(0.0f, 1.0f, animState.normalizedTime), 0.0f);
            //transform.rotation = Quaternion.Lerp(transform.rotation, tarRot, Mathf.SmoothStep(0.95f, 1.0f, animState.normalizedTime));

            animState = m_animator.GetCurrentAnimatorStateInfo(0);
            yield return null;
        }

        //transform.rotation = tarRot;
        m_aboutFace = false;

        yield return null;        
    }

    private void RotateCharacter()
    {
        // In addition to root rotation in the animation
        float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, Mathf.Abs(m_forward));
        transform.Rotate(0.0f, m_turn * turnSpeed * Time.deltaTime, 0.0f);        
    }
    
    public void Move(Vector2 input)
    {
        if (m_grounded)
        {
            Quaternion rot = Quaternion.Euler(0.0f, m_camera.transform.rotation.eulerAngles.y - transform.rotation.eulerAngles.y, 0.0f);
            m_move = Vector3.Lerp(m_move, rot * new Vector3(input.x, 0.0f, input.y), 10.0f * Time.deltaTime);
            m_forward = m_move.z + 0.0001f;
            m_turn = m_move.x + 0.0001f;
        }

        if (m_forward < -0.5f)
        {
            AboutFace(transform.TransformDirection(m_move));
        }
        else if (!m_aboutFace && !m_dancing)
        {
            RotateCharacter();
        }     
    }

    public void SetDancing(bool dancing, float dance)
    {
        m_dancing = dancing;
        m_dance = dance;
    }
}