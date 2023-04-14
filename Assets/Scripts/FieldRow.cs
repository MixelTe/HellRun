using UnityEngine;

public class FieldRow : MonoBehaviour
{
    public void Init(int width, int y, GameObject cellPrefab)
    {
        for (int i = 0; i < width; i++)
        {
            Instantiate(cellPrefab, new Vector2(i, 0), Quaternion.identity, gameObject.transform);
        }
        MoveTo(y);
    }

    public void MoveTo(int y)
    {
        Vector2 position = new Vector2(0, y);
        transform.position = position;
    }
}
