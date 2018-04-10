using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureUpdateController : MonoBehaviour
{
    [SerializeField]
    private TerrainData m_terrain;

    [SerializeField]
    private Material m_material;

    [SerializeField]
    private Material m_snowMaterial;

    [SerializeField]
    private RenderTexture m_srcTexture, m_outTexture, m_debugTex;

    private RenderTexture m_texHist, m_texBuf;
    
    private Texture2D m_heightMap;

    // Use this for initialization
    void Start ()
    {
        if(m_terrain != null)
        {
            m_heightMap = new Texture2D(m_terrain.heightmapWidth, m_terrain.heightmapHeight, TextureFormat.Alpha8, false);

            float[,] heights = new float[0, 0];
            heights = m_terrain.GetHeights(0, 0, m_terrain.heightmapWidth, m_terrain.heightmapHeight);
            for (int y = 0; y < m_heightMap.height; y++)
            {
                for (int x = 0; x < m_heightMap.width; x++)
                {
                    //float d = Mathf.Clamp(heights[y, x] * 2.0f, 0.0f, 1.0f);
                    float d = heights[y, x];

                    //m_heightMap.SetPixel(x, y, Color.red * d);
                    m_heightMap.SetPixel(x, y, Color.white * d);
                }
            }
            
            m_heightMap.Apply();

            m_material.SetTexture("_HeightMap", m_heightMap);

            Graphics.Blit(m_heightMap, m_debugTex);

            if(m_snowMaterial != null)
            {
                m_snowMaterial.SetTexture("_HeightMap", m_heightMap);
            }
        }

        m_texHist = new RenderTexture(m_srcTexture.width, m_srcTexture.height, m_srcTexture.depth, RenderTextureFormat.ARGB32);
        m_texHist.enableRandomWrite = true;
        m_texHist.Create();
        m_material.SetTexture("_TexHist", m_texHist);

        m_texBuf = new RenderTexture(m_srcTexture.width, m_srcTexture.height, m_srcTexture.depth, RenderTextureFormat.ARGB32);
        m_texBuf.enableRandomWrite = true;
        m_texBuf.Create();
    }
	
	// Update is called once per frame
	void Update ()
    {
        UpdateTexture();
	}

    private void UpdateTexture()
    {
        Graphics.Blit(m_srcTexture, m_texBuf, m_material);
        Graphics.Blit(m_texBuf, m_texHist);
        Graphics.Blit(m_texHist, m_outTexture);
    }
}
