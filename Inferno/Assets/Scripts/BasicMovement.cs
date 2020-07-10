﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    private const float GUN_COOLDOWN = 0.5f;
    private const float DASH_COOLDOWN = 5.0f;

    // used to change animator state
    public Animator animator;

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

        // update Daniel's position
        transform.position = transform.position + movement * Time.deltaTime;

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
        GameObject bullet = Instantiate(bulletPrefab, transform.position + new Vector3(0.15f, -0.15f, 0.0f), Quaternion.identity);

        // fire bullet in the direction of the crosshair
        bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(danielToCrosshair.x, danielToCrosshair.y) * 5.0f;
        
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
            dashDist = 0.3f;

            return 1;
        }
        else if (dashing) // if we are mid-dash
        {
            float t = Time.deltaTime; // get time
            if (dashDist < t) // if this is the last frame of the dash, dash the remaining distance and end the dash
            {
                transform.position = transform.position + dashDir * dashDist;
                dashing = false;
                animator.SetBool("Dashing", false);
                dashCooldownTime = Time.time + DASH_COOLDOWN; // set the cd timer
            }
            else // if this is not the last frame of the dash, dash according to time passed
            {
                transform.position = transform.position + dashDir * t * 3.0f;
                dashDist -= t;
            }
            return 1;
        }

        return 0; // if we are not dashing
    }
}
