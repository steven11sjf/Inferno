using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{

    public GameObject player; // the player
    public GameObject victoryCondition; // the enemy that needs to be dead for the victory condition
    public DialogueManager dialogueManager; // the dialogue manager

    private Health playerHealth; // the Health script for the player
    private Health enemyHealth; // the Health script for the enemy, set in Start()
    private bool paused; // true = paused
    private bool playerAlive; // true = player isn't dead
    private bool won; // true = victoryCondition eliminated

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
        // check if pause is pressed
        if(Input.GetKeyDown("escape") && playerAlive && !won && state == STATE_GAMEPLAY)
        {
            paused = !paused;

            if (paused)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }

        // if paused, no more logic needed
        if (paused) return;

        if (Input.GetKeyDown("e"))
        {
            if (state == STATE_GAMEPLAY)
            {
                player.GetComponent<Daniel>().Interact();
            }
            else if (state == STATE_DIALOGUE)
            {
                dialogueManager.DisplayNextSentence();
            }
        }

        // handle victory condition
        if(enemyHealth.GetHealth() < 0.0f)
        {
            won = true;
            Time.timeScale = 0;
        }
    }

    // check if game is paused
    public bool isPaused()
    {
        return paused;
    }

    // handle player death
    public void PlayerDeath()
    {
        playerAlive = false;
        Time.timeScale = 0;
        
        // TODO: make menu for retry/menu/whatever
    }

    public void StartDialogue(Dialogue dialogue)
    {
        dialogueManager.StartDialogue(dialogue);
        state = STATE_DIALOGUE;
        Time.timeScale = 0;
    }

    public void EndDialogue()
    {
        state = STATE_GAMEPLAY;
        Time.timeScale = 1;
    }

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
}
