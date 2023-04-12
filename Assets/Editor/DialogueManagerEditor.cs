using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ChainGroup))]
public class ChainGroupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var chainGroup = (ChainGroup)target;
        GUILayout.BeginHorizontal();
        GUILayout.Label("Strikes");
        GUILayout.FlexibleSpace();
        GUILayout.Label($"{chainGroup.ChainStrikes.Count}");
        GUILayout.EndHorizontal();


        foreach (var strike in chainGroup.ChainStrikes)
		{
            DrawStrike(strike);
		}

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        var style = new GUIStyle(GUI.skin.button);
        style.fontSize = 20;
        style.fixedHeight = 25;
        style.fixedWidth = 50;
        if (GUILayout.Button("+", style))
		{
            Undo.RecordObject(target, "Added Strike");
            chainGroup.ChainStrikes.Add(new ChainStrike());
        }
        if (GUILayout.Button("-", style))
        {
            Undo.RecordObject(target, "Removed Strike");
            if (chainGroup.ChainStrikes.Count > 0)
                chainGroup.ChainStrikes.RemoveAt(chainGroup.ChainStrikes.Count - 1);
        }
        GUILayout.EndHorizontal();
    }

    private void DrawStrike(ChainStrike strike)
	{
        var toggleSize = 15;

        var lastRect = GUILayoutUtility.GetLastRect();
        var rectFull = new Rect(100, lastRect.yMax, 100, (toggleSize + 1) * strike.HorizontalChains.Length + 30);

        var height = toggleSize * strike.HorizontalChains.Length;
        for (int i = 0; i < strike.VerticalChains.Length; i++)
		{
            var rect = new Rect(rectFull.x + toggleSize * (i + 1), rectFull.y, toggleSize, toggleSize);
            EditorGUI.BeginChangeCheck();
            var value = GUI.Toggle(rect, strike.VerticalChains[i], "");
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed Strike");
                strike.VerticalChains[i] = value;
            }

            if (value)
			{
                rect.y += toggleSize;
                rect.height = height;
                EditorGUI.DrawRect(rect.Inflate(-2), Color.blue);
                EditorGUI.DrawRect(rect.Inflate(-3), Color.cyan);
			}
        }

        var width = toggleSize * strike.VerticalChains.Length;
        for (int i = 0; i < strike.HorizontalChains.Length; i++)
        {
            var rect = new Rect(rectFull.x, rectFull.y + toggleSize * (i + 1), toggleSize, toggleSize);
            EditorGUI.BeginChangeCheck();
            var value = GUI.Toggle(rect, strike.HorizontalChains[i], "");
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed Strike");
                strike.HorizontalChains[i] = value;
            }

            if (value)
            {
                rect.x += toggleSize;
                rect.width = width;
                EditorGUI.DrawRect(rect.Inflate(-2), Color.blue);
                EditorGUI.DrawRect(rect.Inflate(-3), Color.cyan);
            }
        }

        GUILayout.BeginVertical(GUILayout.Height(rectFull.height));
        GUILayout.Label("Strike");
        GUILayout.EndVertical();
    }
}
