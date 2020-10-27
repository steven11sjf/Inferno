using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipAbilities : MonoBehaviour
{
    public Player player;

    public int numAbilities = 3;
    public Ability[] abilities;

    public Ability ability1;
    public Ability ability2;
    public Ability ability3;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();

        abilities = new Ability[numAbilities];
        abilities[0] = ability1;
        abilities[1] = ability2;
        abilities[2] = ability3;
    }

    /*
     * Sets the ability effects. Should be called on scene load for levels, but for testing purposes it's bound to interact.
     */
    public void SetAbilities()
    {
        ResetToDefaultParameters();

        foreach(Ability a in abilities)
        {
            if (a == null) return;
            Debug.Log("Setting ability " + a.GetId());

            AbilityEffect[] ae = a.GetEffects();
            foreach (AbilityEffect effect in ae)
            {
                Debug.Log("Setting ability effect for " + effect.idNumber);
                SetAbilityEffect(effect);
            }
        }
    }

    /*
     * Resets all variables to their default. Called by SetAbilities to make sure effects stack properly.
     */
    private void ResetToDefaultParameters()
    {
        player.x_movement_mult = player.DEFAULT_X_MOVEMENT_MULT;
        player.y_movement_mult = player.DEFAULT_Y_MOVEMENT_MULT;

        player.dash_cooldown = player.DEFAULT_DASH_COOLDOWN;
        player.dive_speed = player.DEFAULT_DIVE_SPEED;
        player.dive_time = player.DEFAULT_DIVE_TIME;

        player.guns.ResetToDefault();
    }

    /*
     * Sets the ability effects for an ability effect. Called by SetAbilities().
     */
    void SetAbilityEffect(AbilityEffect effect)
    {
        float[] param = new float[10];
        switch(effect.idNumber)
        {
            // Y axis movement
            case 1001:
                player.y_movement_mult *= effect.effects["movementSpeed"];
                break;

            // X axis movement
            case 1002:
                player.x_movement_mult *= effect.effects["movementSpeed"];
                break;

            // dash
            case 1003:
                player.dash_cooldown *= effect.effects["dashCooldown"];
                player.dive_speed *= effect.effects["dashSpeed"];
                player.dive_time *= effect.effects["dashTime"];
                break;

            // sword swing
            case 2001:

                break;

            // gun settings
            case 3001:
                player.guns.swap_time *= effect.effects["gunSwapTime"];
                break;

            // pistol
            case 3002:
                player.guns.ChangeGunProperty(0, "damage", effect.effects["pistolDmg"]);
                player.guns.ChangeGunProperty(0, "fireRate", effect.effects["pistolFireRate"]);
                player.guns.ChangeGunProperty(0, "spread", effect.effects["pistolSpread"]);
                player.guns.ChangeGunProperty(0, "speed", effect.effects["pistolBulletSpeed"]);
                break;

            default:
                Debug.Log("Invalid ID Number: " + effect.idNumber);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
