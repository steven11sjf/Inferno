using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Dialogue;
using System.Runtime.InteropServices;

public class DialogueManager : MonoBehaviour
{

    public Text nameText;
    public Text messageText;
    public Text decisionChoice1, decisionChoice2, decisionChoice3;
    public Animator animator;

    public int typeSpeed;

    private bool inDialogue;
    private bool isTyping;
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
    }

    // Update is called once per frame
    void Update()
    {
        if(inDialogue)
        {
            if(Input.GetKeyDown("e"))
            {
                // TODO: handle e advancing dialogue

                // if still typing, complete sentence
                if (isTyping)
                {
                    StopAllCoroutines();
                    isTyping = false;
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

    public void StartDialogue(DialogueGraph graph)
    {
        inDialogue = true;
        dialogue = graph;

        graph.Restart();
        ShowDialogue();
    }

    public void SelectAnswer(int i)
    {
        Debug.Log("Answered " + i.ToString());
        dialogue.AnswerQuestion(i);
        ShowDialogue();
    }

    private void ShowDialogue()
    {
        if (dialogue == null) return;

        current = dialogue.current;

        // disable all buttons
        decisionChoice1.transform.parent.gameObject.SetActive(false);
        decisionChoice2.transform.parent.gameObject.SetActive(false);
        decisionChoice3.transform.parent.gameObject.SetActive(false);


        // set name
        nameText.text = current.character.name;

        // open dialogue
        animator.SetBool("IsOpen", true);

        // start coroutine to type
        StopAllCoroutines();
        StartCoroutine(TypeMessage(current.text, typeSpeed));
    }

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

    // displays the message
    IEnumerator TypeMessage(string message, int speed)
    {
        isTyping = true;
        messageText.text = "";

        foreach (char letter in message.ToCharArray())
        {
            for (int i = 0; i < speed; ++i)
                yield return null;

            messageText.text += letter;
        }

        ShowAnswers();

        isTyping = false;
    }

    public void EndDialogue()
    {
        inDialogue = false;
        current = null;
        dialogue = null;
        animator.SetBool("IsOpen", false);
    }
}
