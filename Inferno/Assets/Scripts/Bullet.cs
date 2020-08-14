using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public Vector2 velocity = new Vector2(0.0f, 0.0f);
    public GameObject instantiator;
    public float damage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // calculate current and new position
        Vector2 currentPosition = new Vector2(transform.position.x, transform.position.y);
        Vector2 newPosition = currentPosition + velocity * Time.deltaTime;

        // check what the arrow has hit this frame
        RaycastHit2D[] hits = Physics2D.LinecastAll(currentPosition, newPosition);

        foreach (RaycastHit2D hit in hits)
        {
            GameObject other = hit.collider.gameObject;
            if (other != instantiator)
            {
                if (other.CompareTag("Walls"))
                {
                    Destroy(gameObject);
                    break;
                }

                if (other.CompareTag("Enemies"))
                {
                    other.GetComponent<Health>().Damage(damage);
                    Destroy(gameObject);
                    break;
                }

                if (other.CompareTag("Player"))
                {
                    other.GetComponent<Health>().Damage(damage);
                    Destroy(gameObject);
                    break;
                }
            }
        }

        transform.position = newPosition;
    }
}
