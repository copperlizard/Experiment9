using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EveController))]
public class EveControlInput : MonoBehaviour {

    private EveController m_eveController;

    private Vector2 m_move = Vector2.zero;

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
    }

    private void GetXBOXcontrolInput()
    {
        m_move.x = Input.GetAxis("Xaxis");
        m_move.y = Input.GetAxis("Yaxis");
    }
}
