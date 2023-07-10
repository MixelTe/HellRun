using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(ThornGroup))]
public class ThornGroupEditor : Editor
{
    private static Color _colorThorns = Utils.ColorFromRGB(198, 77, 77);
    private static Color _colorBadPlace = Utils.ColorFromRGB(94, 62, 174);
    private static Color _colorBack = Utils.ColorFromRGB(21, 75, 81);

    public override void OnInspectorGUI()
    {
        var thornGroup = (ThornGroup)target;
        if (thornGroup == null) return;

        GUILayout.BeginHorizontal();
        GUILayout.Label("Strikes");
        GUILayout.FlexibleSpace();
        GUILayout.Label($"{thornGroup.Thorns.Count}");
        GUILayout.EndHorizontal();
        DrawHint();

        for (int i = 0; i < thornGroup.Thorns.Count; i++)
		{
            DrawThorns(thornGroup.Thorns[i], i > 0 ? thornGroup.Thorns[i - 1] : null);
        }

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        var style = new GUIStyle(GUI.skin.button)
        {
            fontSize = 20,
            fixedHeight = 25,
            fixedWidth = 50
        };
        if (GUILayout.Button("+", style))
        {
            Undo.RecordObject(target, "Added Thorns");
            EditorUtility.SetDirty(target);
            thornGroup.Thorns.Add(new ThornStrike());
        }
        if (GUILayout.Button("-", style))
        {
            Undo.RecordObject(target, "Removed Thorns");
            EditorUtility.SetDirty(target);
            if (thornGroup.Thorns.Count > 0)
                thornGroup.Thorns.RemoveAt(thornGroup.Thorns.Count - 1);
        }
        GUILayout.EndHorizontal();
    }

	private void DrawThorns(ThornStrike thornGroup, ThornStrike thornGroupPrev)
	{
        thornGroup.Init();
        var gap = 3;
        var toggleSize = 15 + gap * 2;

        var lastRect = GUILayoutUtility.GetLastRect();
        var width = toggleSize * thornGroup.Width;
        var rectFull = new Rect(50, lastRect.yMax, width, toggleSize * (thornGroup.Height + 4));

        EditorGUI.DrawRect(rectFull, _colorBack);

        DrawThornsLine(rectFull, toggleSize, 0);
        DrawThornsLine(rectFull, toggleSize, thornGroup.Height + 2);

        for (int y = 0; y < thornGroup.Height; y++)
        {
            for (int x = 0; x < thornGroup.Width; x++)
            {
                var rect = new Rect(rectFull.x + toggleSize * x, rectFull.y + toggleSize * (y + 2), toggleSize, toggleSize);
                if (thornGroupPrev != null)
				{
                    if (thornGroupPrev[x, y])
					{
                        EditorGUI.DrawRect(rect, _colorThorns);
					}
					else
					{
                        var hasEscape = 
                            !thornGroupPrev.TryGet(x - 1, y) || 
                            !thornGroupPrev.TryGet(x + 1, y) || 
                            !thornGroupPrev.TryGet(x, y - 1) || 
                            !thornGroupPrev.TryGet(x, y + 1);
                        if (!hasEscape)
						{
                            EditorGUI.DrawRect(rect, _colorBadPlace);
                        }
                    }
				}
                rect = rect.Inflate(-gap);
                EditorGUI.BeginChangeCheck();
                var value = GUI.Toggle(rect, thornGroup[x, y], "");
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, "Changed Thorn");
                    EditorUtility.SetDirty(target);
                    thornGroup[x, y] = value;
                }
            }
        }

        GUILayout.BeginVertical(GUILayout.Height(rectFull.height));
        GUILayout.Label("|");
        GUILayout.EndVertical();
    }

	private void DrawThornsLine(Rect rectFull, float toggleSize, int y)
	{
        var rect = new Rect(rectFull.x, rectFull.y + toggleSize * y, rectFull.width, toggleSize * 2);
        EditorGUI.DrawRect(rect, _colorThorns);
    }

    private void DrawHint()
	{
        var lastRect = GUILayoutUtility.GetLastRect();
        var rect = new Rect(lastRect.x + 60, lastRect.y, 60, 16);
        EditorGUI.DrawRect(rect, _colorBadPlace);
        GUI.Label(rect, "Bad place");

        rect.x += 80;
        EditorGUI.DrawRect(rect, _colorThorns);
        GUI.Label(rect, "Past thorns");

    }
}
