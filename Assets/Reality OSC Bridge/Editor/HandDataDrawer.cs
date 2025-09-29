using UnityEditor;
using UnityEngine;

namespace StretchSense.OSCBridge
{
    [CustomPropertyDrawer(typeof(HandData))]
    public class HandDataDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Extra space for the help box
            float helpBoxHeight = EditorGUIUtility.singleLineHeight * 2f;
            return helpBoxHeight + EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Calculate rect for help box
            float helpBoxHeight = EditorGUIUtility.singleLineHeight * 2f;
            Rect helpBoxRect = new Rect(position.x, position.y, position.width, helpBoxHeight);

            // Draw help/info box
            EditorGUI.HelpBox(helpBoxRect, "This data is read-only in the inspector and updated at runtime.", MessageType.Info);

            // Property field position (below the help box)
            Rect propertyRect = new Rect(
                position.x,
                position.y + helpBoxHeight + EditorGUIUtility.standardVerticalSpacing,
                position.width,
                EditorGUI.GetPropertyHeight(property, label, true)
            );

            // Draw property normally
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(propertyRect, property, label, true);

            // Discard changes (keep read-only behavior)
            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.Update();
            }
        }
    }

}