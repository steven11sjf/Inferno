using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CutsceneActions : MonoBehaviour
{
    // the player's rigidbody, used for movement
    public Rigidbody2D rb;
    public Vector2 m_velocity;

    // Start is called before the first frame update
    void Start()
    {
        m_velocity = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = m_velocity;
    }

    public void CompleteAction(string action, float vars)
    {
        GameLogic gl = FindObjectOfType<GameLogic>();
        Player p = FindObjectOfType<Player>();

        switch(action)
        {
            case "PlayerMoveDown":
                Debug.Log("Move Down");
                gl.EndDialogue();
                p.Move(Vector2.down);
                Debug.Log(m_velocity);
                break;
            /*case "PlayerMoveLeft":
                StartCoroutine(Move(Vector2.left, vars));
                break;
            case "PlayerMoveUp":
                StartCoroutine(Move(Vector2.up, vars));
                break;
            case "PlayerMoveRight":
                StartCoroutine(Move(Vector2.right, vars));
                break;*/
            default:
                Debug.Log("Action not found!");
                break;
        }
    }

    /// <summary>
    /// Moves the player in the Vector2's direction for time
    /// </summary>
    /// <param name="dir">The direction</param>
    /// <param name="time">How long to travel</param>
    /// <returns></returns>
    IEnumerator Move(Vector2 dir, float time)
    {
        for (float t = time; t > 0;)
        {
            rb.velocity = dir;
            yield return null;
        }

        rb.velocity = Vector3.zero;
    }
}
