using UnityEngine;
using UnityEditor;
using System;

class LocalizationWindow : EditorWindow
{
    [MenuItem("HellRun/Localization")]
    public static void ShowWindow()
    {
		var window = GetWindow(typeof(LocalizationWindow));
        window.titleContent = new GUIContent("Localization");
		var position = window.position;
		position.size = new Vector2(200, 100);
		window.position = position;
	}
	void OnEnable()
	{
		var myIcon = EditorGUIUtility.Load("Assets/Editor/Icon.png") as Texture2D;
		titleContent.image = myIcon;
	}

	private void OnGUI()
    {
		GUILayout.BeginVertical();
        GUILayout.Label("Localizations", EditorStyles.boldLabel);
		var languages = Enum.GetNames(typeof(Languages));

		for (int i = 0; i < languages.Length; i++)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(16);
			GUILayout.Label(languages[i], GUILayout.Width(24));

			if (GUILayout.Button("Set", GUILayout.Width(36)))
			{
				Localization.Language = Enum.Parse<Languages>(languages[i]);
				SceneView.RepaintAll();
			}

			GUILayout.EndHorizontal();
		}
		GUILayout.EndVertical();
	}
}