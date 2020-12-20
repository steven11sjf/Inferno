using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Vector2 m_vel;
    public int m_moveTimer;

    public float d_MovementMultX; // the default movement multiplier along the X axis
    public float d_MovementMultY; // the default movement multiplier along the Y axis
    public float d_DashCooldown = 5.0f; // the default dash cooldown
    public float d_DiveSpeed = 4.0f; // the default 
    public float d_DashTime;

    private GameLogic m_GameLogic;

    // used to change animator state
    public Animator m_Animator;

    // used to track player health
    public Health m_Health;

    // used to shoot and change weapons
    public Guns m_Guns;

    // used to use melee attacks and change weapons
    public MeleeWeapons m_Melees;

    // used to utilise the physics engine
    public Rigidbody2D m_rb;

    // used to set health on HUD
    public Text healthHUD;
    public Text dashHUD;

    public GameObject m_Crosshair; // the crosshair's location
    public GameObject m_BulletPrefab; // prefab for bullets

    public int m_Dashing; // 0 = not dashing, 1 = diving, 2 = rolling
    private float t_DashTime; // the distance remaining in the dash
    private Vector3 m_DashDir; // the direction of the sprint
    private float t_DashCooldownTime; // the time the dash is available

    public float m_MovementMultX;
    public float m_MovementMultY;
    public float m_DashCooldown;
    public float m_DiveSpeed;
    public float m_DashTime;

    // Start is called before the first frame update
    void Start()
    {
        m_GameLogic = FindObjectOfType<GameLogic>();
        m_Dashing = 0;
        t_DashCooldownTime = 0.0f;
        m_vel = Vector2.zero;

        m_MovementMultX = d_MovementMultX;
        m_MovementMultY = d_MovementMultY;
        m_DashCooldown = d_DashCooldown;
        m_DiveSpeed = d_DiveSpeed;
        m_DashTime = d_DashTime;
    }

    // Update is called once per frame
    void Update()
    {
        MoveCrosshair();
        // skip update if game is in cutscene or dialog
        if (!m_GameLogic.DoGameplay()) return;

        // get & set horizontal and vertical movement
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f);
        movement.Normalize();

        if (Dash(movement) != 0) return;

        // shoot if this is the first frame M1 was pressed
        if (Input.GetMouseButton(0))
        {
            m_Guns.Shoot();
        }

        // melee if this is the first frame M1 was pressed
        if (Input.GetMouseButton(1))
        {
            m_Melees.Swing();
        }

        // hot-swap gun if a number was pressed
        if (Input.GetKeyDown("1")) m_Guns.ChangeGun(0);
        else if (Input.GetKeyDown("2")) m_Guns.ChangeGun(1);
        else if (Input.GetKeyDown("3")) m_Guns.ChangeGun(2);

        // set animator variables
        m_Animator.SetFloat("Horizontal", movement.x);
        m_Animator.SetFloat("Vertical", movement.y);
        m_Animator.SetFloat("Magnitude", movement.magnitude);

        m_rb.velocity = new Vector2(movement.x * m_MovementMultX, movement.y * m_MovementMultY);
        if (m_moveTimer > 0)
        {
            m_rb.velocity += m_vel;
            --m_moveTimer;
        }

    }

    private void MoveCrosshair()
    {
        // get point on screen where mouse is
        Vector3 aim = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
        aim = Camera.main.ScreenToWorldPoint(aim);
        Vector3 followXOnly = new Vector3(aim.x, aim.y, transform.position.z);
        m_Crosshair.transform.position = followXOnly;
    }

    /*
     * Starts or continues a dash
     * movement is a normalized vector
     */
    private int Dash(Vector3 movement)
    {
        // if we are starting a dash
        if (m_Dashing == 0 && Input.GetKeyDown(KeyCode.Space) && t_DashCooldownTime < Time.time)
        {

            // start the dash
            m_Dashing = 1;
            m_Animator.SetInteger("DashState", 1);

            // set the direction vector and distance
            m_DashDir = movement;
            t_DashTime = m_DashTime;

            return 0;
        }
        else if (m_Dashing == 1) // if we are mid-dive
        {
            float t = Time.deltaTime; // get time
            if (t_DashTime < t) // if this is the last frame of the dive, dive the remaining distance and roll
            {
                m_rb.velocity = new Vector2(m_DashDir.x, m_DashDir.y) * m_DiveSpeed;
                Debug.Log(m_rb.velocity);
                m_Dashing = 0;
                t_DashTime = d_DashTime;

                m_Animator.SetInteger("DashState", 0);
                t_DashCooldownTime = Time.time + m_DashCooldown; // set the cd timer
            }
            else // if this is not the last frame of the dive, dive according to time passed
            {
                m_rb.velocity = new Vector2(m_DashDir.x, m_DashDir.y) * m_DiveSpeed;
                t_DashTime -= t;
            }

            if (Input.GetMouseButton(0))
            {
                m_Guns.Shoot();
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
            m_Health.Damage(dmg);
        }

        // TODO healthpacks and walls interactions
    }

    public void Interact()
    {
        // get a normalized vector pointing from Daniel to the crosshair
        Vector3 danielToCrosshair = m_Crosshair.transform.position - transform.position;
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
                other.GetComponent<DialogueSceneGraph>().StartDialogue();
                return;
            }
        }
        
    }

    public void Move(Vector2 vec)
    {
        m_vel = vec;
        m_moveTimer = 30;
    }

    void OnGUI()
    {
        string str = "Health: " + m_Health.GetHealth().ToString();
        healthHUD.text = str;

        float dashTime = t_DashCooldownTime - Time.time;
        if (dashTime > 0) dashHUD.text = "Dash: " + dashTime.ToString("0") + "s";
        else dashHUD.text = "Dash: 0s";
    }
}
