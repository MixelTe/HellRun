using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChainGroup")]
public class ChainGroup : ScriptableObject
{
    public List<ChainStrike> ChainStrikes = new();
}

[System.Serializable]
public class ChainStrike
{
    public bool[] VerticalChains = new bool[Settings.Width];
    public bool[] HorizontalChains = new bool[Settings.Height];
}

