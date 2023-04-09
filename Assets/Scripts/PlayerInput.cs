using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private float _moveCoolDown = .4f;
    [SerializeField] private float _tapRadius = 10f;
    
    private float _timeSinceLastMove = 0f;
    private Vector3 _touchStart;

    public Action<Vector2> OnMoved;

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

        _timeSinceLastMove += Time.deltaTime;

        if (_moveCoolDown > _timeSinceLastMove)
            return;
        if (Input.GetKeyDown(KeyCode.W))
        {
            OnMoved?.Invoke(Vector2.up);
            _timeSinceLastMove = 0;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            OnMoved?.Invoke(Vector2.down);
            _timeSinceLastMove = 0;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            OnMoved?.Invoke(Vector2.right);
            _timeSinceLastMove = 0;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            OnMoved?.Invoke(Vector2.left);
            _timeSinceLastMove = 0;
        }

    }

    private void OnTap(Vector2 position)
    {
        print("tap-tap");
    }

    private void OnSwipe(Vector2 direction)
    {
        direction = DetermineDirection(direction);

        if (_moveCoolDown <= _timeSinceLastMove)
        {
            OnMoved?.Invoke(direction);
            _timeSinceLastMove = 0;
        }
    }

    private static Vector2 DetermineDirection(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            return direction.x > 0 ? Vector2.right : Vector2.left;
        else
            return direction.y > 0 ? Vector2.up : Vector2.down;
    }
}
