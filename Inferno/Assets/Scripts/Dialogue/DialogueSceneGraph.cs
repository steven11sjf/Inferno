using System.Collections;
using System.Collections.Generic;
using Dialogue;
using UnityEngine;
using XNode;

public abstract class DialogueSceneGraph : MonoBehaviour {
	[SerializeField]
	public DialogueGraph dialogueGraph;
    public IDictionary<string, bool> variables = new Dictionary<string, bool>();

    public GameLogic gameLogic;

    public void Start()
    {
        gameLogic = FindObjectOfType<GameLogic>();
    }

    /// <summary>
    /// Starts the dialogue tree
    /// </summary>
    public void StartDialogue()
    {
        gameLogic.StartDialogue(dialogueGraph);
    }

    /// <summary>
    /// Starts a cutscene action
    /// </summary>
    /// <param name="action">The name of the action</param>
    /// <param name="args">The parameters for the action, if any</param>
    public void DoCutsceneAction(string action, string[] args)
    {
        switch (action)
        {
            case "EndDialogue":
                gameLogic.EndDialogue();
                break;

            default:
                DoCustomCutsceneAction(action, args);
                break;
        }
    }

    /// <summary>
    /// Starts a cutscene action that is custom to the scene
    /// </summary>
    /// <param name="action">The name of the action</param>
    /// <param name="args">The parameters for the action, if any</param>
    public abstract void DoCustomCutsceneAction(string action, string[] args);

    /// <summary>
    /// Updates a variable used in the dialogue graph
    /// </summary>
    /// <param name="var">the variable to be changed</param>
    /// <param name="state">whether it is true or false</param>
    public void UpdateVariable(string var, bool state)
    {
        if(variables.ContainsKey(var))
        {
            variables[var] = state;
        }
        else
        {
            variables.Add(var, state);
        }
    }

    /// <summary>
    /// Finds the value of the key
    /// </summary>
    /// <param name="key">The key to check</param>
    /// <param name="result">The value of the key, or false if it does not exist</param>
    /// <returns>true if the key exists</returns>
    public bool CheckVariable(string key, out bool result)
    {
        if (variables.ContainsKey(key))
        {
            result = variables[key];
            return true;
        }
        else
        {
            Debug.Log("Key " + key + "not in dictionary");
            result = false;
            return false;
        }
    }

    /// <summary>
    /// Updates all variables
    /// </summary>
    public abstract void UpdateVariables();
}