using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConversationManager : MonoBehaviour
{
    public Text nameText;
    public Text messageText;
    public Text decisionChoice1, decisionChoice2, decisionChoice3;
    public Animator animator;

    public int typeSpeed; // typing machine go brrrrrrrr

    private Conversation curr; // the current conversation
    private ConversationNode node; // the current node

    // Start is called before the first frame update
    void Start()
    {
        // there is no current node
        node = null;

        // disable all buttons
        decisionChoice1.transform.parent.gameObject.SetActive(false);
        decisionChoice2.transform.parent.gameObject.SetActive(false);
        decisionChoice3.transform.parent.gameObject.SetActive(false);
    }

    void Update()
    {
        if (node != null && Input.GetKeyDown("e") && !node.isDecision)
        {
            Debug.Log("UPDATE");
            curr.Respond(node.nextResponse[0]);
        }
    }

    // carries out one ConversationNode
    public void Converse(Conversation conv, ConversationNode node)
    {
        curr = conv;
        // set node to conv
        this.node = node;

        // disable all buttons
        decisionChoice1.transform.parent.gameObject.SetActive(false);
        decisionChoice2.transform.parent.gameObject.SetActive(false);
        decisionChoice3.transform.parent.gameObject.SetActive(false);

        // set name
        nameText.text = node.name;

        // open dialogue
        animator.SetBool("IsOpen", true);

        // start coroutine
        StopAllCoroutines();
        StartCoroutine(TypeMessage(node.message, typeSpeed));
    }

    // displays the message
    IEnumerator TypeMessage (string message, int speed)
    {
        messageText.text = "";

        foreach (char letter in message.ToCharArray())
        {
            for (int i = 0; i < speed; ++i)
                yield return null;

            messageText.text += letter;
        }

        // determine if message or decision
        if (node.isDecision)
        {
            decisionChoice1.text = node.responses[0];
            decisionChoice1.transform.parent.gameObject.SetActive(true);

            decisionChoice2.text = node.responses[1];
            decisionChoice2.transform.parent.gameObject.SetActive(true);

            if(node.responses.Length == 3)
            {
                decisionChoice3.text = node.responses[2];
                decisionChoice3.transform.parent.gameObject.SetActive(true);
            }
        }
        else
        {

        }
    }

    public void Response1()
    {
        animator.SetBool("IsOpen", false);
        curr.Respond(node.nextResponse[0]);
        Debug.Log("Response1");
    }

    public void Response2()
    {
        animator.SetBool("IsOpen", false);
        curr.Respond(node.nextResponse[1]);
    }

    public void Response3()
    {
        animator.SetBool("IsOpen", false);
        curr.Respond(node.nextResponse[2]);
    }
}
