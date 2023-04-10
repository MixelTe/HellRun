using UnityEngine;

public class ThornsSpawner : MonoBehaviour
{
    [SerializeField] private Thorn _thornPrefab;

    private void Start()
    {
        GameField.OnLineMoved += CreateNewThornsRow;
        
        CreateThornRow(1, true);
        CreateThornRow(2, false);
        
        CreateThornRow(GameManager.GameField.Height, true);
        CreateThornRow(GameManager.GameField.Height - 1, false);
    }

    private void CreateThornRow(int y, bool growingState)
    {
        var thornRowEmptyObject = new GameObject("ThornRow");
        thornRowEmptyObject.transform.SetParent(gameObject.transform);
        
        for (int i = 0; i < GameManager.GameField.Width; i++)
        {
            var thornRow = Instantiate(_thornPrefab, new Vector2(i, y), Quaternion.identity, thornRowEmptyObject.transform);
            thornRow.IsGrowing = growingState;
        }
    }

    private void CreateNewThornsRow()
    {
        CreateThornRow(-GameManager.GameField.ScrolledLines+1, true);
        CreateThornRow(-GameManager.GameField.ScrolledLines + GameManager.GameField.Height-1, false);
    }

    
}
