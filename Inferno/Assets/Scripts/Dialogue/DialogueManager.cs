using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Dialogue;
using System.Runtime.InteropServices;
using System.Linq;

public class DialogueManager : MonoBehaviour
{

    public Text nameText;
    public Text messageText;
    public Image decisionBox;
    public Text decisionChoice1, decisionChoice2, decisionChoice3;
    public Animator animator;

    public int typeSpeed;

    private bool inDialogue;
    private bool isTyping;

    public List<string> currentRichTextTags;
    public string currentString;
    public string tempString;
    public bool isParsingTag;

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
                    StopAllCoroutines();

                    isTyping = false;
                    currentRichTextTags.Clear();
                    currentString = "";
                    tempString = "";
                    isParsingTag = false;

                    messageText.text = dialogue.current.text;
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
        if (current == null) Debug.Log("null");

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

        // start coroutine to type
        StopAllCoroutines();
        StartCoroutine(TypeMessage(current.text, typeSpeed));
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
    IEnumerator TypeMessage(string message, int speed)
    {
        isTyping = true;
        messageText.text = "";
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
                for (int i = 0; i < speed; ++i)
                    yield return null;

                currentString += letter;
            }

            // print current string to text box
            string temp = currentString;
            foreach(string s in currentRichTextTags)
            {
                temp += s;
            }
            messageText.text = temp;

        }

        ShowAnswers();

        isTyping = false;
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
                string toParse = tag.Substring(3, tag.Length - 4);

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
        decisionBox.color = Color.white;
    }
}
