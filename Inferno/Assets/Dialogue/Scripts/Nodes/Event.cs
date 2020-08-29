using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
namespace Dialogue {
	[NodeTint("#FFFFAA")]
	public class Event : DialogueBaseNode {

        public string actionName;
        public string[] args;

        public override void Trigger()
        {
            ((DialogueGraph)graph).sceneGraph.DoCutsceneAction(actionName, args);
        }
    }
}