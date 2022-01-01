using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniShotgun : Badge
{
    // Start is called before the first frame update
    void Start()
    {
        effects = new BadgeEffect[1];
        effects[0].idNumber = 3003;
        effects[0].effects = new Dictionary<string, float>();
        effects[0].effects.Add("shotgunDmg", 0.6f);
        effects[0].effects.Add("shotgunFireRate", 0.10f);
        effects[0].effects.Add("shotgunSpread", 1.5f);
        effects[0].effects.Add("shotgunBulletSpeed", 0.8f);
        effects[0].effects.Add("shotgunNumProj", 0.0f);
    }
}
