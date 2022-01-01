using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrongerPistol : Badge
{
    // Start is called before the first frame update
    void Start()
    {
        effects = new BadgeEffect[1];
        effects[0].idNumber = 3002;
        effects[0].effects = new Dictionary<string, float>();
        effects[0].effects.Add("pistolDmg", 1.0f);
        effects[0].effects.Add("pistolFireRate", 0.7f);
        effects[0].effects.Add("pistolSpread", 1.0f);
        effects[0].effects.Add("pistolBulletSpeed", 3.0f);
    }
}
