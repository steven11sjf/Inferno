using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XNode;

namespace Dialogue
{
    [NodeTint("#FFCCCC")]
    public class ChangeAvatar : DialogueBaseNode
    {
        public Sprite source;

        public override void Trigger()
        {
            // gets the scene graph
            DialogueSceneGraph scene = (graph as DialogueGraph).sceneGraph;
            scene.ChangeAvatar(source);
        }
    }
}