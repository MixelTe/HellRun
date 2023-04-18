using System.Collections;
using UnityEngine;

public class ChainSpawner : MonoBehaviour
{
    [SerializeField] private Chain _chain;
    [SerializeField] private float _speedMul = 1;
    [SerializeField] private float _strikeTime;
    [SerializeField] private float _strikeChainMul;
    [SerializeField] private ChainGroup[] _chainGroups;

    private float CurStrikeTime { get => _strikeTime / (GameManager.GameField.ScrollSpeed * _speedMul); }

    public void StartSpawn()
	{
        StartCoroutine(SpawnChains());
    }

    private IEnumerator SpawnChains()
	{
		while (true)
		{
            if (!GameManager.GameIsRunning) break;

            var group = _chainGroups.GetRandom();
            foreach (var strike in group.ChainStrikes)
		    {
                if (!GameManager.GameIsRunning) break;

                SpawnStrike(strike);

                yield return WaitForNextStrike();
            }
            yield return WaitForNextStrike();
        }
    }

    private IEnumerator WaitForNextStrike()
	{
        do yield return new WaitForSeconds(CurStrikeTime);
        while (!GameManager.GameField.Scroling);
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
        chain.Strike(CurStrikeTime * _strikeChainMul);
    }
}
