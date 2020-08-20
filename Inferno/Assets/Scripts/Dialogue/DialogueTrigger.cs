using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    // the dialogue to be displayed when the trigger is set
    public Dialogue dialogue;

    // called in Daniel.Interact
    public void TriggerDialogue ()
    {
        FindObjectOfType<GameLogic>().StartDialogue(dialogue);
    }
}
