using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LocalizeImage))]
public class LocalizeImageEditor : Editor
{
    public override void OnInspectorGUI()
	{
		var localizeImage = (LocalizeImage)target;
		if (localizeImage == null) return;
		
		var languages = Enum.GetNames(typeof(Languages));

		if (localizeImage.Localizations == null)
			localizeImage.Localizations = new Sprite[languages.Length];
	
		if (localizeImage.Localizations.Length < languages.Length)
			Array.Resize(ref localizeImage.Localizations, languages.Length);

		GUILayout.BeginVertical();
		GUILayout.Label("Localizations:");

		for (int i = 0; i < languages.Length; i++)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(16);
			GUILayout.Label(languages[i], GUILayout.Width(24));

			EditorGUI.BeginChangeCheck();
			var value = (Sprite)EditorGUILayout.ObjectField(localizeImage.Localizations[i], typeof(Sprite), false);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(target, "Changed Localization");
				EditorUtility.SetDirty(target);
				localizeImage.Localizations[i] = value;
			}

			if (GUILayout.Button("Set", GUILayout.Width(36)))
			{
				localizeImage.SetLang(Enum.Parse<Languages>(languages[i]));
			}

			GUILayout.EndHorizontal();
		}

		GUILayout.EndVertical();
	}
}
