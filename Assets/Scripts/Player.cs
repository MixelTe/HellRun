using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private bool _immortal = false;

    private void Start()
    {
        GameManager.GameField.OnLineMoved += CheckCurPosition;
        GameManager.PlayerInput.OnMoved += OnMoved;
    }
    
    public void OnMoved(Vector2 moveTo)
    {
        Vector2 newPosition = new Vector2(transform.position.x, transform.position.y) + moveTo;
        
        if (GameManager.GameField.IsInsideField(newPosition))
            transform.position = newPosition;
        else
            print("You can't leave the field!");
    }

    private void CheckCurPosition()
    {
        if (_immortal) return;
        if (!GameManager.GameField.IsInsideField(transform.position))
        {
            print("Outside of the field!");
            GameManager.OverGame();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_immortal) return;

        if (collision.TryGetComponent<Enemy>(out var enemy))
        {
            print("Collision with enemy");
            GameManager.OverGame();
        }
    }
}
