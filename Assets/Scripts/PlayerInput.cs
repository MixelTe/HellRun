using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private float _tapRadius = 10f;
    private Vector3 _touchStart;

    public event Action<Vector2Int> OnMoved;
    private bool _afterPause = true;

    public void Update()
    {
        if (!GameManager.GameIsRunning) return;

        if (Input.GetKeyDown(KeyCode.Space))
            GameManager.TogglePause();
        
        if (GameManager.GameIsPaused)
		{
            _afterPause = true;
            return;
		}

        if (Input.GetMouseButtonDown(0))
        {
            _touchStart = Input.mousePosition;
            _afterPause = false;
        }
        if (Input.GetMouseButtonUp(0) && !_afterPause)
        {
            var d = Input.mousePosition - _touchStart;
            if (d.magnitude > _tapRadius)
                OnSwipe(d);
            else
                OnTap(d);
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            MoveUp();
        
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            MoveDown();
        
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            MoveRight();
     
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            MoveLeft();
    }

    private void OnTap(Vector2 position)
    {
        print("tap-tap");
    }

    private void OnSwipe(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            if (direction.x > 0) MoveRight();
            else MoveLeft();
        else
            if (direction.y > 0) MoveUp();
            else MoveDown();
    }

    public void MoveRight() => OnMoved?.Invoke(Vector2Int.right);
    public void MoveLeft() => OnMoved?.Invoke(Vector2Int.left);
    public void MoveUp() => OnMoved?.Invoke(Vector2Int.up);
    public void MoveDown() => OnMoved?.Invoke(Vector2Int.down);
}
