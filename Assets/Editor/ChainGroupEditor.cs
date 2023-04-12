using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ChainGroup))]
public class ChainGroupEditor : Editor
{
    private static Color _colorThorns = Utils.ColorFromRGB(198, 77, 77);
    private static Color _colorChain = Utils.ColorFromRGB(0, 84, 143);
    private static Color _colorChainOutliine = Utils.ColorFromRGB(0, 40, 140);

    public override void OnInspectorGUI()
    {
        var chainGroup = (ChainGroup)target;
        if (chainGroup == null) return;

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
        var style = new GUIStyle(GUI.skin.button)
        {
            fontSize = 20,
            fixedHeight = 25,
            fixedWidth = 50
        };
        if (GUILayout.Button("+", style))
        {
            Undo.RecordObject(target, "Added Strike");
            EditorUtility.SetDirty(target);
            chainGroup.ChainStrikes.Add(new ChainStrike());
        }
        if (GUILayout.Button("-", style))
        {
            Undo.RecordObject(target, "Removed Strike");
            EditorUtility.SetDirty(target);
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

        var width = toggleSize * strike.VerticalChains.Length;
        for (int i = 0; i < strike.HorizontalChains.Length; i++)
        {
            var rect = new Rect(rectFull.x, rectFull.y + toggleSize * (i + 1), toggleSize, toggleSize);
            EditorGUI.BeginChangeCheck();
            var value = GUI.Toggle(rect, strike.HorizontalChains[i], "");
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed Strike");
                EditorUtility.SetDirty(target);
                strike.HorizontalChains[i] = value;
            }

            rect.x += toggleSize;
            rect.width = width;
            if (i < 2 || i > strike.HorizontalChains.Length - 3)
            {
                EditorGUI.DrawRect(rect, _colorThorns);
            }
            if (value)
            {
                EditorGUI.DrawRect(rect.Inflate(-2), _colorChainOutliine);
                EditorGUI.DrawRect(rect.Inflate(-3), _colorChain);
            }
        }

        var height = toggleSize * strike.HorizontalChains.Length;
        for (int i = 0; i < strike.VerticalChains.Length; i++)
        {
            var rect = new Rect(rectFull.x + toggleSize * (i + 1), rectFull.y, toggleSize, toggleSize);
            EditorGUI.BeginChangeCheck();
            var value = GUI.Toggle(rect, strike.VerticalChains[i], "");
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed Strike");
                EditorUtility.SetDirty(target);
                strike.VerticalChains[i] = value;
            }

            if (value)
            {
                rect.y += toggleSize;
                rect.height = height;
                EditorGUI.DrawRect(rect.Inflate(-2), _colorChainOutliine);
                EditorGUI.DrawRect(rect.Inflate(-3), _colorChain);
            }
        }

        GUILayout.BeginVertical(GUILayout.Height(rectFull.height));
        GUILayout.Label("Strike");
        GUILayout.EndVertical();
    }
}
