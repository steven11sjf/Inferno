using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConversationNode
{
    // unique node ID for this conversation
    public int id;

    // who is speaking in this node
    public string name;

    // the dialogue message
    [TextArea(3,10)]
    public string message;

    // whether the question is a decision
    public bool isDecision;

    // list of responses
    public string[] responses;

    public int[] nextResponse;
}
