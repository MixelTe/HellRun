using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainSpawner : MonoBehaviour
{
    [SerializeField] private Chain _chain;
    [SerializeField] private ChainGroup[] _chainGroups;
    [SerializeField] private float _strikeTime;

    private bool _strikeNow = false;

    public event Action OnChainGroupEnded;

    [ContextMenu("StartChainGroup")]
    public void StartChainGroup()
    {
        if (_strikeNow) return;
        _strikeNow = true;

        StartCoroutine(SpawnChainGroup());
    }

    private IEnumerator SpawnChainGroup()
	{
        var group = _chainGroups.GetRandom();
		foreach (var strike in group.ChainStrikes)
		{
            SpawnStrike(strike);

            var delay = _strikeTime / GameManager.GameField.ScrollSpeed;
            yield return new WaitForSeconds(delay);
        }
        OnChainGroupEnded?.Invoke();
        _strikeNow = false;
    }

    private void SpawnStrike(ChainStrike strike)
	{
		for (int i = 0; i < strike.VerticalChains.Length; i++)
		{
            if (strike.VerticalChains[i])
			{
                SpawnChain(true, i);
			}
        }
        for (int i = 0; i < strike.HorizontalChains.Length; i++)
        {
            if (strike.HorizontalChains[i])
            {
                SpawnChain(false, i);
            }
        }
    }

    private void SpawnChain(bool vert, int i)
	{
        Vector3 positon;
        Quaternion rotation;
        if (vert)
        {
            positon = new Vector3(i, GameManager.GameField.ScrolledLines - Settings.Height / 2f + 0.5f);
            rotation = Quaternion.identity;
        }
        else
        {
            positon = new Vector3(Settings.Width / 2f - 0.5f, GameManager.GameField.ScrolledLines - i);
            rotation = Quaternion.Euler(0, 0, 90);
        }
        Instantiate(_chain, positon, rotation, transform);
    }
}
