using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueAnimator : MonoBehaviour
{
    public int MAX_CHAR_COUNT;

    public int m_NumAnimatedCharacters;
    public float CurveScale = 1.0f;
    public float AngleMultiplier = 1.0f;

    private TMP_Text m_TextComponent;
    private int m_CharsPrinted;
    public bool[] m_IsAnimated;

    private struct VertexAnim
    {
        public float angleRange;
        public float angle;
        public float speed;
    }

    private void Awake()
    {
        m_TextComponent = GetComponent<TMP_Text>();
    }
    


    // Start is called before the first frame update
    void Start()
    {
        m_IsAnimated = new bool[MAX_CHAR_COUNT];
        m_NumAnimatedCharacters = 0; // no characters are animated initially

        StartCoroutine(AnimateVertices());
    }

    IEnumerator AnimateVertices()
    {
        // force an update of the text objects
        m_TextComponent.ForceMeshUpdate();

        TMP_TextInfo textInfo = m_TextComponent.textInfo;

        Matrix4x4 matrix;

        int loopCount = 0;

        // create an array containing pre-computed Angle Ranges and Speeds for a bunch of characters
        VertexAnim[] vertexAnims = new VertexAnim[MAX_CHAR_COUNT];
        for (int i = 0; i < MAX_CHAR_COUNT; i++)
        {
            vertexAnims[i].angleRange = Random.Range(10f, 25f);
            vertexAnims[i].speed = Random.Range(1f, 3f);
        }

        TMP_MeshInfo[] cachedMeshInfo = textInfo.CopyMeshInfoVertexData();

        while(true)
        {
            // if no characters are animated then just yield and wait
            if (m_NumAnimatedCharacters == 0)
            {
                yield return new WaitForSeconds(0.25f);
                continue;
            }

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

                // skip to next character if this one isn't animated or isn't visible
                if (!m_IsAnimated[i] || !charInfo.isVisible)
                {
                    continue;
                }

                VertexAnim vertAnim = vertexAnims[i];

                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;

                int vertexIndex = textInfo.characterInfo[i].vertexIndex;

                Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;

                Vector2 charMidBaseline = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;

                Vector3 offset = charMidBaseline;

                Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;

                destinationVertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] - offset;
                destinationVertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] - offset;
                destinationVertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] - offset;
                destinationVertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] - offset;

                vertAnim.angle = 0;
                Vector3 jitterOffset = new Vector3(Random.Range(-.05f, .05f), Random.Range(-.25f, .25f), 0);

                matrix = Matrix4x4.TRS(jitterOffset * CurveScale, Quaternion.identity, Vector3.one);

                destinationVertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 0]);
                destinationVertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 1]);
                destinationVertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 2]);
                destinationVertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(destinationVertices[vertexIndex + 3]);

                destinationVertices[vertexIndex + 0] += offset;
                destinationVertices[vertexIndex + 1] += offset;
                destinationVertices[vertexIndex + 2] += offset;
                destinationVertices[vertexIndex + 3] += offset;

                vertexAnims[i] = vertAnim;
            }

            // push changes into mesh
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                m_TextComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
            }

            loopCount++;

            yield return new WaitForSeconds(0.05f);
        }
    }

    public void Reset()
    {
        m_IsAnimated = new bool[MAX_CHAR_COUNT];
        for(int i=0; i<256; i++)
        {
            m_IsAnimated[i] = true;
        }
        m_NumAnimatedCharacters = 0;
    }

    public void ShakeCharacter(int position)
    {
        m_IsAnimated[position] = true;
        m_NumAnimatedCharacters++;
    }
}
