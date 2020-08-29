using Dialogue;
using UnityEngine;
using XNode;

public abstract class DialogueSceneGraph : MonoBehaviour {
	[SerializeField]
	public DialogueGraph dialogueGraph;

    public GameLogic gameLogic;

    public void Start()
    {
        gameLogic = FindObjectOfType<GameLogic>();
    }

    public void StartDialogue()
    {
        gameLogic.StartDialogue(dialogueGraph);
    }

    public abstract void DoCutsceneAction(string action, string[] args);
}