using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EveController : MonoBehaviour
{
    [SerializeField]
    private float m_groundCheckDist = 0.01f, m_lerpToGround = 0.1f;

    private Rigidbody m_rigidBody;

    private RaycastHit m_groundAt;

    private bool m_grounded = true;

    // Use this for initialization
    void Start()
    {
        m_rigidBody = GetComponent<Rigidbody>();
        if (m_rigidBody == null)
        {
            Debug.Log("m_rigidBody not found!");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        checkGround();
    }

    public void checkGround()
    {
        if(!Physics.Raycast(transform.position + Vector3.up * 0.25f, Vector3.down, out m_groundAt, m_groundCheckDist + 0.25f, LayerMask.NameToLayer("Default")))
        {
            m_grounded = true;
            m_rigidBody.useGravity = false;
            transform.position = new Vector3(transform.position.x, Mathf.Lerp(transform.position.y, m_groundAt.point.y, m_lerpToGround), transform.position.z);
        }
        else
        {
            m_grounded = false;
            m_rigidBody.useGravity = true;
        }
    }

    public void Move(Vector2 input)
    {
        m_rigidBody.AddForce(new Vector3(input.x, 0.0f, input.y));
    }
}