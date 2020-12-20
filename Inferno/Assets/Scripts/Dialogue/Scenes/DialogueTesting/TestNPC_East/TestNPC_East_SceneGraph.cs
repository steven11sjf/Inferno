using UnityEngine;

public class TestNPC_East_SceneGraph : DialogueSceneGraph
{
    new void Start()
    {
        base.Start();
        base.dialogueGraph.sceneGraph = this;
    }

    public override void DoCustomCutsceneAction(string action, string[] args)
    {
        switch (action)
        {
            default:
                Debug.Log("Invalid command " + action);
                break;
        }
    }

    public override void UpdateVariables()
    {
        
    }
}
