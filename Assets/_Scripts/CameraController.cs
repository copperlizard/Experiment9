using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField]
    private GameObject m_target;

    private GameObject m_boon;

    private Vector3 m_tarOffset = Vector3.zero;

	// Use this for initialization
	void Start ()
    {
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
	}

    public void Move (Vector2 input)
    {

    } 
}
