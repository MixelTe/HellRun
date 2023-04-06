using UnityEngine;

public class FieldRow : MonoBehaviour
{
    [SerializeField] private GameObject _cellPrefab;
    public void CreateLine(int width)
    {
        for (int i = 0; i < width; i++)
        {
            Vector2 position;
            position.y = 0;
            position.x = i;
            
            GameObject cell = Instantiate(_cellPrefab, gameObject.transform);
            cell.transform.position = position;
        }
    }
}
