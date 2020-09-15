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

    public GameObject DialogueBox;
    public GameObject NameTextBox;
    public GameObject MainDialogueBox;
    public Text decisionChoice1, decisionChoice2, decisionChoice3;
    
    private Text NameText;
    private TMP_Text MessageText;
    private VertexWave VertexWaveObject;
    private VertexColor VertexColorObject;
    private Image DialogueBoxName;
    private Animator DialogueBoxAnimator;



    // allows us to see the current strings in the inspector
    public string X_message;
    public string X_typewriter;

    // the current (variable) type speed and the player's selected type speed
    public float DefaultMediumTypeSpeed;
    public float TypeSpeed;

    private bool inDialogue;
    private bool isTyping;
    private bool isSkippable;

    private DialogueGraph dialogue;
    private Chat current;
    
    // Start is called before the first frame update
    void Start()
    {
        NameText = NameTextBox.GetComponent<Text>();
        MessageText = MainDialogueBox.GetComponent<TMP_Text>();
        VertexWaveObject = MainDialogueBox.GetComponent<VertexWave>();
        VertexColorObject = MainDialogueBox.GetComponent<VertexColor>();
        DialogueBoxName = DialogueBox.GetComponent<Image>();
        DialogueBoxAnimator = DialogueBox.GetComponent<Animator>();
        DefaultMediumTypeSpeed = TYPE_SPEED_MEDIUM;
        inDialogue = false;
        isTyping = false;

        TypeSpeed = DefaultMediumTypeSpeed;

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

                    TypeSpeed = DefaultMediumTypeSpeed;
                    MessageText.text = ""; // needed to properly reset vertices
                    MessageText.text = StripTypewriterCommands(dialogue.current.text);
                    VertexColorObject.ColorAllVertices();
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
        if (current == null)
        {
            Debug.Log("DialogueManager: null");
            
        }

        // disable all buttons
        decisionChoice1.transform.parent.gameObject.SetActive(false);
        decisionChoice2.transform.parent.gameObject.SetActive(false);
        decisionChoice3.transform.parent.gameObject.SetActive(false);

        // set name
        NameText.text = current.character.m_name;

        // set color
        DialogueBoxName.color = current.character.color;

        // open dialogue
        DialogueBoxAnimator.SetBool("IsOpen", true);

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
        MessageText.text = "";
        yield return new WaitForSeconds(0.3f);
        VertexWaveObject.ResetCharacterWave();
        isSkippable = true;

        string messageText = StripTypewriterCommands(message);
        string typewriterText = ToTypewriterCommands(message);
        X_message = messageText;
        X_typewriter = typewriterText;

        MessageText.text = messageText;
        VertexColorObject.UncolorAllVertices();

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

                    string command, param;
                    SplitTag(tag, out command, out param);

                    switch(command)
                    {
                        case "pause":
                            float pauseLength;
                            substring = param;

                            float.TryParse(param, out pauseLength);
                            yield return new WaitForSeconds(pauseLength);
                            break;

                        case "speed":
                            float newSpeed;
                            substring = param;

                            float.TryParse(substring, out newSpeed);
                            TypeSpeed = newSpeed;
                            break;

                        case "/speed":
                            TypeSpeed = DefaultMediumTypeSpeed;
                            break;
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
                VertexColorObject.ColorVertex();
                ++cur;
                yield return new WaitForSeconds(TypeSpeed);

                if (c == '.' || c == '!' || c == '?')
                {
                    yield return new WaitForSeconds(TypeSpeed * 2f);
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
        bool isWaving = false;
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
                        string command, param;
                        SplitTag(tag, out command, out param);
                        switch (command)
                        {
                            case "wave":
                                isWaving = true;
                                break;

                            case "/wave":
                                isWaving = false;
                                break;

                            default:
                                result += tag;
                                break;
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
                if (isWaving)
                {
                    VertexWaveObject.AddWaveToCharacter(curr);
                }
                ++curr;
            }
        }
        X_message = result;

        return result;
    }

    /// <summary>
    /// Splits a tag into a command and the parameter provided
    /// </summary>
    /// <param name="tag">The unchanged tag parsed</param>
    /// <param name="command">either the contents of the tag or the part of the tag between left angle-bracket and equals sign </param>
    /// <param name="param">An empty string or the part of the tag between equals and right angle-bracket </param>
    /// <returns>true if the tag has a parameter</returns>
    private bool SplitTag(string tag, out string command, out string param)
    {
        int indexOfEquals = tag.IndexOf("=");
        if(indexOfEquals == -1)
        {
            command = tag.Substring(1, tag.IndexOf(">") - 1);
            param = "";
            return false;
        }
        else
        {
            command = tag.Substring(1, indexOfEquals - 1);
            param = tag.Substring(indexOfEquals + 1, tag.IndexOf(">") - indexOfEquals - 1);
            return true;
        }
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

        string command;
        string param;
        SplitTag(tag, out command, out param);

        switch(command)
        {
            case "b":
            case "/b":
            case "i":
            case "/i":
            case "color":
            case "/color":
            case "size":
            case "/size":
            case "wave":
            case "/wave":
                result = false;
                break;

            case "speed":
            case "/speed":
            case "pause":
                result = true;
                break;

            default:
                Debug.Log("Command not found!");
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
        DialogueBoxAnimator.SetBool("IsOpen", false);
    }
}
