using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BadgeEffect
{
    public int idNumber;
    public IDictionary<string, float> effects;

}

public class Badge : MonoBehaviour
{
    [SerializeField]
    protected int id;
    [SerializeField]
    protected BadgeEffect[] effects;

    [SerializeField]
    protected new string name;
    [SerializeField]
    protected string description;

    public int GetId() { return id; }
    public string GetName() { return name; }
    public string GetDescription() { return description; }
    public BadgeEffect[] GetEffects() { return effects; }
}
