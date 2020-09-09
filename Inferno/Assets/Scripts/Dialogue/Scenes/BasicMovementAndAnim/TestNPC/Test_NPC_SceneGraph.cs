using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityScript.Steps;

public class Test_NPC_SceneGraph : DialogueSceneGraph
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
        switch(action)
        {
            case "PlayerMoveUp":
                Debug.Log("Moving up!");
                float magnitude;
                if (float.TryParse(args[0], out magnitude))
                    player.Move(Vector2.up * magnitude);
                else player.Move(Vector2.up);
                break;
            case "EndDialogue":
                base.gameLogic.EndDialogue();
                break;
            case "PlayerHeal":
                float amount;
                if (float.TryParse(args[0], out amount))
                    player.GetComponent<Health>().Heal(amount);
                else
                    Debug.Log("PlayerHeal: args[0] not set!");
                break;
            default:
                Debug.Log("Invalid command " + action);
                break;
        }
    }

    public override void UpdateVariables()
    {
        // PlayerHealthOver20
        UpdateVariable("PlayerHealthOver20", player.GetComponent<Health>().GetHealth() > 20.0f);
    }
}
