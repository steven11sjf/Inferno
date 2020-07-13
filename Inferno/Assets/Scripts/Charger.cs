using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charger : MonoBehaviour
{

    public GameObject player;

    public float CHARGE_SPEED; // how fast the charge goes
    public float CHARGE_TIME; // how long the charge will naturally go
    public float CHARGE_WINDUP; // the windup animation time
    public float CHARGE_STUN; // the amount of time stunned when you miss
    public float CHARGE_TURN_RADIUS; // the turn radius of the charge 
    public float CHARGE_OVERCHARGE_TIME; // how long the charger continues to charge after missing

    private int charging;
    private float chargeTimer;
    private Vector3 chargeDir;

    // Start is called before the first frame update
    void Start()
    {
        charging = 0;
        chargeTimer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {

        if(charging == 0) // not charging
        {
            // raycast to check if player is in line of sight
            RaycastHit2D[] hits = Physics2D.LinecastAll(transform.position, player.transform.position);

            foreach (RaycastHit2D hit in hits)
            {
                GameObject other = hit.collider.gameObject;
                if (other.CompareTag("Walls")) break; // walls break los
                if(other.CompareTag("Player"))
                {
                    // start windup and set charge time
                    charging = 1;
                    chargeTimer = Time.time + CHARGE_WINDUP;
                    break;
                }
            }
        }
        else if(charging == 1) // winding up
        {
            // check timer
            if(Time.time > chargeTimer)
            {
                // get direction of charge
                chargeDir = player.transform.position - transform.position;
                chargeDir.Normalize();

                // start the charge and set the timer
                charging = 2;
                chargeTimer = Time.time + CHARGE_TIME;
            }
        }
        else if(charging == 2) // charging
        {
            // check timer
            if(Time.time > chargeTimer)
            {
                // end the charge
                charging = 0;
            }

            // calculate current direction
            Vector3 curDir = player.transform.position - transform.position;
            curDir.Normalize();

            // if it charged past you then go to "third" stage (brief overcharge and stun)
            if (Vector3.Dot(chargeDir, curDir) < 0.5f)
            {
                charging = 3; // overcharge
                chargeTimer = Time.time + CHARGE_OVERCHARGE_TIME;
                transform.position = transform.position + chargeDir * Time.deltaTime * CHARGE_SPEED;
            }

            // calculate charge (dot) (current rotated 90 deg counterclockwise)
            float dot = chargeDir.y * curDir.x - chargeDir.x * curDir.y;
            
            if(dot > 0) // need rightwards adjustment
            {
                chargeDir = Quaternion.Euler(0.0f, 0.0f, -CHARGE_TURN_RADIUS * Time.deltaTime) * chargeDir;
            }
            else if(dot < 0) // need leftwards adjustment
            {
                chargeDir = Quaternion.Euler(0.0f, 0.0f, CHARGE_TURN_RADIUS * Time.deltaTime) * chargeDir;
            }

            transform.position = transform.position + chargeDir * Time.deltaTime * CHARGE_SPEED;
        }
        else if(charging == 3) // overcharge
        {
            if(chargeTimer < Time.time)
            {
                charging = 4;
                chargeTimer = Time.time + CHARGE_STUN;
            }

            transform.position = transform.position + chargeDir * Time.deltaTime * CHARGE_SPEED;
        }
        else if(charging == 4) // stun
        {
            if(chargeTimer < Time.time)
            {
                charging = 0;
            }
        }
    }
}
