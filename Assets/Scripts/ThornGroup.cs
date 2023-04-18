using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ThornGroup")]
public class ThornGroup : ScriptableObject
{
    public List<ThornStrike> Thorns = new();
}

[System.Serializable]
public class ThornStrike
{
    public List<bool> Thorns = new();

    public int Width { get => Settings.Width; }
    public int Height { get => Settings.Height - 4; }

    public void Init()
	{
        if (Thorns.Count == Height * Width) return;
        Thorns = new();
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                Thorns.Add(false);
    }

    public bool this[int x, int y]
    {
        get => Thorns[Width * y + x];
        set => Thorns[Width * y + x] = value;
    }
}
