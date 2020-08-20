using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conversation : MonoBehaviour
{
    // array containing all nodes
    public ConversationNode[] nodes;

    public GameLogic gameLogic;

    public List<int> nodesUsed;

    void Start()
    {
        gameLogic = FindObjectOfType<GameLogic>();
        nodesUsed = new List<int>();
    }

    public void StartConversation()
    {
        // add the first node to the list
        nodesUsed.Add(0);

        // start from first node
        gameLogic.StartConversation(this, nodes[0]);
    }

    // accepts a response from ConversationManager, and responds if appropriate
    public void Respond(int id)
    {
        if (id != -1)
        {
            nodesUsed.Add(id);
            gameLogic.StartConversation(this, nodes[id]);
        }
        else EndConversation();
    }

    // called when a conversation ends
    void EndConversation()
    {
        Debug.Log("Ended!");
        gameLogic.EndConversation(); // returns to gameplay state
    }
}
