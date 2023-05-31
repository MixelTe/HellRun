using UnityEngine;

public class FieldRow : MonoBehaviour
{
    private static int _lastCrackX = 0;
    private Crack _crackPrefab;
    private Crack _crack;
    private int _width;

    public void Init(int width, int y, GameObject cellPrefab, Crack crackPrefab)
    {
        _crackPrefab = crackPrefab;
        _width = width;
        for (int i = 0; i < width; i++)
        {
            Instantiate(cellPrefab, new Vector2(i, 0), Quaternion.identity, gameObject.transform);
        }
        MoveTo(y);
    }

    public void UpdateCrack()
	{
        if (_crack == null)
            _crack = Instantiate(_crackPrefab, Vector2.zero, Quaternion.identity, gameObject.transform);

        var crackW = 2;
        var crackCount = Random.Range(0, 2);
        if (crackCount == 1)
        {
            var crackI = Random.Range(0, _width - crackW * 2);
            if (crackI > _lastCrackX - crackW && crackI < _lastCrackX + crackW)
                crackI += crackW;
            _lastCrackX = crackI;
            _crack.gameObject.SetActive(true);
            _crack.SetRandom();
            _crack.transform.localPosition = new Vector2(crackI + 0.5f, 0.5f);
        }
		else
		{
            _crack.gameObject.SetActive(false);
        }
    }

    public void MoveTo(int y)
    {
        var position = new Vector2(0, y);
        transform.position = position;
        UpdateCrack();
    }
}
