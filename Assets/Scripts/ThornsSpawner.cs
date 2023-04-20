using UnityEngine;

public class ThornsSpawner : MonoBehaviour
{
    [SerializeField] private Thorn _thornPrefab;
    private ObjectPool<Thorn> _thornPool;

    private void Start()
    {
        _thornPool = new(_thornPrefab, Settings.Width * 6, transform);
        GameManager.GameField.OnLineMoved += CreateNewThornsRow;
        
        CreateThornRow(0, 0);
        CreateThornRow(1, 1);
        CreateThornRow(2, 2);
        
        CreateThornRow(Settings.Height - 1, 0);
        CreateThornRow(Settings.Height, 1);
        CreateThornRow(Settings.Height + 1, 1);
    }

    private void CreateThornRow(int y, int growingState)
    {
        for (int i = 0; i < Settings.Width; i++)
        {
            var thorn = _thornPool.GetFreeElement(new Vector2(i, y));
            thorn.ChangeThornsState(growingState);
        }
    }

    private void CreateNewThornsRow()
    {
        CreateThornRow(-GameManager.GameField.ScrolledLines, 0);
        CreateThornRow(-GameManager.GameField.ScrolledLines +Settings.Height-1, 0);
    }
}
