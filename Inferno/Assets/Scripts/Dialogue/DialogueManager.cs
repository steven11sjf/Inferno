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
    private static float END_OF_SENTENCE_DELAY = 0.15f;

    public DialogueAnimator dialogueAnimator;

    public Text nameText;
    public Text messageText;
    public TMP_Text m_MessageText;
    public VertexJitter m_VertexJitter;
    public Image decisionBox;
    public Text decisionChoice1, decisionChoice2, decisionChoice3;
    public Animator animator;

    public string X_message;
    public string X_typewriter;

    public float typeSpeed;

    private bool inDialogue;
    private bool isTyping;

    private bool isTextJittering;

    public List<string> currentRichTextTags;
    public string currentString;
    public string tempString;
    public bool isParsingTag;
    private int currentCharIndex;

    public DialogueGraph dialogue;
    public Chat current;
    
    // Start is called before the first frame update
    void Start()
    {
        inDialogue = false;
        isTyping = false;
        // disable all buttons
        decisionChoice1.transform.parent.gameObject.SetActive(false);
        decisionChoice2.transform.parent.gameObject.SetActive(false);
        decisionChoice3.transform.parent.gameObject.SetActive(false);

        currentRichTextTags = new List<string>();
    }

    // Update is called once per frame
    void Update()
    {
        if(inDialogue)
        {
            if(Input.GetKeyDown("e"))
            {
                // if still typing, complete sentence
                if (isTyping)
                {
                    Debug.Log("Entering IsTyping");
                    StopAllCoroutines();

                    isTyping = false;
                    currentRichTextTags.Clear();
                    currentString = "";
                    tempString = "";
                    isParsingTag = false;

                    m_MessageText.text = ParseMessage(dialogue.current.text);
                    ShowAnswers(); // show answers, if any
                }
                else
                {
                    // if there are no responses, continue to next dialogue
                    if(dialogue.current.answers.Count == 0)
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
        Debug.Log("Answered " + i.ToString());
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

        // reset DialogueAnimator jittering
        dialogueAnimator.Reset();

        // set name
        nameText.text = current.character.m_name;

        // set color
        decisionBox.color = current.character.color;

        // open dialogue
        animator.SetBool("IsOpen", true);

        // start coroutine to type
        StopAllCoroutines();
        StartCoroutine(TMP_TypeMessage(current.text));
    }

    IEnumerator TMP_TypeMessage(string message)
    {
        isTyping = true;
        m_VertexJitter.ResetJitter();

        string messageText = StripTypewriterCommands(message);
        string typewriterText = ToTypewriterCommands(message);

        X_message = messageText;
        X_typewriter = typewriterText;

        m_MessageText.text = messageText;
        // TODO SetTextClear();

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
                        typeSpeed = TYPE_SPEED_MEDIUM;
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
                // TODO ShowNextChar()
                ++cur;
                yield return new WaitForSeconds(typeSpeed);

                if (c == '.' || c == '!' || c == '?')
                {
                    yield return new WaitForSeconds(typeSpeed * 2.5f);
                }
            }
        }
        Debug.Log("Finished typeing");

        ShowAnswers();
        isTyping = false;
    }

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
    /// Prints message using typewriter effect at rate of one letter per speed frames
    /// </summary>
    /// <param name="message">The text to put in the dialogue box's main text</param>
    /// <param name="speed">how fast to print letters</param>
    /// <returns></returns>
    IEnumerator TypeMessage(string message)
    {
        m_VertexJitter.ResetJitter();

        currentCharIndex = 0;
        isTextJittering = false;
        isTyping = true;
        m_MessageText.text = "";
        currentString = "";

        foreach (char letter in message.ToCharArray())
        {
            if (isParsingTag)
            {
                if (letter == '>')
                {
                    tempString += letter;
                    float pauseFrames = ParseTag(tempString);
                    isParsingTag = false;

                    float unpauseTime = Time.time + pauseFrames;
                    while(unpauseTime > Time.time)
                    {
                        yield return null;
                    }
                }
                else
                {
                    tempString += letter;
                }
            }
            else if (letter == '<')
            {
                isParsingTag = true;
                tempString = "<";
            }
            else
            {
                yield return new WaitForSeconds(typeSpeed);

                currentString += letter;
                if (isTextJittering) m_VertexJitter.AddJitterToCharacter(currentCharIndex);
                ++currentCharIndex;
            }

            // print current string to text box
            string temp = currentString;
            foreach(string s in currentRichTextTags)
            {
                temp += s;
            }
            m_MessageText.text = temp;

            // add delay if end of a sentence
            if (letter == '.' || letter == '?' || letter == '!')
            {
                yield return new WaitForSeconds(END_OF_SENTENCE_DELAY);
            }
        }

        ShowAnswers();

        isTyping = false;
    }

    string ParseMessage(string message)
    {
        string result = "";
        
        m_MessageText.text = "";
        currentString = "";
        currentCharIndex = 0;
        isTextJittering = false;

        foreach (char letter in message.ToCharArray())
        {
            if (isParsingTag)
            {
                if (letter == '>')
                {
                    tempString += letter;
                    ParseTag(tempString);
                    isParsingTag = false;
                }
                else
                {
                    tempString += letter;
                }
            }
            else if (letter == '<')
            {
                isParsingTag = true;
                tempString = "<";
            }
            else
            {
                currentString += letter;
                if (isTextJittering) m_VertexJitter.AddJitterToCharacter(currentCharIndex);
                ++currentCharIndex;
            }
        }
        result = currentString;
        return result;
    }

    /// <summary>
    /// Parses a tag given by the DialogueManager
    /// </summary>
    /// <param name="tag">The tag read from the string</param>
    /// <returns>the number of frames to pause after the tag</returns>
    private float ParseTag(string tag)
    {
        float retVal = 0.0f;
        char[] charArr = tag.ToCharArray();

        string toParse;
        switch (charArr[1])
        {
            // bold
            case 'b':
                currentString += "<b>";
                currentRichTextTags.Insert(0, "</b>");
                break;

            // italics
            case 'i':
                currentString += "<i>";
                currentRichTextTags.Insert(0, "</i>");
                break;

            // pause typing
            case 'p':
                float pauseLength;
                toParse = tag.Substring(7, tag.Length - 8);

                float.TryParse(toParse, out pauseLength);
                retVal = pauseLength;
                break;

            // change color
            case 'c':
                currentString += tag;
                currentRichTextTags.Insert(0, "</color>");
                break;

            // change speed
            case 's':
                float newSpeed;
                toParse = tag.Substring(7, tag.Length - 8);

                float.TryParse(toParse, out newSpeed);
                typeSpeed = newSpeed;
                break;

            // enable text jitter
            case 'j':
                Debug.Log("Jittering!");
                isTextJittering = true;
                break;

            // closing tag
            case '/':
                switch (charArr[2])
                {
                    // bold
                    case 'b':
                        if (currentRichTextTags.First().Equals("</b>"))
                        {
                            currentString += "</b>";
                            currentRichTextTags.RemoveAt(0);
                        }
                        else
                        {
                            Debug.Log("DialogueManager: Expected " + currentRichTextTags.First() + " not </b>");
                        }
                        break;

                    // italics
                    case 'i':
                        if (currentRichTextTags.First().Equals("</i>"))
                        {
                            currentString += "</i>";
                            currentRichTextTags.RemoveAt(0);
                        }
                        else
                        {
                            Debug.Log("DialogueManager: Expected " + currentRichTextTags.First() + " not </i>");
                        }
                        break;

                    // color
                    case 'c':
                        if (currentRichTextTags.First().Equals("</color>"))
                        {
                            currentString += "</color>";
                            currentRichTextTags.RemoveAt(0);
                        }
                        else
                        {
                            Debug.Log("DialogueManager: Expected " + currentRichTextTags.First() + " not " + tag);
                        }
                        break;

                    // speed
                    case 's':
                        typeSpeed = TYPE_SPEED_MEDIUM;
                        break;

                    // remove jittering
                    case 'j':
                        isTextJittering = false;
                        break;

                    // unknown
                    default:

                        break;
                }
                break;

            default:

                break;
        }

        return retVal;
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
