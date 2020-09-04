using System.Collections.Generic;
using System.Linq;
using Dialogue;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace DialogueEditor {
    [CustomNodeEditor(typeof(Dialogue.Event))]
    public class EventEditor : NodeEditor {

        public override void OnBodyGUI()
        {
            serializedObject.Update();

            Dialogue.Event node = target as Dialogue.Event;

            NodeEditorGUILayout.PortField(target.GetInputPort("input"), GUILayout.Width(100));
            // the input port
            EditorGUILayout.Space();
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("actionName"));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("args"));

            //NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("text"), GUIContent.none);
            //NodeEditorGUILayout.DynamicPortList("answers", typeof(DialogueBaseNode), serializedObject, NodePort.IO.Output, Node.ConnectionType.Override);


            serializedObject.ApplyModifiedProperties();
        }

        public override int GetWidth() {
            return 336;
        }
    }
}