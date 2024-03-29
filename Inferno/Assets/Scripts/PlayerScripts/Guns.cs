﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Struct that includes info on a gun's firing pattern
/// </summary>
struct Gun
{
    public Gun(string weaponName, float dmg, float rate, float spr, float spd, int projPerShot)
    {
        name = weaponName;
        damage = dmg;
        fireRate = rate;
        spread = spr;
        speed = spd;
        projectiles = projPerShot;
    }

    public string name; // name of the weapon
    public float damage; // damage per pellet
    public float fireRate; // time between bullets
    public float spread; // range in degrees that bullets can stray
    public float speed; // speed of each bullet
    public int projectiles; // number of projectiles that are fired per shot
}

public class Guns : MonoBehaviour
{
    public float GUN_SWAP_TIME;
    public float BULLET_SPEED_RANGE;
    private GameObject player;
    private GameObject crosshair;
    public GameObject bulletPrefab;

    private GameLogic gameLogic;
    private Gun[] gun_defaults;
    private Gun[] guns;

    public int selectedGun; // id of the selected gun
    private float nextShot; // time gun is next ready to fire
    public int numGuns = 3;
    public float swap_time;

    /// <summary>
    /// Fires the gun, if possible
    /// </summary>
    public void Shoot()
    {
        if (nextShot > 0.0f) return;

        // get a normalized vector pointing from Daniel to the crosshair
        Vector3 danielToCrosshair = crosshair.transform.position - transform.position;
        danielToCrosshair.Normalize();

        // for each projectile
        for (int i = 0; i < guns[selectedGun].projectiles; ++i)
        {
            // choose a random spread
            float spread = guns[selectedGun].spread;
            float spreadDeg = Random.Range(-spread, spread);
            // get vec3 direction for spread
            Vector3 spreadAdjusted = Quaternion.AngleAxis(spreadDeg, Vector3.forward) * danielToCrosshair;

            // initialize bullet with spread and direction
            // instantiate a new bullet from the center of daniel
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

            // fire bullet in the direction of the crosshair
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            bulletScript.velocity = new Vector2(spreadAdjusted.x, spreadAdjusted.y) * guns[selectedGun].speed * (1.0f + Random.Range(0, BULLET_SPEED_RANGE));
            bulletScript.instantiator = player;
            bulletScript.damage = guns[selectedGun].damage;
        }

        // set time until next shot
        nextShot = guns[selectedGun].fireRate;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = gameObject;
        crosshair = player.GetComponent<Player>().m_Crosshair;
        swap_time = GUN_SWAP_TIME;

        gameLogic = FindObjectOfType<GameLogic>();

        gun_defaults = new Gun[numGuns];
        guns = new Gun[numGuns];
        gun_defaults[0] = new global::Gun("Pistol", 15.0f, 1.0f, 0.0f, 6.0f, 1);
        gun_defaults[1] = new global::Gun("Shotgun", 7.0f, 2.0f, 10.0f, 10.0f, 8);
        gun_defaults[2] = new global::Gun("SMG", 4.0f, 0.1f, 2.5f, 3.0f, 1);

        guns[0] = gun_defaults[0];
        guns[1] = gun_defaults[1];
        guns[2] = gun_defaults[2];

        ChangeGun(0); // auto-equip pistol
    }

    /// <summary>
    /// Changes the gun to a valid weapon
    /// </summary>
    /// <param name="gun">the id of the gun</param>
    /// <returns>0 on success</returns>
    public int ChangeGun(int gun)
    {
        // validate input
        if (gun < 0 || gun >= numGuns)
            return 1;

        selectedGun = gun;

        nextShot = swap_time;
        return 0;
    }

    private void Update()
    {
        // check if gameplay is active
        if(gameLogic.DoGameplay())
        {
            nextShot -= Time.deltaTime;
            return;
        }
    }

    public void ChangeGunProperty(int index, string property, float val)
    {
        switch(property)
        {
            case "damage":
                guns[index].damage *= val;
                break;
            case "fireRate":
                guns[index].fireRate *= val;
                break;
            case "spread":
                guns[index].spread *= val;
                break;
            case "speed":
                guns[index].speed *= val;
                break;
            case "projectiles":
                guns[index].projectiles += (int)val;
                break;
            default:
                Debug.LogError("Error: Propery " + property + " not valid!");
                break;
        }
    }

    public void ResetToDefault()
    {
        swap_time = GUN_SWAP_TIME;
        for(int i=0; i<numGuns; i++)
        {
            guns[i] = gun_defaults[i];
        }
    }
}