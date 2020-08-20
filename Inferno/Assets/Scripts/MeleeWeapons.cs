using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Melee
{
    public Melee(string weaponName, float weaponDamage, float weaponLength, float startingAngle, float angleLength, float swingSpeed, float recoveryTime)
    {
        name = weaponName;
        damage = weaponDamage;
        length = weaponLength;
        start = startingAngle;
        swingLength = Mathf.Abs(angleLength / swingSpeed);
        speed = swingSpeed;
        recovery = recoveryTime;
    }

    public string name;
    public float damage;
    public float length;
    public float start;
    public float swingLength;
    public float speed;
    public float recovery;
}

public class MeleeWeapons : MonoBehaviour
{
    public GameObject crosshair;

    public GameObject swordPrefab;

    public int numMelees = 2;

    private Melee[] melees;
    private float nextSwing; // next time melee is available to swing
    private float currAngle;

    public int equippedMelee;

    // Start is called before the first frame update
    void Start()
    {
        melees = new Melee[numMelees];
        melees[0] = new Melee("Sword", 15.0f, 3.0f, 90.0f, 180.0f, -1080.0f, 0.5f);
        melees[1] = new Melee("Sword", 60.0f, 2.0f, -90.0f, 180.0f, 180.0f, 1.0f);

        equippedMelee = 0;
        nextSwing = Time.time + melees[equippedMelee].recovery;
    }

    public void Swing()
    {
        // validate that swing is available
        if (Time.time < nextSwing) return;


        Vector3 DanielToCrosshair = crosshair.transform.position - transform.position;
        DanielToCrosshair.Normalize();

        float angle = Vector3.SignedAngle(Vector3.up, DanielToCrosshair, Vector3.forward);

        // start at offset
        Quaternion rotation = Quaternion.Euler(0, 0, angle + melees[equippedMelee].start);
        GameObject sword = Instantiate(swordPrefab, transform.position, rotation);
        Sword swordScript = sword.GetComponent<Sword>();
        swordScript.player = gameObject;
        swordScript.speed = melees[equippedMelee].speed;

        // set recovery
        nextSwing = Time.time + melees[equippedMelee].recovery;
    }
}
