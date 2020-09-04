using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charger : MonoBehaviour
{
    private GameLogic gameLogic;

    public GameObject player; // reference to player
    public Rigidbody2D rb; // the rigidbody to access the physics engine
    public Animator animator; // the animator

    public float CHARGE_SPEED; // how fast the charge goes
    public float CHARGE_TIME; // how long the charge will naturally go
    public float CHARGE_WINDUP; // the windup animation time
    public float CHARGE_STUN; // the amount of time stunned when you miss
    public float CHARGE_TURN_RADIUS; // the turn radius of the charge 
    public float CHARGE_OVERCHARGE_TIME; // how long the charger continues to charge after missing
    public float REBOUND_SPEED; // the speed the charger bounces back after a collision
    public float COLLISION_SLIDE_ANGLE; // the angle that allows the charger to "slide" along the wall instead of stopping

    private int charging;
    private float chargeTimer;
    private Vector3 chargeDir;

    // Start is called before the first frame update
    void Start()
    {
        gameLogic = FindObjectOfType<GameLogic>();
        charging = 0;
        animator.SetInteger("ChargerState", 0);
        chargeTimer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        // skip update if game is in cutscene or dialog
        if (!gameLogic.DoGameplay())
        {
            rb.velocity = Vector2.zero;
            return;
        }

        // update charge timer
        if(charging != 0)
        {
            chargeTimer -= Time.deltaTime;
        }

        if (charging == 0 && CanSeePlayer()) // not charging
        {
            if (CanSeePlayer())
            {
                // get direction of charge
                chargeDir = player.transform.position - transform.position;
                chargeDir.Normalize();

                // start windup and set charge time
                charging = 1;
                animator.SetInteger("ChargerState", 1);
                chargeTimer = CHARGE_WINDUP;
                return;
            }
        }
        else if(charging == 1) // winding up
        {
            if (CanSeePlayer())
            {
                // get direction of charge
                chargeDir = player.transform.position - transform.position;
                chargeDir.Normalize();
            }

            // check timer
            if(chargeTimer <= 0.0f)
            {

                // start the charge and set the timer
                charging = 2;
                animator.SetInteger("ChargerState", 2);
                chargeTimer = CHARGE_TIME;
            }
        }
        else if(charging == 2) // charging
        {
            // check timer
            if(chargeTimer <= 0.0f)
            {
                // end the charge
                charging = 0;
                animator.SetInteger("ChargerState", 0);
            }

            // calculate current direction
            Vector3 curDir = player.transform.position - transform.position;
            curDir.Normalize();

            // if it charged past you then go to "third" stage (brief overcharge and stun)
            if (Vector3.Dot(chargeDir, curDir) < 0.5f)
            {
                charging = 3; // overcharge
                animator.SetInteger("ChargerState", 3);
                chargeTimer = CHARGE_OVERCHARGE_TIME;
                //transform.position = transform.position + chargeDir * Time.deltaTime * CHARGE_SPEED;
                rb.velocity = new Vector2(chargeDir.x, chargeDir.y) * CHARGE_SPEED;
            }

            // if we have LOS to the player than adjust course, otherwise keep going straight
            if (CanSeePlayer())
            {
                // calculate charge (dot) (current rotated 90 deg counterclockwise)
                float dot = chargeDir.y * curDir.x - chargeDir.x * curDir.y;

                if (dot > 0) // need rightwards adjustment
                {
                    chargeDir = Quaternion.Euler(0.0f, 0.0f, -CHARGE_TURN_RADIUS * Time.deltaTime) * chargeDir;
                }
                else if (dot < 0) // need leftwards adjustment
                {
                    chargeDir = Quaternion.Euler(0.0f, 0.0f, CHARGE_TURN_RADIUS * Time.deltaTime) * chargeDir;
                }
            }

            rb.velocity = new Vector2(chargeDir.x, chargeDir.y) * CHARGE_SPEED;
            //transform.position = transform.position + chargeDir * Time.deltaTime * CHARGE_SPEED;
        }
        else if(charging == 3) // overcharge
        {
            if(chargeTimer <= 0.0f)
            {
                charging = 4;
                animator.SetInteger("ChargerState", 4);
                chargeTimer = CHARGE_STUN;
            }

            rb.velocity = new Vector2(chargeDir.x, chargeDir.y) * CHARGE_SPEED;
            //transform.position = transform.position + chargeDir * Time.deltaTime * CHARGE_SPEED;
        }
        else if(charging == 4) // stun
        {
            if(chargeTimer <= 0.0f)
            {
                charging = 0;
                animator.SetInteger("ChargerState", 0);
            }
        }
    }

    /// <summary>
    /// Check if the Charger can see the player
    /// </summary>
    /// <returns>true if a linecast detects the player without colliding with walls</returns>
    bool CanSeePlayer()
    {
        RaycastHit2D[] hits = Physics2D.LinecastAll(transform.position, player.transform.position);

        foreach (RaycastHit2D hit in hits)
        {
            GameObject other = hit.collider.gameObject;
            if (other.CompareTag("Walls")) return false; // walls break los
            if (other.CompareTag("Player")) return true; // can see player
        }

        return false;
    }

    /// <summary>
    /// Handle collisions with walls and player
    /// </summary>
    /// <param name="col">the detected collision</param>
    void OnCollisionEnter2D(Collision2D col)
    {
        // get the other object
        GameObject other = col.gameObject;

        // if it's a wall, get stunned
        if (other.CompareTag("Walls"))
        {
            Vector3 normal = col.contacts[0].normal;
            Vector3 vel = rb.velocity;
            Debug.Log("Angle: " + Vector3.Angle(chargeDir, -normal).ToString());
            if(Vector3.Angle(vel, -normal) > COLLISION_SLIDE_ANGLE)
            {

            } else
            {

                rb.velocity = new Vector2(-chargeDir.x, -chargeDir.y) * REBOUND_SPEED;
                charging = 4;
                chargeTimer = Time.time + CHARGE_STUN;
            }
        }
        // if it's a player, stop for a sec
        if(other.CompareTag("Player"))
        {
            rb.velocity = new Vector2(-chargeDir.x, -chargeDir.y) * CHARGE_SPEED;
            charging = 4;
            chargeTimer = Time.time + CHARGE_STUN;
        }
    }
}
