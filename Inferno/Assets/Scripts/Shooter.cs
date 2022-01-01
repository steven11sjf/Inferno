using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    private GameLogic gameLogic;

    public GameObject player;
    public Rigidbody2D rb;
    public Animator animator;

    public GameObject bulletPrefab; // prefab for bullets

    public float BULLET_SPEED; // how fast bullets travel
    public float SHOOTING_DELAY; // delay between shots
    public float MOVEMENT_SPEED; // movement speed
    public float LOSE_LOS_TIME; // how long after losing line-of-sight the shooter returns to neutral behavior
    public float MIN_IDLE_MOVEMENT_TIME; // shortest time between idle movements
    public float MAX_IDLE_MOVEMENT_TIME; // longest time between idle movements
    public float MIN_STRAFE_TURN_TIME; // shortest time between switching directions
    public float MAX_STRAFE_TURN_TIME; // longest time between switching directions
    public float IDLE_MOVEMENT_DURATION; // how long to move

    private int aggression; // 0 if passive, 1 if has had los recently, 2 if has los currently
    private float nextShot; // time when gun can fire next
    private float timer; // used for both changing strafe direction and timing idle movement
    private float lastLOS; // the last time the shooter saw the player
    private float nextIdleMovement; // the next time the shooter will move idly
    private float strafeDir; // the direction of the strafe
    private Vector3 lastLocation; // the last location the shooter saw the player
    private Vector2 idleDir; // the direction of idle movement

    // Start is called before the first frame update
    void Start()
    {
        gameLogic = FindObjectOfType<GameLogic>();

        aggression = 0;
        nextShot = 0.0f;
        lastLOS = 0.0f;
        nextIdleMovement = 0.0f;
        strafeDir = 1.0f;
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

        if (aggression == 2) // is aggroed
        {
            if (CanSeePlayer())
            {
                // get vector pointing to player
                Vector3 shooterToPlayer = player.transform.position - transform.position;
                shooterToPlayer.Normalize();

                // change direction if needed
                if(Time.time > timer)
                {
                    timer = Time.time + Random.Range(MIN_STRAFE_TURN_TIME, MAX_STRAFE_TURN_TIME); // set the time until strafe turns again
                    strafeDir = -strafeDir;
                }

                // run "around" the player
                Vector3 strafeVector = Quaternion.AngleAxis(90, Vector3.forward) * shooterToPlayer * strafeDir;
                rb.velocity = new Vector2(strafeVector.x, strafeVector.y) * MOVEMENT_SPEED;

                // shoot at player
                Shoot();
            }
            else
            {
                // go to level 1 aggro and set the last-seen time and place
                aggression = 1;
                lastLOS = Time.time;
                lastLocation = player.transform.position;
            }
        }
        else if (aggression == 1) // is searching
        {
            if (CanSeePlayer())
            {
                aggression = 2; // go to next aggro level
                timer = Time.time + Random.Range(MIN_STRAFE_TURN_TIME, MAX_STRAFE_TURN_TIME); // set the time until strafe turns again
                if (Random.value > 0.5) // turn around 50% of the time
                    strafeDir = -strafeDir;


                Shoot(); // attempt to shoot at player
            }
            else if (Time.time > lastLOS + LOSE_LOS_TIME)
            {
                // has lost los for too long, return to neutral behavior
                aggression = 0;
                nextIdleMovement = Time.time + Random.Range(MIN_IDLE_MOVEMENT_TIME, MAX_IDLE_MOVEMENT_TIME);
            }
            else
            {
                // keep strafing
                Vector3 shooterToLocation = lastLocation - transform.position;
                shooterToLocation.Normalize();
                Vector3 strafeVector = Quaternion.AngleAxis(90, Vector3.up) * shooterToLocation * strafeDir;
                rb.velocity = new Vector2(strafeVector.x, strafeVector.y) * MOVEMENT_SPEED;
            }
        }
        else // is idle
        {
            if (CanSeePlayer())
            {
                aggression = 2; // go to next aggro level
                if (Random.value > 0.5) // turn around 50% of the time
                    strafeDir = -strafeDir;

                Shoot(); // attempt to shoot at player
            }
            else
            {
                if (Time.time > nextIdleMovement) // if we are overdue for idle movement
                {
                    // get a random vector
                    Vector2 idleMovementVec = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
                    idleMovementVec.Normalize();

                    // travel in that direction
                    idleDir = idleMovementVec * MOVEMENT_SPEED;
                    rb.velocity = idleDir;

                    // set the timers
                    timer = Time.time + IDLE_MOVEMENT_DURATION;
                    nextIdleMovement = timer + Random.Range(MIN_IDLE_MOVEMENT_TIME, MAX_IDLE_MOVEMENT_TIME);
                }
                else if (Time.time < timer) // if we still are moving idly
                {
                    rb.velocity = idleDir;
                }
                else // if we aren't moving set velocity to 0
                    rb.velocity = Vector2.zero;
            }
        }

        animator.SetFloat("XAxis", rb.velocity.x);
    }

    /// <summary>
    /// Checks if the shooter can see the player
    /// </summary>
    /// <returns>True if a raycast detects a player before any walls</returns>
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
    /// Attempts to shoot at the player
    /// </summary>
    void Shoot()
    {
        if(Time.time > nextShot)
        {
            animator.SetBool("IsShooting", true);

            // shoot
            // get a normalized vector pointing from shooter to Daniel
            Vector3 shot = player.transform.position - transform.position;
            Debug.Log(shot);
            shot.Normalize();
            Debug.Log(shot);

            // instantiate a new bullet
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

            // fire bullet in the direction of daniel
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.velocity = new Vector2(shot.x, shot.y) * BULLET_SPEED;
            Debug.Log(bulletScript.velocity);
            bulletScript.instantiator = gameObject;
            bulletScript.damage = 7.5f;

            // set delay for next shot
            nextShot = Time.time + SHOOTING_DELAY;
        }
        else
        {
            animator.SetBool("IsShooting", false);
        }
    }

    /// <summary>
    /// Handles a collision with the player
    /// </summary>
    /// <param name="col">The collision2D with the player</param>
    void OnCollisionEnter2D(Collision2D col)
    {
        GameObject other = col.gameObject;

        if(other.CompareTag("Player"))
        {
            // bounce shooter back
            Vector3 playerToShooter = transform.position - player.transform.position;
            rb.velocity = new Vector2(playerToShooter.x, playerToShooter.y);
        }
    }
}
