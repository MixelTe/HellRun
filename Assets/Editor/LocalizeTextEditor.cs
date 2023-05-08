using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LocalizeText), true)]
public class LocalizeTextEditor : Editor
{
	public override void OnInspectorGUI()
	{
		var localizeText = (LocalizeText)target;
		if (localizeText == null) return;

		var languages = Enum.GetNames(typeof(Languages));

		if (localizeText.Localizations == null)
			localizeText.Localizations = new string[languages.Length];

		if (localizeText.Localizations.Length < languages.Length)
			Array.Resize(ref localizeText.Localizations, languages.Length);

		GUILayout.BeginVertical();
		GUILayout.Label("Localizations:");

		for (int i = 0; i < languages.Length; i++)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(16);
			GUILayout.Label(languages[i], GUILayout.Width(24));

			EditorGUI.BeginChangeCheck();
			var value = GUILayout.TextArea(localizeText.Localizations[i]);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(target, "Changed Localization");
				EditorUtility.SetDirty(target);
				localizeText.Localizations[i] = value;
			}

			if (GUILayout.Button("Set", GUILayout.Width(36)))
			{
				localizeText.SetLang(Enum.Parse<Languages>(languages[i]));
			}

			GUILayout.EndHorizontal();
		}

		GUILayout.EndVertical();
	}
}

