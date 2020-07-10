using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerMovementAttacking : MonoBehaviour
{

    public GameObject player;

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
            // start windup and set charge time
            charging = 1;
            chargeTimer = Time.time + 1.0f;
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
                chargeTimer = Time.time + 2.0f;
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
                chargeTimer = Time.time + 0.25f;
                transform.position = transform.position + chargeDir * Time.deltaTime * 1.5f;
            }

            // calculate charge (dot) (current rotated 90 deg counterclockwise)
            float dot = chargeDir.y * curDir.x - chargeDir.x * curDir.y;
            
            if(dot > 0) // need rightwards adjustment
            {
                chargeDir = Quaternion.Euler(0.0f, 0.0f, -0.25f) * chargeDir;
            }
            else if(dot < 0) // need leftwards adjustment
            {
                chargeDir = Quaternion.Euler(0.0f, 0.0f, 0.25f) * chargeDir;
            }

            transform.position = transform.position + chargeDir * Time.deltaTime * 2.0f;
        }
        else if(charging == 3) // overcharge
        {
            if(chargeTimer < Time.time)
            {
                charging = 4;
                chargeTimer = Time.time + 1.0f;
            }

            transform.position = transform.position + chargeDir * Time.deltaTime * 2.0f;
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
