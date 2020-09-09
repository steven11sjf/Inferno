using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VertexColor : MonoBehaviour
{
    private int MAX_CHARACTER_COUNT = 1024;

    private TMP_Text m_TextComponent;

    private int m_CharacterCount;
    private int m_CharToColor;

    private void Awake()
    {
        m_TextComponent = GetComponent<TMP_Text>();
        m_CharToColor = 0;
        m_CharacterCount = m_TextComponent.textInfo.characterCount;
    }

    /// <summary>
    /// Sets alpha for all vertices to full (255)
    /// </summary>
    public void ColorAllVertices()
    {
        m_TextComponent.ForceMeshUpdate();

        Debug.Log("Coloring all");
        m_CharToColor = 0;
        m_CharacterCount = m_TextComponent.textInfo.characterCount;

        // force a mesh update
        m_TextComponent.ForceMeshUpdate();

        // get text info
        TMP_TextInfo textInfo = m_TextComponent.textInfo;

        Color32[] newVertexColors;
        Color32 c0 = m_TextComponent.color;
        c0 = new Color32(c0.r, c0.g, c0.b, 255);
        Debug.Log("c0: " + c0.ToString());
        int characterCount = textInfo.characterCount;

        for (int currentCharacter = 0; currentCharacter < characterCount; currentCharacter++)
        {
            // get the index of the material
            int materialIndex = textInfo.characterInfo[currentCharacter].materialReferenceIndex;

            // get the vertex colors of the mesh used by this text element
            newVertexColors = textInfo.meshInfo[materialIndex].colors32;

            // get the index of the first vertex
            int vertexIndex = textInfo.characterInfo[currentCharacter].vertexIndex;

            // change the vertex colors if the element is visible
            if (textInfo.characterInfo[currentCharacter].isVisible)
            {
                Color32 c1 = newVertexColors[vertexIndex];
                c1 = new Color32(c1.r, c1.g, c1.b, 255);
                newVertexColors[vertexIndex + 0] = c1;
                newVertexColors[vertexIndex + 1] = c1;
                newVertexColors[vertexIndex + 2] = c1;
                newVertexColors[vertexIndex + 3] = c1;
            }

            // push changes to the text component
            m_TextComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }
    }

    /// <summary>
    /// Sets the alpha on all characters to 0
    /// </summary>
    public void UncolorAllVertices()
    {

        m_TextComponent.ForceMeshUpdate();

        Debug.Log("Uncoloring");
        m_CharToColor = 0;
        m_CharacterCount = m_TextComponent.textInfo.characterCount;

        // force a mesh update
        m_TextComponent.ForceMeshUpdate();

        // get text info
        TMP_TextInfo textInfo = m_TextComponent.textInfo;

        Color32[] newVertexColors;
        Color32 c0 = m_TextComponent.color;
        c0 = new Color32(c0.r, c0.g, c0.b, 0);

        int characterCount = textInfo.characterCount;

        for (int currentCharacter = 0; currentCharacter < characterCount; currentCharacter++)
        {
            // get the index of the material
            int materialIndex = textInfo.characterInfo[currentCharacter].materialReferenceIndex;

            // get the vertex colors of the mesh used by this text element
            newVertexColors = textInfo.meshInfo[materialIndex].colors32;

            // get the index of the first vertex
            int vertexIndex = textInfo.characterInfo[currentCharacter].vertexIndex;

            
            // change the vertex colors if the element is visible
            if (textInfo.characterInfo[currentCharacter].isVisible)
            {
                Color32 c1 = newVertexColors[vertexIndex];
                c1 = new Color32(c1.r, c1.g, c1.b, 0);
                newVertexColors[vertexIndex + 0] = c1;
                newVertexColors[vertexIndex + 1] = c1;
                newVertexColors[vertexIndex + 2] = c1;
                newVertexColors[vertexIndex + 3] = c1;
            }

            // push changes to the text component
            m_TextComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }
    }

    /// <summary>
    /// Sets the alpha on the character at index m_CharToColor to full (255)
    /// </summary>
    public void ColorVertex()
    {
        
        // verify there are still characters left to color
        if (m_CharToColor > m_CharacterCount) return;

        TMP_TextInfo textInfo = m_TextComponent.textInfo;

        Color32[] newVertexColors;

        int materialIndex = textInfo.characterInfo[m_CharToColor].materialReferenceIndex;

        newVertexColors = textInfo.meshInfo[materialIndex].colors32;

        int vertexIndex = textInfo.characterInfo[m_CharToColor].vertexIndex;

        Color32 c0 = m_TextComponent.color;
        c0 = new Color32(c0.r, c0.g, c0.b, 255);

        if (textInfo.characterInfo[m_CharToColor].isVisible)
        {
            // get original color and only change alpha to 255
            Color32 c1 = newVertexColors[vertexIndex];
            c1 = new Color32(c1.r, c1.g, c1.b, 255);
            newVertexColors[vertexIndex + 0] = c1;
            newVertexColors[vertexIndex + 1] = c1;
            newVertexColors[vertexIndex + 2] = c1;
            newVertexColors[vertexIndex + 3] = c1;
        }

        m_TextComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

        m_CharToColor++;
    }
}
