using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementPlus : Ability
{
    // Start is called before the first frame update
    void Start()
    {
        effects = new AbilityEffect[3];

        effects[0].idNumber = 1001;
        effects[0].effects = new Dictionary<string, float>();
        effects[0].effects.Add("movementSpeed", 1.8f);

        effects[1].idNumber = 1002;
        effects[1].effects = new Dictionary<string, float>();
        effects[1].effects.Add("movementSpeed", 1.8f);

        effects[2].idNumber = 1003;
        effects[2].effects = new Dictionary<string, float>();
        effects[2].effects.Add("dashCooldown", 0.6f);
        effects[2].effects.Add("dashSpeed", 2.0f);
        effects[2].effects.Add("dashTime", 0.5f);
    }
}
