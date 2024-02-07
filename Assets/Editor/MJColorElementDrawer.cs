using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MjColors))]
public class MJColorElementDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        string[] labelArray = {"Corner", "Corner", "Corner", "Corner", "Button",
                               "Button", "Button", "Button", "Button", "Button",
                               "Button", "Button", "Button"};

        EditorGUI.BeginProperty(position, label, property);
        int arrayIndex = property.GetArrayIndex();
        if (arrayIndex >= 0)
        {
            if (arrayIndex < 4)
            {
                label.text = (arrayIndex + 1).ToString() + " " + labelArray[arrayIndex];
            }
            else
            {
                label.text = (arrayIndex - 3).ToString() + " " + labelArray[arrayIndex];
            }
        }
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        label.text = "";
        var colorRect = new Rect(position.x, position.y + 5, 40, position.height - 10);
        var enumRect = new Rect(position.x + 45, position.y, position.width - 45, position.height);
        var enumValue = (MjColors)property.enumValueIndex;
        GUI.enabled = false;
        var color = MJColorUtility.GetColorFromEnum(enumValue);
        EditorGUI.ColorField(colorRect, label, color, showEyedropper: false, showAlpha: false, hdr: false);
        GUI.enabled = true;
        var enumNew = (MjColors)EditorGUI.EnumPopup(enumRect, enumValue);
        property.enumValueIndex = (int)enumNew;
        EditorGUI.EndProperty();
    }
}
