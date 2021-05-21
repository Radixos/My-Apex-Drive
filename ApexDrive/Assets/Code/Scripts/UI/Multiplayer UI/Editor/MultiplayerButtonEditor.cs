// Alec Gamble

using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using System.Collections;

[CustomEditor(typeof(MultiplayerButton))]
public class MultiplayerButtonEditor : SelectableEditor
{
    public override void OnInspectorGUI()
    {
        MultiplayerButton button = (MultiplayerButton) target;
        serializedObject.Update();
        base.OnInspectorGUI();
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("DisableControllerOnSubmit"));
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("OnSubmitEvent"));
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("OnCancelEvent"));
        serializedObject.ApplyModifiedProperties();
    }
}
