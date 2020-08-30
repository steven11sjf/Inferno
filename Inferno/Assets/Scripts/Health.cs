using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    // the initial health level
    public float maxHealth;

    // the gamelogic object
    public GameLogic gameLogic;
    // the unit's melee damage
    public float meleeDamage;

    // the current health
    private float currHealth;

    // Start is called before the first frame update
    void Start()
    {
        currHealth = maxHealth;
    }

    // Returns the unit's melee damage
    public float GetMeleeDamage()
    {
        return meleeDamage;
    }

    // Returns the unit's current health
    public float GetHealth()
    {
        return currHealth;
    }

    ///<summary>
    ///Decreases the object's health
    ///</summary>
    public void Damage(float dmg)
    {
        Debug.Log("Damaged " + dmg);
        if (dmg > 0) currHealth -= dmg;

        if (currHealth < 0.0f) Die();
    }

    // increases the object's health
    public void Heal(float health)
    {
        if (health > 0) currHealth += health;

        if (currHealth > maxHealth) currHealth = maxHealth;
    }

    // Kills the object
    void Die()
    {
        if (gameObject.CompareTag("Player"))
        {
            gameLogic.PlayerDeath();
        }
        else
        {
            Debug.Log("Dead!");
            gameObject.SetActive(false);
        }
    }
}
