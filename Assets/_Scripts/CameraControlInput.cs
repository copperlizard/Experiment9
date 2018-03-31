using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlInput : MonoBehaviour {

    private CameraController m_camController;

    private Vector2 m_move = Vector2.zero;

    // Use this for initialization
    void Start()
    {
        m_camController = GetComponent<CameraController>();
        if (m_camController == null)
        {
            Debug.Log("m_eveController not found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
    }

    private void GetInput()
    {
        //Add control options here

        GetXBOXcontrolInput();

        m_camController.Move(m_move);
    }

    private void GetXBOXcontrolInput()
    {
        m_move.x = Input.GetAxis("lXaxis");
        m_move.y = Input.GetAxis("lYaxis");
    }
}
