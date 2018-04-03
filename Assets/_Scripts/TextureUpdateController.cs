﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureUpdateController : MonoBehaviour
{   
    [SerializeField]
    private Material m_material;

    [SerializeField]
    private RenderTexture m_srcTexture, m_outTexture;

    private RenderTexture m_texHist;

    //public RenderTexture m_outTexture;

    // Use this for initialization
    void Start ()
    {
        m_texHist = new RenderTexture(m_srcTexture.width, m_srcTexture.height, m_srcTexture.depth, RenderTextureFormat.ARGB32);
        m_texHist.enableRandomWrite = true;
        m_texHist.Create();
        m_material.SetTexture("_TexHist", m_texHist);
    }
	
	// Update is called once per frame
	void Update ()
    {
        UpdateTexture();
	}

    private void UpdateTexture()
    {
        //combine srcTex with texHist
        Graphics.Blit(m_srcTexture, m_texHist, m_material);

        //render combined tex to outtex
        Graphics.Blit(m_texHist, m_outTexture);
        
        //Graphics.Blit(m_srcTexture, m_outTexture);
    }
}
