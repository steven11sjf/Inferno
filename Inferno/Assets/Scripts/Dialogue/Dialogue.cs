using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{

    public string name; // name of NPC we're talking to
    
    [TextArea(3, 10)]
    public string[] sentences; // array of sentences (which actually aren't sentences necessarily, just text that fits in the window!)
}
