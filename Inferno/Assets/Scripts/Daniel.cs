using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Daniel : MonoBehaviour
{
    public float DASH_COOLDOWN = 5.0f;
    public float DIVE_SPEED = 4.0f;
    public float DIVE_TIME;
    public float ROLL_SPEED = 2.0f;
    public float ROLL_TIME;

    private GameLogic gameLogic;

    // used to change animator state
    public Animator animator;

    // used to track player health
    public Health health;

    // used to shoot and change weapons
    public Guns guns;

    // used to use melee attacks and change weapons
    public MeleeWeapons melees;

    // used to utilise the physics engine
    public Rigidbody2D rb;

    public GameObject crosshair; // the crosshair's location
    public GameObject bulletPrefab; // prefab for bullets

    public int dashing; // 0 = not dashing, 1 = diving, 2 = rolling
    private float dashDist; // the distance remaining in the dash
    private Vector3 dashDir; // the direction of the sprint
    private float dashCooldownTime; // the time the dash is available

    // Start is called before the first frame update
    void Start()
    {
        gameLogic = FindObjectOfType<GameLogic>();
        dashing = 0;
        dashCooldownTime = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        // skip update if game is in cutscene or dialog
        if (!gameLogic.DoGameplay()) return;

        // get & set horizontal and vertical movement
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f);
        movement.Normalize();

        MoveCrosshair();
        if (Dash(movement) != 0) return;

        // shoot if this is the first frame M1 was pressed
        if (Input.GetMouseButton(0))
        {
            guns.Shoot();
        }

        // melee if this is the first frame M1 was pressed
        if (Input.GetMouseButton(1))
        {
            melees.Swing();
        }
        // set animator variables
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Magnitude", movement.magnitude);

        rb.velocity = new Vector2(movement.x, movement.y);

    }

    private void MoveCrosshair()
    {
        // get point on screen where mouse is
        Vector3 aim = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
        aim = Camera.main.ScreenToWorldPoint(aim);
        Vector3 followXOnly = new Vector3(aim.x, aim.y, transform.position.z);
        crosshair.transform.position = followXOnly;
    }

    /*
     * Starts or continues a dash
     * movement is a normalized vector
     */
    private int Dash(Vector3 movement)
    {
        // if we are starting a dash
        if (dashing == 0 && Input.GetKeyDown(KeyCode.Space) && dashCooldownTime < Time.time)
        {

            // start the dash
            dashing = 1;
            animator.SetInteger("DashState", 1);

            // set the direction vector and distance
            dashDir = movement;
            dashDist = DIVE_TIME;

            return 0;
        }
        else if (dashing == 1) // if we are mid-dive
        {
            float t = Time.deltaTime; // get time
            if (dashDist < t) // if this is the last frame of the dive, dive the remaining distance and roll
            {
                rb.velocity = new Vector2(dashDir.x, dashDir.y) * DIVE_SPEED;
                Debug.Log(rb.velocity);
                dashing = 2;
                dashDist = ROLL_TIME;

                animator.SetInteger("DashState", 2);
            }
            else // if this is not the last frame of the dive, dive according to time passed
            {
                rb.velocity = new Vector2(dashDir.x, dashDir.y) * DIVE_SPEED;
                dashDist -= t;
            }

            if (Input.GetMouseButton(0))
            {
                guns.Shoot();
            }
            return 1;
        }
        else if (dashing == 2) // if we are recovering
        {
            float t = Time.deltaTime; // get time
            if (dashDist < t) // if this is the last frame of the roll, roll the remaining distance and end dive
            {
                rb.velocity = new Vector2(dashDir.x, dashDir.y) * ROLL_SPEED;

                dashing = 0;
                animator.SetInteger("DashState", 0);
                dashCooldownTime = t + DASH_COOLDOWN; // set the cd timer
            }
            else
            {
                rb.velocity = new Vector2(dashDir.x, dashDir.y) * ROLL_SPEED;
                dashDist -= t;
            }
            return 1;
        }

        return 0; // if we are not dashing
    }

    void OnCollisionEnter2D (Collision2D col)
    {
        // get the other object
        GameObject other = col.gameObject;

        // if it's an enemy take melee damage
        if(other.CompareTag("Enemies"))
        {
            float dmg = other.GetComponent<Health>().GetMeleeDamage();
            health.Damage(dmg);
        }

        // TODO healthpacks and walls interactions
    }

    public void Interact()
    {
        // get a normalized vector pointing from Daniel to the crosshair
        Vector3 danielToCrosshair = crosshair.transform.position - transform.position;
        danielToCrosshair.Normalize();

        // get current position and endpoint of detection raycast
        Vector2 currentPosition = new Vector2(transform.position.x, transform.position.y);
        Vector2 raycastEnd = currentPosition + new Vector2(danielToCrosshair.x, danielToCrosshair.y) * 0.5f;

        // raycast the line and store in hits[]
        RaycastHit2D[] hits = Physics2D.LinecastAll(currentPosition, raycastEnd);

        foreach (RaycastHit2D hit in hits)
        {
            GameObject other = hit.collider.gameObject;
            if(other.CompareTag("NPCs"))
            {
                // trigger dialogue and return
                other.GetComponent<DialogueTrigger>().TriggerDialogue();
                return;
            }
        }
        
    }

    void OnGUI()
    {
        string str = "Current Health: " + health.GetHealth().ToString();
        GUI.Label(new Rect(10, 10, 150, 20), str);
    }
}
