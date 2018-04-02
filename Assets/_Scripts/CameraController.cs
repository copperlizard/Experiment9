using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField]
    private GameObject m_target;

    [SerializeField]
    private float m_tiltSpeed = 4.0f, m_yawSpeed = 4.0f, m_maxTilt = 60.0f, m_minTilt = -60.0f;

    private Camera m_camera;
    private GameObject m_boon;

    private Vector3 m_tarOffset = Vector3.zero;

    private float m_tilt = 0.0f;

	// Use this for initialization
	void Start ()
    {
        m_camera = Camera.main;
        if (m_camera == null)
        {
            Debug.Log("m_camera not found!");
        }

        m_boon = transform.GetChild(0).gameObject;
        if(m_boon == null)
        {
            Debug.Log("m_boon not found!");
        }

        if(m_target == null)
        {
            m_target = GameObject.FindGameObjectWithTag("Player");
            if(m_target == null)
            {
                Debug.Log("m_target not found/assigned!");
            }
        }		

        if(m_target != null)
        {
            m_tarOffset = m_target.transform.position - transform.position;
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.position = m_target.transform.position - m_tarOffset;

        m_camera.transform.rotation = Quaternion.LookRotation(((transform.position - (transform.up * 0.5f)) - m_camera.transform.position).normalized);
	}

    public void Move (Vector2 input)
    {
        if(m_tilt > m_maxTilt && input.y > 0.0f)
        {
            input.y = 0.0f;
        }
        else if(m_tilt < m_minTilt && input.y < 0.0f)
        {
            input.y = 0.0f;
        }

        m_tilt += input.y * m_tiltSpeed;

        m_boon.transform.localRotation = Quaternion.Euler(m_boon.transform.localEulerAngles.x + input.y * m_tiltSpeed, m_boon.transform.localEulerAngles.y + input.x * m_yawSpeed, m_boon.transform.localEulerAngles.z);
    } 
}
