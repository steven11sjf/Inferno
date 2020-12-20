using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipBadges : MonoBehaviour
{
    public Player player;
    public Text badgeHUD;

    public int numBadges = 3;
    public Badge[] badges;

    public Badge badge1;
    public Badge badge2;
    public Badge badge3;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();

        badges = new Badge[numBadges];
    }

    /*
     * Sets the ability effects. Should be called on scene load for levels, but for testing purposes it's bound to interact.
     */
    public void SetBadges()
    {
        ResetToDefaultParameters();

        foreach (Badge b in badges)
        {
            if (b == null) continue;
            Debug.Log("Setting badge " + b.GetId());

            BadgeEffect[] be = b.GetEffects();
            foreach (BadgeEffect effect in be)
            {
                Debug.Log("Setting badge effect for " + effect.idNumber);
                SetBadgeEffect(effect);
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
    void SetBadgeEffect(BadgeEffect effect)
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

            // shotgun
            case 3003:
                player.m_Guns.ChangeGunProperty(1, "damage", effect.effects["shotgunDmg"]);
                player.m_Guns.ChangeGunProperty(1, "fireRate", effect.effects["shotgunFireRate"]);
                player.m_Guns.ChangeGunProperty(1, "spread", effect.effects["shotgunSpread"]);
                player.m_Guns.ChangeGunProperty(1, "speed", effect.effects["shotgunBulletSpeed"]);
                player.m_Guns.ChangeGunProperty(1, "projectiles", effect.effects["shotgunNumProj"]);
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
            if (badges[0]) badges[0] = null;
            else badges[0] = badge1;
            SetBadges();
        }

        if (Input.GetKeyDown("x"))
        {
            if (badges[1]) badges[1] = null;
            else badges[1] = badge2;
            SetBadges();
        }

        if (Input.GetKeyDown("c"))
        {
            if (badges[2]) badges[2] = null;
            else badges[2] = badge3;
            SetBadges();
        }
    }

    private void OnGUI()
    {
        string res = "Badges:\n";
        if(badges[0]) res += badges[0].name + "\n";
        if(badges[1]) res += badges[1].name + "\n";
        if(badges[2]) res += badges[2].name;

        badgeHUD.text = res;
    }
}
