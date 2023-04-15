using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private float _tapRadius = 10f;
    private Vector3 _touchStart;

    public event Action<Vector2Int> OnMoved;

    public void Update()
    {
        if (!GameManager.GameIsRunning)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            _touchStart = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            var d = Input.mousePosition - _touchStart;
            if (d.magnitude > _tapRadius)
                OnSwipe(d);
            else
                OnTap(d);
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            OnMoved?.Invoke(Vector2Int.up);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            OnMoved?.Invoke(Vector2Int.down);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            OnMoved?.Invoke(Vector2Int.right);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            OnMoved?.Invoke(Vector2Int.left);
        }
    }

    private void OnTap(Vector2 position)
    {
        print("tap-tap");
    }

    private void OnSwipe(Vector2 direction)
    {
        OnMoved?.Invoke(DetermineDirection(direction));
    }

    private static Vector2Int DetermineDirection(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            return direction.x > 0 ? Vector2Int.right : Vector2Int.left;
        else
            return direction.y > 0 ? Vector2Int.up : Vector2Int.down;
    }
}
