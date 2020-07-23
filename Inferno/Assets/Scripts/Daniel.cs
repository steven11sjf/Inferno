using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Daniel : MonoBehaviour
{
    public float GUN_COOLDOWN = 0.5f;
    public float BULLET_SPEED = 5.0f;
    public float DASH_COOLDOWN = 5.0f;
    public float DASH_SPEED = 3.0f;
    public float DASH_TIME;

    // used to change animator state
    public Animator animator;

    // used to track player health
    public Health health;

    // used to utilise the physics engine
    public Rigidbody2D rb;

    public GameObject crosshair; // the crosshair's location
    public GameObject bulletPrefab; // prefab for bullets

    private bool dashing; // true if player is dashing
    private float dashDist; // the distance remaining in the dash
    private Vector3 dashDir; // the direction of the sprint

    private float gunCooldownTime; // the time the gun can fire next
    private float dashCooldownTime; // the time the dash is available

    // Start is called before the first frame update
    void Start()
    {
        dashing = false;
        gunCooldownTime = 0.0f;
        dashCooldownTime = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
        // get & set horizontal and vertical movement
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f);
        movement.Normalize();

        MoveCrosshair();
        if (Dash(movement) != 0) return;

        // shoot if this is the first frame M1 was pressed and gun is off CD
        if (Input.GetMouseButton(0) && gunCooldownTime < Time.time)
        {
            Shoot();
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

    private void Shoot()
    {
        // get a normalized vector pointing from Daniel to the crosshair
        Vector3 danielToCrosshair = crosshair.transform.position - transform.position;
        danielToCrosshair.Normalize();

        // instantiate a new bullet from the center of daniel
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

        // fire bullet in the direction of the crosshair
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.velocity = new Vector2(danielToCrosshair.x, danielToCrosshair.y) * BULLET_SPEED;
        bulletScript.instantiator = gameObject;
        bulletScript.damage = 5.0f;
        
        // destroy bullet 3 seconds after firing
        Destroy(bullet, 3.0f);

        // set cooldown time
        gunCooldownTime = Time.time + GUN_COOLDOWN;
    }

    /*
     * Starts or continues a dash
     * movement is a normalized vector
     */
    private int Dash(Vector3 movement)
    {
        // if we are starting a dash
        if (!dashing && Input.GetKeyDown(KeyCode.Space) && dashCooldownTime < Time.time)
        {

            // start the dash
            dashing = true;
            animator.SetBool("Dashing", true);

            // set the direction vector and distance
            dashDir = movement;
            dashDist = DASH_TIME;

            return 1;
        }
        else if (dashing) // if we are mid-dash
        {
            float t = Time.deltaTime; // get time
            if (dashDist < t) // if this is the last frame of the dash, dash the remaining distance and end the dash
            {
                transform.position = transform.position + dashDir * dashDist * DASH_SPEED;
                dashing = false;
                animator.SetBool("Dashing", false);
                dashCooldownTime = Time.time + DASH_COOLDOWN; // set the cd timer
            }
            else // if this is not the last frame of the dash, dash according to time passed
            {
                transform.position = transform.position + dashDir * t * DASH_SPEED;
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

    void OnGUI()
    {
        string str = "Current Health: " + health.GetHealth().ToString();
        GUI.Label(new Rect(10, 10, 150, 20), str);
    }
}
