using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityScript.Steps;

public class TestNPC_North_SceneGraph : DialogueSceneGraph
{
    public Player player;

    new void Start()
    {
        player = FindObjectOfType<Player>();
        base.Start();

        base.dialogueGraph.sceneGraph = this;
    }

    public override void DoCutsceneAction(string action, string[] args)
    {
        switch (action)
        {
            case "EndDialogue":
                base.gameLogic.EndDialogue();
                break;
            default:
                Debug.Log("Invalid command " + action);
                break;
        }
    }

    public override void UpdateVariables()
    {
        
    }
}
