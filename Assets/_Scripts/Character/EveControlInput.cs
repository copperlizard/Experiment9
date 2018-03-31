using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EveController))]
public class EveControlInput : MonoBehaviour {

    private EveController m_eveController;

    private Vector2 m_move = Vector2.zero;

    private float m_dance = 0.0f;
    private bool m_dancing = false;

	// Use this for initialization
	void Start ()
    {
        m_eveController = GetComponent<EveController>();
        if (m_eveController == null)
        {
            Debug.Log("m_eveController not found!");
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        GetInput();
    }

    private void GetInput()
    {
        //Add control options here

        GetXBOXcontrolInput();

        m_eveController.Move(m_move);
        m_eveController.SetDancing(m_dancing, m_dance);
    }

    private void GetXBOXcontrolInput()
    {
        m_move.x = Input.GetAxis("Xaxis");
        m_move.y = Input.GetAxis("Yaxis");

        if (Input.GetButtonDown("Dance"))
        {
            m_dancing = !m_dancing;
        }

        if (Input.GetButtonDown("NextDance"))
        {
            m_dance += 1.0f;
        }

        if (Input.GetButtonDown("PrevDance"))
        {
            m_dance -= 1.0f;
        }

        if (m_dance < 0.0f)
        {
            m_dance = 5.0f;
        }
        else if (m_dance > 5.0f)
        {
            m_dance = 0.0f;
        }
    }
}
