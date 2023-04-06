using UnityEngine;

public class CellLine : MonoBehaviour
{
    public void CreateLine(int y, int width, GameObject cellPrefab)
    {
        for (int i = 0; i < width; i++)
        {
            Vector2 position;
            position.y = 0;
            position.x = i;
            
            GameObject cell = Instantiate(cellPrefab, gameObject.transform);
            cell.transform.position = position;
        }
    }
}
