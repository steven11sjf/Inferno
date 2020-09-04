using System.Collections;
using System.Collections.Generic;
using Dialogue;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLogic : MonoBehaviour
{

    public GameObject player; // the player
    public Camera cam; // the camera
    public GameObject victoryCondition; // the enemy that needs to be dead for the victory condition
    public DialogueManager dialogueManager; // the conversation manager

    private Health playerHealth; // the Health script for the player
    private Health enemyHealth; // the Health script for the enemy, set in Start()
    private bool paused; // true = paused
    private bool playerAlive; // true = player isn't dead
    private bool won; // true = victoryCondition eliminated

    // the state of the game (gameplay, in dialogue, in a cutscene)
    private int state;
    private static int STATE_GAMEPLAY = 0;
    private static int STATE_DIALOGUE = 1;
    private static int STATE_CUTSCENE = 2;

    // Start is called before the first frame update
    void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();

        enemyHealth = victoryCondition.GetComponent<Health>();
        paused = false;
        playerAlive = true;
        won = false;

        state = STATE_GAMEPLAY;
    }

    // Update is called once per frame
    void Update()
    {
        if(state == STATE_GAMEPLAY || state == STATE_DIALOGUE)
        {
            cam.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);
        }

        // check if pause is pressed
        if(Input.GetKeyDown("escape") && playerAlive && !won && state == STATE_GAMEPLAY)
        {
            paused = !paused;

            if (paused)
            {
                PauseAllAnimators();
                Time.timeScale = 0;
            }
            else
            {
                UnpauseAllAnimators();
                Time.timeScale = 1;
            }
        }

        // if paused, no more logic needed
        if (paused) return;

        // check if player is trying to interact
        if (Input.GetKeyDown("e"))
        {
            // if we are in a gameplay state, attempt to interact
            if (state == STATE_GAMEPLAY)
            {
                player.GetComponent<Player>().Interact();
            }
        }

        // handle victory condition
        if(enemyHealth.GetHealth() < 0.0f)
        {
            won = true;
            Time.timeScale = 0;
        }
    }

    /// <summary>
    /// check if game is paused
    /// </summary>
    /// <returns>true if game is paused</returns>
    public bool IsPaused()
    {
        return paused;
    }

    /// <summary>
    /// check if normal gameplay should continue
    /// </summary>
    /// <returns>true if game is in an active state</returns>
    public bool DoGameplay()
    {
        if (state == STATE_GAMEPLAY) return true;
        else return false;
    }

    /// <summary>
    /// returns the state of the game
    /// </summary>
    /// <returns></returns>
    public int GetState()
    {
        return state;
    }

    /// <summary>
    /// Handles the player's death
    /// </summary>
    public void PlayerDeath()
    {
        playerAlive = false;
        Time.timeScale = 0;
        
        // TODO: make menu for retry/menu/whatever
    }

    /// <summary>
    /// Pauses game and starts dialogue based on the DialogueGraph
    /// </summary>
    /// <param name="graph">The dialogue tree to use</param>
    public void StartDialogue(DialogueGraph graph)
    {
        PauseAllAnimators();

        dialogueManager.StartDialogue(graph);
        
        state = STATE_DIALOGUE;
        player.GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, 0.0f); // stop player's velocity
    }

    /// <summary>
    /// Returns to a gameplay state and closes the dialogue bubble
    /// </summary>
    public void EndDialogue()
    {
        UnpauseAllAnimators();

        state = STATE_GAMEPLAY;
        dialogueManager.EndDialogue();
    }

    /// <summary>
    /// Displays victory or defeat conditions
    /// </summary>
    void OnGUI()
    {
        if (won)
        {
            string str = "Victory!";
            GUI.Label(new Rect(500, 500, 650, 20), str);
        }

        if (!playerAlive)
        {
            string str = "Dead!";
            GUI.Label(new Rect(500, 500, 650, 20), str);
        }
    }

    /// <summary>
    /// Pauses all animators in the scene
    /// </summary>
    private void PauseAllAnimators()
    {
        // disable player's animator
        player.GetComponent<Animator>().enabled = false;

        // disable enemies animators
        GameObject enemies = GameObject.FindGameObjectWithTag("EnemyContainer");
        Animator[] enemyAnimators = enemies.GetComponentsInChildren<Animator>();

        foreach (Animator a in enemyAnimators)
        {
            a.enabled = false;
        }
    }

    /// <summary>
    /// Unpauses all animators in the scene
    /// </summary>
    private void UnpauseAllAnimators()
    {
        // enable player's animator
        player.GetComponent<Animator>().enabled = true;

        // enable enemies animators
        GameObject enemies = GameObject.FindGameObjectWithTag("EnemyContainer");
        Animator[] enemyAnimators = enemies.GetComponentsInChildren<Animator>();

        foreach (Animator a in enemyAnimators)
        {
            a.enabled = true;
        }
    }
}
