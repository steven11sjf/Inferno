using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using XNode;

namespace Dialogue {
    [NodeTint("#CCCCFF")]
    public class Branch : DialogueBaseNode {

        public string[] conditions;
        [Output] public DialogueBaseNode pass;
        [Output] public DialogueBaseNode fail;

        private bool success;

        public override void Trigger() {
            DialogueSceneGraph scene = (graph as DialogueGraph).sceneGraph;

            // update conditions
            scene.UpdateVariables();

            // Perform condition
            bool success = true;
            bool result;
            for (int i = 0; i < conditions.Length; i++) {
                // if the variable doesn't exist, or the value is false
                if (!scene.CheckVariable(conditions[i], out result) || !result) {
                    success = false;
                    break;
                }
            }

            //Trigger next nodes
            NodePort port;
            if (success) port = GetOutputPort("pass");
            else port = GetOutputPort("fail");
            if (port == null) return;
            for (int i = 0; i < port.ConnectionCount; i++) {
                NodePort connection = port.GetConnection(i);
                (connection.node as DialogueBaseNode).Trigger();
            }
        }
    }
}