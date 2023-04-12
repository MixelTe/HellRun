using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainSpawner : MonoBehaviour
{
    [SerializeField] private Chain _chain;
    [SerializeField] private ChainGroup[] _chainGroups;
    [SerializeField] private float _strikeTime;

	private void Start()
	{
        StartCoroutine(SpawnChains());
    }

    private IEnumerator SpawnChains()
	{
		while (true)
		{
            var group = _chainGroups.GetRandom();
		    foreach (var strike in group.ChainStrikes)
		    {
                SpawnStrike(strike);

                yield return new WaitForSeconds(_strikeTime / GameManager.GameField.ScrollSpeed);
            }
            yield return new WaitForSeconds(_strikeTime / GameManager.GameField.ScrollSpeed);
        }
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
            positon = new Vector3(i, -GameManager.GameField.ScrolledLines - Settings.Height / 2f + 0.5f + Settings.Height);
            rotation = Quaternion.identity;
        }
        else
        {
            positon = new Vector3(Settings.Width / 2f - 0.5f, -GameManager.GameField.ScrolledLines - i + Settings.Height);
            rotation = Quaternion.Euler(0, 0, 90);
        }
        var chain = Instantiate(_chain, positon, rotation, transform);
        chain.Strike(_strikeTime / GameManager.GameField.ScrollSpeed);
    }
}
