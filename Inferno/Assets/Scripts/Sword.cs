using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public Rigidbody2D rb;
    public GameObject player;

    public float length = 3.0f;
    public float speed = -1440.0f;
    public float damage = 20.0f;
    public float swingLength = 180.0f;

    private float lengthRemaining;
    public float initAngle;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(initAngle);
        //rb.rotation = initAngle;
        lengthRemaining = swingLength;
        transform.position = player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        rb.rotation += Time.deltaTime * speed;
        transform.position = player.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D hit)
    {
        GameObject other = hit.gameObject;
        Debug.Log("Collided");

        if (other == player) return; // ignore player collisions

        // check if hit wall
        if (other.CompareTag("Walls"))
        {
            // end swing
            Debug.Log("Sword hit wall!");
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Enemies"))
        {
            // damage enemy
            other.GetComponent<Health>().Damage(damage);

            // bounce back
            Vector2 swordAngle = new Vector2(Mathf.Sin(rb.rotation), Mathf.Cos(rb.rotation));

            Vector2 relVelocity = hit.attachedRigidbody.velocity; // opposite of enemy velocity

            Debug.Log(swordAngle);
            Debug.Log(relVelocity);
            hit.GetComponent<Rigidbody2D>().velocity = (swordAngle + relVelocity) * 5.0f;
        }
    }
}
