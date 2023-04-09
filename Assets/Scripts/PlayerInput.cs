using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private float _moveCollDown = .4f;
    [SerializeField] private float _tapRadius = 10f;
    
    private float _timeSinceLastMove = 0f;
    private Vector3 _touchStart;

    public static Action<Vector2> OnMoved;
    
    public void Update()
    {
        if (!GameManager.GameIsRunning)
			return;

		if (Input.GetMouseButtonDown(0))
        {
            _touchStart = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            var d = Input.mousePosition - _touchStart;
        }
        if (Input.GetMouseButtonUp(0))
        {
            var d = Input.mousePosition - _touchStart;
            if (d.magnitude > _tapRadius)
                OnSwipe(new Vector2(d.x, d.y));
            else
                OnTap(d);
        }
        
        _timeSinceLastMove += Time.deltaTime;

        if (_moveCollDown <= _timeSinceLastMove && Input.GetKeyDown(KeyCode.W))
        {
            OnMoved?.Invoke(new Vector2(0, 1));
            _timeSinceLastMove = 0;
        }
        else if (_moveCollDown <= _timeSinceLastMove && Input.GetKeyDown(KeyCode.S))
        {
            OnMoved?.Invoke(new Vector2(0, -1));
            _timeSinceLastMove = 0;
        }
        else if (_moveCollDown <= _timeSinceLastMove && Input.GetKeyDown(KeyCode.D))
        {
            OnMoved?.Invoke(new Vector2(1, 0));
            _timeSinceLastMove = 0;
        }
        else if (_moveCollDown <= _timeSinceLastMove && Input.GetKeyDown(KeyCode.A))
        {
            OnMoved?.Invoke(new Vector2(-1, 0));
            _timeSinceLastMove = 0;
        }
    }

    private void OnTap(Vector2 position)
    {
        print("tap-tap");
    }

    private void OnSwipe(Vector2 direction)
    {
        Vector2 dir;
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0)
                dir = new Vector2(1, 0);
            else
                dir = new Vector2(-1, 0);
        }
        else
        {
            if (direction.y > 0)
                dir = new Vector2(0, 1);
            else
                dir = new Vector2(0, -1);
        }
        GameManager.Player.OnMoved(dir);
    }
}
