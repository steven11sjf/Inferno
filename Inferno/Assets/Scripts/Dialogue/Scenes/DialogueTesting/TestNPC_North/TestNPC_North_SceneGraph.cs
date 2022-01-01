using UnityEngine;

public class TestNPC_North_SceneGraph : DialogueSceneGraph
{
    public Player player;

    new void Start()
    {
        player = FindObjectOfType<Player>();
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
