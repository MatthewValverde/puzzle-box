using UnityEditor;
using UnityEngine;

public static class MJColorUtility
{
    public static Color GetColorFromEnum(MjColors colorEnum)
    {
        switch (colorEnum)
        {
            case MjColors.Black: return Color.black;
            case MjColors.Green: return new Color(0, 0.7f, 0);
            case MjColors.Red: return new Color(0.7f, 0, 0);
            case MjColors.Blue: return new Color(0, 0.498f, 1f);
            case MjColors.Yellow: return Color.yellow;
            case MjColors.Orange: return new Color(1f, 0.647f, 0); // RGB for orange
            case MjColors.Purple: return new Color(0.6f, 0, 0.6f); // RGB for purple
            case MjColors.Pink: return new Color(1f, 0.753f, 0.796f); // RGB for pink
            case MjColors.White: return Color.white;
            case MjColors.Gray: return Color.gray;
            default: return Color.white;
        }
    }
}

public static class SerializedPropertyExtensions
{
    public static int GetArrayIndex(this SerializedProperty property)
    {
        string path = property.propertyPath;
        int startIndex = path.LastIndexOf("[") + 1;
        int endIndex = path.LastIndexOf("]");

        if (startIndex < 0 || endIndex < 0)
            return -1; // Not an array element

        string indexString = path.Substring(startIndex, endIndex - startIndex);
        if (int.TryParse(indexString, out int index))
            return index;

        return -1; // Unable to parse index
    }
}

