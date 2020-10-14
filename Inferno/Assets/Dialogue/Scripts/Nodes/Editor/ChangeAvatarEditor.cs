using UnityEditor;
using UnityEngine;
using XNodeEditor;

namespace DialogueEditor
{
    [CustomNodeEditor(typeof(Dialogue.ChangeAvatar))]
    public class ChangeAvatarEditor : NodeEditor
    {

        public override void OnBodyGUI()
        {
            serializedObject.Update();

            Dialogue.ChangeAvatar node = target as Dialogue.ChangeAvatar;

            NodeEditorGUILayout.PortField(target.GetInputPort("input"), GUILayout.Width(100));
            // the input port
            EditorGUILayout.Space();
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("source"));

            //NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("text"), GUIContent.none);
            //NodeEditorGUILayout.DynamicPortList("answers", typeof(DialogueBaseNode), serializedObject, NodePort.IO.Output, Node.ConnectionType.Override);


            serializedObject.ApplyModifiedProperties();
        }

        public override int GetWidth()
        {
            return 336;
        }
    }
}