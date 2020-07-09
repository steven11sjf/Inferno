using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    // used to change animator state
    public Animator animator;

    public GameObject crosshair; // the crosshair's location
    public GameObject bulletPrefab; // prefab for bullets

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // get & set horizontal and vertical movement
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f);

        MoveCrosshair();

        // shoot if this is the first frame M1 was pressed
        if (Input.GetMouseButtonDown(0))
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
    }
}
