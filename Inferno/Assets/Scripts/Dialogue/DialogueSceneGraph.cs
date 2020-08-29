using Dialogue;
using UnityEngine;
using XNode;

public class DialogueSceneGraph : MonoBehaviour {
	[SerializeField]
	public DialogueGraph dialogueGraph;

    private GameLogic gameLogic;

    private void Start()
    {
        gameLogic = FindObjectOfType<GameLogic>();
    }

    public void StartDialogue()
    {
        gameLogic.StartDialogue(dialogueGraph);
    }
}