using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Copyable))]
public class CopyableDrawer : PropertyDrawer{


	private static Dictionary<string, object> Clipboard = new Dictionary<string, object>();


	public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
	{
		var extraHeight = 0f;
		if (property.isExpanded) {
			extraHeight = property.CountInProperty () * EditorGUIUtility.singleLineHeight;
		}
            return EditorGUIUtility.singleLineHeight + extraHeight;
	}

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {

        string subString = property.propertyPath.Substring(property.propertyPath.IndexOf('[') + 1, 1);
        if (subString == "0")
    {
        var copy = new Rect(new Vector2(position.x + 45, position.y - EditorGUIUtility.singleLineHeight - 3), new Vector2(position.width / 8, EditorGUIUtility.singleLineHeight));
        var paste = new Rect(new Vector2(copy.position.x + copy.width, copy.position.y), new Vector2(position.width / 8, EditorGUIUtility.singleLineHeight));

        if (GUI.Button(copy, "Copy")) {
            var obj = property.serializedObject.targetObject;
            var type = property.serializedObject.targetObject.GetType();
            var field = type.GetField(property.propertyPath.Substring(0, property.propertyPath.IndexOf('.')));
            var val = field.GetValue(obj);

            if (Clipboard.ContainsKey(property.type)) {
                Clipboard[property.type] = val;
            } else {
                Clipboard.Add(property.type, val);
            }

        }

        GUI.enabled = Clipboard.ContainsKey(property.type);
        if (GUI.Button(paste, "Paste")) {
            var val = Clipboard[property.type];

            var obj = property.serializedObject.targetObject;
            var type = property.serializedObject.targetObject.GetType();
            var field = type.GetField(property.propertyPath.Substring(0, property.propertyPath.IndexOf('.')));
            field.SetValue(obj, val);
            property.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(obj);

        }
    }
        GUI.enabled = true;
		EditorGUI.PropertyField (position, property, true);
    }
}
