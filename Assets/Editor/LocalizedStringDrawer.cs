using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomPropertyDrawer(typeof(LocalizedString))]
public class LocalizedStringDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		var localizations = property.FindPropertyRelative("_localizations");
		EditorGUI.BeginProperty(position, label, property);

		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
		var languages = Enum.GetNames(typeof(Languages));

		var curSize = localizations.arraySize;
		for (int i = 0; i < languages.Length - curSize; i++)
		{
			localizations.InsertArrayElementAtIndex(curSize + i);
		}

		var labelWidth = EditorGUIUtility.labelWidth;
		EditorGUIUtility.labelWidth = 24;

		for (int i = 0; i < languages.Length; i++)
		{
			var prop = localizations.GetArrayElementAtIndex(i);

			label = EditorGUI.BeginProperty(rect, new GUIContent(languages[i]), prop);
			EditorGUI.PropertyField(rect, prop, label);
			EditorGUI.EndProperty();

			rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
		}

		EditorGUIUtility.labelWidth = labelWidth;
		EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty();
	}

	//public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	//{
	//	return 50.0f;
	//}
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		int lineCount = property.FindPropertyRelative("_localizations").arraySize;
		return EditorGUIUtility.singleLineHeight * lineCount + EditorGUIUtility.standardVerticalSpacing * (lineCount - 1);
	}

}