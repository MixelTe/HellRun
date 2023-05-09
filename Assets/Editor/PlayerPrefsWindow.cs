using UnityEngine;
using UnityEditor;
using System.Collections;

public class PlayerPrefsWindow : EditorWindow
{
    [MenuItem("HellRun/Player Prefs")]
    public static void OpenWindow()
    {
        var window = (PlayerPrefsWindow)GetWindow(typeof(PlayerPrefsWindow));
        window.titleContent = new GUIContent("Player Prefs");
        window.Show();
    }
    void OnEnable()
    {
        var myIcon = EditorGUIUtility.Load("Assets/Editor/Icon.png") as Texture2D;
        titleContent.image = myIcon;
    }

    enum FieldType { String, Integer, Float }
	class Field
	{
        public string key;
        public string value;
        public FieldType type;

		public Field(string key, FieldType type)
		{
			this.key = key;
			this.type = type;
            value = GetValue(key, type);
        }

        private static string GetValue(string key, FieldType type)
        {
            if (type == FieldType.Integer)
                return PlayerPrefs.GetInt(key).ToString();
            else if (type == FieldType.Float)
                return PlayerPrefs.GetFloat(key).ToString();
            else
                return PlayerPrefs.GetString(key);
        }
    }

    private Field[] _fields = new Field[0];
    private string _error = "";

    private void Init()
	{
        _fields = new Field[]
        {
            new Field("RecordScore", FieldType.Integer),
            new Field("SoundVolume", FieldType.Float),
            new Field("MusicVolume", FieldType.Float),
            new Field("Language", FieldType.Integer),
        };
        _error = "";
    }

    void OnGUI()
    {
        if (_fields.Length == 0)
            Init();

        EditorGUILayout.LabelField("Player Prefs Editor", EditorStyles.boldLabel);
        EditorGUILayout.Separator();
        
        if (_error != "")
            EditorGUILayout.HelpBox(_error, MessageType.Error);

        EditorGUILayout.BeginVertical();

		foreach (var field in _fields)
		{
            EditorGUILayout.BeginHorizontal();
            field.value = EditorGUILayout.TextField(Utils.SpacesByUppercase(field.key), field.value);

            if (GUILayout.Button("Set", GUILayout.Width(36)))
            {
                GUI.FocusControl(null);
                _error = "";
                if (field.type == FieldType.Integer)
                    if (int.TryParse(field.value, out var result))
                        PlayerPrefs.SetInt(field.key, result);
                    else
                        _error = "Invalid input \"" + field.value + "\"";

                else if (field.type == FieldType.Float)
                    if (float.TryParse(field.value, out var result))
                        PlayerPrefs.SetFloat(field.key, result);
                    else
                        _error = "Invalid input \"" + field.value + "\"";

                else
                    PlayerPrefs.SetString(field.key, field.value);

                if (_error == "")
                    PlayerPrefs.Save();
                else
                    Repaint();
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.Separator();

        if (GUILayout.Button("Reload values"))
        {
            GUI.FocusControl(null);
            Init();
            Repaint();
        }
        EditorGUILayout.EndVertical();

        if (GUI.Button(new Rect(0, 0, position.width, position.height), "", GUIStyle.none))
            GUI.FocusControl(null);
    }
}
