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
    public float currHealth;

    // Start is called before the first frame update
    void Start()
    {
        currHealth = maxHealth;
    }

    /// <summary>
    /// Gets the unit's melee (collision) damage
    /// </summary>
    /// <returns></returns>
    public float GetMeleeDamage()
    {
        return meleeDamage;
    }

    /// <summary>
    /// Gets the unit's current health
    /// </summary>
    /// <returns></returns>
    public float GetHealth()
    {
        return currHealth;
    }

    /// <summary>
    /// Deals damage to the unit
    /// </summary>
    /// <param name="dmg">The amount of damage to deal</param>
    public void Damage(float dmg)
    {
        Debug.Log("Damaged " + dmg);
        if (dmg > 0) currHealth -= dmg;

        if (currHealth < 0.0f) Die();
    }

    /// <summary>
    /// Raises the unit's health, capped at maxHealth
    /// </summary>
    /// <param name="health">how much health to give</param>
    public void Heal(float health)
    {
        if (health > 0) currHealth += health;

        if (currHealth > maxHealth) currHealth = maxHealth;
    }

    /// <summary>
    /// Kills the object
    /// </summary>
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
