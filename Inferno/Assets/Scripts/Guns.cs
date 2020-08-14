using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * struct that includes info on a gun's damage, spread, fire rate
 */
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
    public GameObject player;
    public GameObject crosshair;
    public GameObject bulletPrefab;

    private Gun[] guns;

    public int selectedGun; // id of the selected gun
    private float nextShot; // time gun is next ready to fire
    public int numGuns = 3;

    /*
     * Fires the gun, if able to
     */
    public void Shoot()
    {
        if (Time.time < nextShot) return;

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
            bulletScript.velocity = new Vector2(spreadAdjusted.x, spreadAdjusted.y) * guns[selectedGun].speed;
            bulletScript.instantiator = player;
            bulletScript.damage = guns[selectedGun].damage;

            // destroy bullet 3 seconds after firing
            Destroy(bullet, 3.0f);
        }

        // set time until next shot
        nextShot = Time.time + guns[selectedGun].fireRate;
    }

    // Start is called before the first frame update
    void Start()
    {
        guns = new Gun[numGuns];
        guns[0] = new global::Gun("Pistol", 15.0f, 1.0f, 0.0f, 6.0f, 1);
        guns[1] = new global::Gun("Shotgun", 3.0f, 2.0f, 10.0f, 10.0f, 20);
        guns[2] = new global::Gun("SMG", 4.0f, 0.1f, 2.5f, 3.0f, 1);

        ChangeGun(1); // auto-equip pistol
    }

    /*
     * changes the gun to the given gun
     * returns 0 on success
     *         1 if out of range
     */
    public int ChangeGun(int gun)
    {
        // validate input
        if (gun < 0 || gun >= numGuns)
            return 1;

        selectedGun = gun;

        nextShot = Time.time + guns[selectedGun].fireRate + GUN_SWAP_TIME;
        return 0;
    }


}