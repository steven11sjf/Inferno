using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Dialogue;
using System.Runtime.InteropServices;
using System.Linq;
using System.CodeDom;
using TMPro;
using TMPro.Examples;
using System.Runtime.CompilerServices;
using System.Threading;

public class DialogueManager : MonoBehaviour
{
    private static float TYPE_SPEED_MEDIUM = 0.05f;


    public Text nameText;
    public TMP_Text m_MessageText;
    public VertexJitter m_VertexJitter;
    public VertexColor m_VertexColor;
    public Image decisionBox;
    public Text decisionChoice1, decisionChoice2, decisionChoice3;
    public Animator animator;

    // allows us to see the current strings in the inspector
    public string X_message;
    public string X_typewriter;

    // the current (variable) type speed and the player's selected type speed
    public float defaultTypeSpeed;
    public float typeSpeed;

    private bool inDialogue;
    private bool isTyping;
    private bool isSkippable;

    private DialogueGraph dialogue;
    private Chat current;
    
    // Start is called before the first frame update
    void Start()
    {
        defaultTypeSpeed = TYPE_SPEED_MEDIUM;
        inDialogue = false;
        isTyping = false;
        // disable all buttons
        decisionChoice1.transform.parent.gameObject.SetActive(false);
        decisionChoice2.transform.parent.gameObject.SetActive(false);
        decisionChoice3.transform.parent.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(inDialogue)
        {
            if(Input.GetKeyDown("e"))
            {
                // if still typing, complete sentence
                if (isSkippable)
                {
                    
                    StopAllCoroutines();
                    isTyping = false;
                    isSkippable = false;

                    typeSpeed = defaultTypeSpeed;
                    m_MessageText.text = ""; // needed to properly reset vertices
                    m_MessageText.text = StripTypewriterCommands(dialogue.current.text);
                    m_VertexColor.ColorAllVertices();
                    ShowAnswers(); // show answers, if any
                }
                else
                {
                    // if there are no responses, continue to next dialogue
                    if(!isTyping && dialogue.current.answers.Count == 0)
                    {
                        dialogue.AnswerQuestion(0);
                        ShowDialogue();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Starts a conversation using the dialogue tree given
    /// </summary>
    /// <param name="graph">The dialogue tree</param>
    public void StartDialogue(DialogueGraph graph)
    {
        inDialogue = true;
        dialogue = graph;

        graph.Restart();
        ShowDialogue();
    }

    /// <summary>
    /// Selects an answer for the current node
    /// </summary>
    /// <param name="i"></param>
    public void SelectAnswer(int i)
    {
        dialogue.AnswerQuestion(i);
        ShowDialogue();
    }

    /// <summary>
    /// Prints current chat to dialogue box and activates buttons if required. 
    /// </summary>
    private void ShowDialogue()
    {
        if (dialogue == null) return;

        current = dialogue.current;
        if (current == null) Debug.Log("DialogueManager: null");

        // disable all buttons
        decisionChoice1.transform.parent.gameObject.SetActive(false);
        decisionChoice2.transform.parent.gameObject.SetActive(false);
        decisionChoice3.transform.parent.gameObject.SetActive(false);

        // set name
        nameText.text = current.character.m_name;

        // set color
        decisionBox.color = current.character.color;

        // open dialogue
        animator.SetBool("IsOpen", true);

        isTyping = true;

        // start coroutine to type
        StopAllCoroutines();
        StartCoroutine(TMP_TypeMessage(current.text));
    }

    /// <summary>
    /// Types the message out to the TextMesh Pro text box. Prints the Rich Text with an alpha of zero and reveals each character based on the typing speed.
    /// </summary>
    /// <param name="message">A string containing both typewriter commands and rich text</param>
    IEnumerator TMP_TypeMessage(string message)
    {
        m_MessageText.text = "";
        yield return new WaitForSeconds(0.3f);
        m_VertexJitter.ResetJitter();
        isSkippable = true;

        string messageText = StripTypewriterCommands(message);
        string typewriterText = ToTypewriterCommands(message);
        X_message = messageText;
        X_typewriter = typewriterText;

        m_MessageText.text = messageText;
        m_VertexColor.UncolorAllVertices();

        bool parsing = false;
        string tag = "";
        string substring;
        int cur = 0;
        foreach (char c in typewriterText.ToCharArray())
        {
            if (parsing)
            {
                if (c == '>')
                {
                    tag += ">";

                    if (tag[1] == 'p')
                    {
                        float pauseLength;
                        substring = tag.Substring(7, tag.Length - 8);

                        float.TryParse(substring, out pauseLength);
                        yield return new WaitForSeconds(pauseLength);
                    }
                    else if (tag[1] == 's')
                    {
                        float newSpeed;
                        substring = tag.Substring(7, tag.Length - 8);

                        float.TryParse(substring, out newSpeed);
                        typeSpeed = newSpeed;
                    }
                    else if (tag[1] == '/' && tag[2] == 's')
                    {
                        typeSpeed = defaultTypeSpeed;
                    }

                    parsing = false;
                }
                else
                {
                    tag += c;
                }
            }
            else if (c == '<')
            {
                parsing = true;
                tag = "<";
            }
            else
            {
                m_VertexColor.ColorVertex();
                ++cur;
                yield return new WaitForSeconds(typeSpeed);

                if (c == '.' || c == '!' || c == '?')
                {
                    yield return new WaitForSeconds(typeSpeed * 2f);
                }
            }
        }

        ShowAnswers();
        isTyping = false;
        isSkippable = false;
    }

    /// <summary>
    /// Removes all rich-text from the string so it only contains typewriter commands
    /// </summary>
    /// <param name="message">A string containing rich text</param>
    /// <returns>The string with no rich text</returns>
    private string ToTypewriterCommands(string message)
    {
        string result = "";

        string tag = "";
        bool parsing = false;
        foreach (char letter in message.ToCharArray())
        {
            if (parsing)
            {
                if (letter == '>')
                {
                    tag += letter;
                    if (InterpretTag(tag)) result += tag;
                    parsing = false;
                }
                else
                {
                    tag += letter;
                }
            }
            else if (letter == '<')
            {
                parsing = true;
                tag = "<";
            }
            else
            {
                result += letter;
            }
        }

        return result;
    }

    /// <summary>
    /// Removes all the typewriter commands
    /// </summary>
    /// <param name="message">A message with both rich text and typewriter commands</param>
    /// <returns>A message only containing rich text which can be placed directly in the text box</returns>
    private string StripTypewriterCommands(string message)
    {
        string result = "";

        string tag = "";
        bool parsing = false;
        bool isJittering = false;
        int curr = 0;
        foreach (char letter in message.ToCharArray())
        {
            if (parsing)
            {
                if (letter == '>')
                {
                    tag += letter;
                    bool res = InterpretTag(tag);
                    if(!res)
                    {
                        if (tag[1] == 'j')
                        {
                            isJittering = true;
                        }
                        else if (tag[1] == '/' && tag[2] == 'j')
                        {
                            isJittering = false;
                        }
                        else
                        {
                            result += tag;
                        }
                    }
                    parsing = false;
                }
                else
                {
                    tag += letter;
                }
            }
            else if (letter == '<')
            {
                parsing = true;
                tag = "<";
            }
            else
            {
                result += letter;
                if (isJittering)
                {
                    m_VertexJitter.AddJitterToCharacter(curr);
                }
                ++curr;
            }
        }
        X_message = result;

        return result;
    }
    
    /// <summary>
    /// Interprets a tag and returns true if it is a typewriter command, false if not
    /// </summary>
    /// <param name="tag">the tag being parsed</param>
    /// <returns>true if tag is a pause tag or a speed tag</returns>
    private bool InterpretTag(string tag)
    {
        bool result = false;
        char[] tagArray = tag.ToCharArray();

        switch (tagArray[1])
        {
            case 'b':
                result = false;
                break;

            case 'i':
                result = false;
                break;

            case 'c':
                result = false;
                break;

            case 's':
                if (tagArray[2] == 'i') result = false;
                else result = true;
                break;

            case 'j':
                result = false;
                break;

            case 'p':
                result = true;
                break;

            case '/':
                switch (tagArray[2])
                {
                    case 'b':
                        result = false;
                        break;

                    case 'i':
                        result = false;
                        break;

                    case 'c':
                        result = false;
                        break;

                    case 's':
                        if (tagArray[3] == 'i') result = false;
                        else result = true;
                        break;

                    case 'j':
                        result = false;
                        break;
                    default:
                        Debug.Log("DialogueManager: Tag not found " + tag);
                        break;
                }
                break;

            default:
                Debug.Log("DialogueManager: Tag not found " + tag);
                break;
        }

        return result;
    }
    /// <summary>
    /// Activates buttons if required
    /// </summary>
    private void ShowAnswers()
    {
        // determine if message or decision
        int count = dialogue.current.answers.Count;
        if (count != 0)
        {
            decisionChoice1.text = dialogue.current.answers[0].text;
            decisionChoice1.transform.parent.gameObject.SetActive(true);

            if (count >= 2)
            {
                decisionChoice2.text = dialogue.current.answers[1].text;
                decisionChoice2.transform.parent.gameObject.SetActive(true);
            }

            if (count == 3)
            {
                decisionChoice3.text = dialogue.current.answers[2].text;
                decisionChoice3.transform.parent.gameObject.SetActive(true);
            }
        }
    }

 
    /// <summary>
    /// Resets the DialogueManager's state and hides the dialogue box
    /// </summary>
    public void EndDialogue()
    {
        inDialogue = false;
        current = null;
        dialogue = null;
        animator.SetBool("IsOpen", false);
    }
}
