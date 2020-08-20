using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Text nameText; // the name text-box on the canvas
    public Text dialogueText; // the dialogue text box

    public Animator animator; // the animator for the dialogue box

    private Queue<string> sentences; // the current queue of sentences

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue (Dialogue dialogue)
    {

        animator.SetBool("IsOpen", true); // open the dialogue
        
        // set name
        nameText.text = dialogue.name;

        sentences.Clear(); // clear old dialogue

        // fill the queue with given dialogue
        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        // display the first dialogue window
        DisplayNextSentence();
    }

    // displays the next dialogue in the queue
    public void DisplayNextSentence()
    {
        // if there is no dialogue left finish the conversation and return to gameplay
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        // get the next sentence
        string sentence = sentences.Dequeue();
        
        // set the dialogue text to sentence
        dialogueText.text = sentence;
    }

    // ends conversation, empties text boxes and calls GameLogic.EndDialogue()
    void EndDialogue()
    {
        animator.SetBool("IsOpen", false);
        FindObjectOfType<GameLogic>().EndDialogue();
        nameText.text = "";
        dialogueText.text = "";
    }
}
