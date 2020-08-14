using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public Rigidbody2D rb;
    public GameObject player;

    public float length;
    public float speed;
    public float damage;
    public float swingLength;

    private float lengthRemaining;
    private float init;

    // Start is called before the first frame update
    void Start()
    {
        init = rb.rotation;
        lengthRemaining = swingLength;
        transform.position = player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //float angle = Mathf.LerpAngle(init, swingLength, speed * Time.deltaTime);
        //rb.MoveRotation(angle);

        //if (angle == swingLength)
        //{
        //    init = rb.rotation;
        //    swingLength += 100;
        //}

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
