using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastWeaponSwap : Ability
{
    // Start is called before the first frame update
    void Start()
    {
        effects = new AbilityEffect[1];
        effects[0].idNumber = 3001;
        effects[0].effects = new Dictionary<string, float>();
        effects[0].effects.Add("gunSwapTime", 0.7f);
    }
}
