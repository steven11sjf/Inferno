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
    private bool[] IsColored;
    



    private void Awake()
    {
        m_TextComponent = GetComponent<TMP_Text>();
        IsColored = new bool[MAX_CHARACTER_COUNT];
        m_CharToColor = 0;
        m_CharacterCount = m_TextComponent.textInfo.characterCount;
    }

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

        int characterCount = textInfo.characterCount;

        for (int currentCharacter = 0; currentCharacter < characterCount; currentCharacter++)
        {
            // get the index of the material
            int materialIndex = textInfo.characterInfo[currentCharacter].materialReferenceIndex;

            // get the vertex colors of the mesh used by this text element
            newVertexColors = textInfo.meshInfo[materialIndex].colors32;

            // get the index of the first vertex
            int vertexIndex = textInfo.characterInfo[currentCharacter].vertexIndex;

            c0 = new Color32(c0.r, c0.g, c0.b, 0);
            // change the vertex colors if the element is visible
            if (textInfo.characterInfo[currentCharacter].isVisible)
            {
                newVertexColors[vertexIndex + 0].a = 0;
                newVertexColors[vertexIndex + 1].a = 0;
                newVertexColors[vertexIndex + 2].a = 0;
                newVertexColors[vertexIndex + 3].a = 0;
            }

            // push changes to the text component
            m_TextComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }
    }

    public void ColorVertex()
    {
        // verify there are still characters left to color
        if (m_CharToColor > m_CharacterCount) return;

        TMP_TextInfo textInfo = m_TextComponent.textInfo;

        Color32[] newVertexColors;

        int materialIndex = textInfo.characterInfo[m_CharToColor].materialReferenceIndex;

        newVertexColors = textInfo.meshInfo[materialIndex].colors32;

        int vertexIndex = textInfo.characterInfo[m_CharToColor].vertexIndex;

        if (textInfo.characterInfo[m_CharToColor].isVisible)
        {
            newVertexColors[vertexIndex + 0].a = 255;
            newVertexColors[vertexIndex + 1].a = 255;
            newVertexColors[vertexIndex + 2].a = 255;
            newVertexColors[vertexIndex + 3].a = 255;
        }

        m_TextComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

        m_CharToColor++;
    }
}
