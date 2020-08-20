using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private GameLogic gameLogic;

    public Vector2 velocity = new Vector2(0.0f, 0.0f);
    public GameObject instantiator;
    public float damage;

    // the bullet's time-to-live, manually tracked instead of using Destroy(bullet, 3.0f) so it can survive dialogue and cutscenes
    private float ttl;

    // Start is called before the first frame update
    void Start()
    {
        gameLogic = FindObjectOfType<GameLogic>();
        ttl = 3.0f;
    }

    // Update is called once per frame
    void Update()
    {
        // skip update if game is in cutscene or dialog
        if (!gameLogic.DoGameplay()) return;

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

        // handle time-to-live
        ttl -= Time.deltaTime;
        if (ttl < 0.0f) Destroy(gameObject);
    }
}
