using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    public bool Immortal = false;
    [SerializeField] private float _moveTime = .4f;
    private Vector2Int _position;
    private float _moveState = 1;
    private Vector2Int _nextMove = Vector2Int.zero;

    private void Start()
    {
        _position.x = Mathf.FloorToInt(transform.position.x);
        _position.y = Mathf.FloorToInt(transform.position.y);

        GameManager.GameField.OnLineMoved += CheckCurPosition;
        GameManager.PlayerInput.OnMoved += OnMoved;
    }

    private void OnMoved(Vector2Int moveTo)
    {
        if (_moveState < 0.5) return;
        _nextMove = moveTo;

        if (_moveState < 1) return;

        StartCoroutine(Move());
    }

    private void CheckCurPosition()
    {
        if (Immortal) return;
        if (!GameManager.GameField.IsInsideField(transform.position))
        {
            print("Outside of the field!");
            Kill();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Immortal) return;

        if (collision.TryGetComponent<Enemy>(out var enemy))
        {
            print("Collision with enemy");
            Kill();
        }
    }

    private IEnumerator Move()
	{
        var endPos = _position + _nextMove;
        var startPos = _position;
        var moveTime = _moveTime / GameManager.GameField.ScrollSpeed;

        if (GameManager.GameField.IsInsideField(endPos))
		{
            _animator.SetFloat("Speed", 1 / moveTime);

            if (_nextMove == Vector2Int.right)
               _animator.SetTrigger("MoveRight");
            else if (_nextMove == Vector2Int.left)
                _animator.SetTrigger("MoveLeft");
            else if (_nextMove == Vector2Int.up)
                _animator.SetTrigger("MoveUp");
            else if (_nextMove == Vector2Int.down)
                _animator.SetTrigger("MoveDown");

            _moveState = 0;
            _position = endPos;
            _nextMove = Vector2Int.zero;
            
            GameManager.SoundPlayer.PlayPlayerMovedSound(moveTime);

            for (float t = 0; t < 1; t += Time.deltaTime / moveTime)
			{
				_moveState = t;
				transform.position = Vector2.Lerp(startPos, endPos, t);
				yield return null;
			}
			transform.position = new Vector3(endPos.x, endPos.y);
			_moveState = 1;

			if (_nextMove != Vector2Int.zero)
				StartCoroutine(Move());
		}
	}

    private void Kill()
	{
        if (!GameManager.GameIsRunning) return;
        GameManager.SoundPlayer.PlayPlayerDeadSound();
        _animator.SetTrigger("Death");
        GameManager.OverGame();
    }

    public void Reborn()
	{
        _animator.SetTrigger("Reborn");
    }
}
