using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LocalizeTextDynamic), true)]
public class LocalizeTextDynamicEditor : Editor
{
	public override void OnInspectorGUI()
	{
		var localizeText = (LocalizeTextDynamic)target;
		if (localizeText == null) return;

		DrawDefaultInspector();

		var languages = Enum.GetNames(typeof(Languages));
		GUILayout.BeginVertical();
		GUILayout.Label("Localize Text Dynamic:", EditorStyles.boldLabel);
		if (GUILayout.Button("Add", GUILayout.Width(36)))
		{
			Undo.RecordObject(target, "Added Localization Key");
			EditorUtility.SetDirty(target);
			if (!localizeText.Localizations.Contains("Key"))
			{
				localizeText.Localizations.Add("Key");
				localizeText.Localizations.AddRange(languages);
			}
		}
		var itemLen = (languages.Length + 1);
		for (int keyI = 0; keyI < localizeText.Localizations.Count / (languages.Length + 1); keyI++)
		{
			var i = keyI * itemLen;
			GUILayout.BeginHorizontal();
			var key = localizeText.Localizations[i];
			GUILayout.Label("Key:");
			EditorGUI.BeginChangeCheck();
			var newKey = GUILayout.TextField(key);
			if (EditorGUI.EndChangeCheck())
			{
				newKey = newKey.Trim();
				if (!localizeText.Localizations.Contains(newKey))
				{
					Undo.RecordObject(target, "Changed Localization Key");
					EditorUtility.SetDirty(target);
					localizeText.Localizations[i] = newKey;
				}
			}
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Delete"))
			{
				Undo.RecordObject(target, "Delete Localization Key");
				EditorUtility.SetDirty(target);
				localizeText.Localizations.RemoveRange(i, itemLen);
				GUILayout.EndHorizontal();
				break;
			}
			GUILayout.EndHorizontal();

			for (int j = 0; j < languages.Length; j++)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Space(16);
				GUILayout.Label(languages[j], GUILayout.Width(24));

				EditorGUI.BeginChangeCheck();
				var value = GUILayout.TextArea(localizeText.Localizations[i + 1 + j]);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(target, "Changed Localization");
					EditorUtility.SetDirty(target);
					localizeText.Localizations[i + 1 + j] = value;
				}

				if (GUILayout.Button("Set", GUILayout.Width(36)))
				{
					localizeText.SetText(key);
					localizeText.SetLang((Languages)j);
				}

				GUILayout.EndHorizontal();
			}
		}
		GUILayout.BeginHorizontal();
		GUILayout.Label("Values for test:");
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Add", GUILayout.Width(36)))
		{
			Array.Resize(ref localizeText.ValuesTest, localizeText.ValuesTest.Length + 1);
		}
		if (GUILayout.Button("Remove", GUILayout.Width(72)))
		{
			Array.Resize(ref localizeText.ValuesTest, localizeText.ValuesTest.Length - 1);
		}
		GUILayout.EndHorizontal();
		for (int i = 0; i < localizeText.ValuesTest.Length; i++)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(16);

			EditorGUI.BeginChangeCheck();
			var v = localizeText.ValuesTest[i] ?? "";
			var value = GUILayout.TextField(v.ToString());
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(target, "Changed Values");
				EditorUtility.SetDirty(target);
				localizeText.ValuesTest[i] = value;
			}
			GUILayout.EndHorizontal();
		}

		GUILayout.EndVertical();
	}
}
