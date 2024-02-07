using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MorajaiController))]
public class MJColorCodeHelper : Editor
{
    SerializedProperty useVisualColorCode;
    SerializedProperty colorOrder;

    void OnEnable()
    {
        useVisualColorCode = serializedObject.FindProperty("useVisualColorTool");
        colorOrder = serializedObject.FindProperty("colorOrder");
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        //return;

        MorajaiController controller = (MorajaiController)target;

        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("colorCode"));
        EditorGUILayout.PropertyField(useVisualColorCode);

        if (useVisualColorCode.boolValue)
        {
            if (GUILayout.Button("Update Visual Color Tool"))
            {
                controller.UCO();
                EditorUtility.SetDirty(controller);

            }
            EditorGUILayout.PropertyField(colorOrder);
            if (GUILayout.Button("Update ColorCode"))
            {
                controller.UCC();
                EditorUtility.SetDirty(controller);

            }
            GUILayout.Space(10);
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("colorDefinitions"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("colorToCornerAssociations"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("audioClips"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("buttonsContainer"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("buttonTestPanel"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("showDebug"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("useMouseEvents"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("buttonPressDepth"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("cornerPressDepth"));

        serializedObject.ApplyModifiedProperties();
    }
}
