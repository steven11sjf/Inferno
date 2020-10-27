using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AbilityEffect
{
    public int idNumber;
    public IDictionary<string, float> effects;

}

public class Ability : MonoBehaviour
{
    [SerializeField]
    protected int id;
    [SerializeField]
    protected AbilityEffect[] effects;

    [SerializeField]
    protected string name;
    [SerializeField]
    protected string description;

    public int GetId() { return id; }
    public string GetName() { return name; }
    public string GetDescription() { return description; }
    public AbilityEffect[] GetEffects() { return effects; }
}
