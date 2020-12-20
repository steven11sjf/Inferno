using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipAbilities : MonoBehaviour
{
    public Player player;
    public Text abilityHUD;

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
    }

    /*
     * Sets the ability effects. Should be called on scene load for levels, but for testing purposes it's bound to interact.
     */
    public void SetAbilities()
    {
        ResetToDefaultParameters();

        foreach (Ability a in abilities)
        {
            if (a == null) continue;
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
        player.m_MovementMultX = player.d_MovementMultX;
        player.m_MovementMultY = player.d_MovementMultY;

        player.m_DashCooldown = player.d_DashCooldown;
        player.m_DiveSpeed = player.d_DiveSpeed;
        player.m_DashTime = player.d_DashTime;

        player.m_Guns.ResetToDefault();
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
                player.m_MovementMultY *= effect.effects["movementSpeed"];
                break;

            // X axis movement
            case 1002:
                player.m_MovementMultX *= effect.effects["movementSpeed"];
                break;

            // dash
            case 1003:
                player.m_DashCooldown *= effect.effects["dashCooldown"];
                player.m_DiveSpeed *= effect.effects["dashSpeed"];
                player.m_DashTime *= effect.effects["dashTime"];
                break;

            // sword swing
            case 2001:

                break;

            // gun settings
            case 3001:
                player.m_Guns.swap_time *= effect.effects["gunSwapTime"];
                break;

            // pistol
            case 3002:
                player.m_Guns.ChangeGunProperty(0, "damage", effect.effects["pistolDmg"]);
                player.m_Guns.ChangeGunProperty(0, "fireRate", effect.effects["pistolFireRate"]);
                player.m_Guns.ChangeGunProperty(0, "spread", effect.effects["pistolSpread"]);
                player.m_Guns.ChangeGunProperty(0, "speed", effect.effects["pistolBulletSpeed"]);
                break;

            default:
                Debug.Log("Invalid ID Number: " + effect.idNumber);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("z"))
        {
            if (abilities[0]) abilities[0] = null;
            else abilities[0] = ability1;
            SetAbilities();
        }

        if (Input.GetKeyDown("x"))
        {
            if (abilities[1]) abilities[1] = null;
            else abilities[1] = ability2;
            SetAbilities();
        }

        if (Input.GetKeyDown("c"))
        {
            if (abilities[2]) abilities[2] = null;
            else abilities[2] = ability3;
            SetAbilities();
        }
    }

    private void OnGUI()
    {
        string res = "Abilities:\n";
        if(abilities[0]) res += abilities[0].name + "\n";
        if(abilities[1]) res += abilities[1].name + "\n";
        if(abilities[2]) res += abilities[2].name;

        abilityHUD.text = res;
    }
}
