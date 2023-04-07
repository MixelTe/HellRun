using UnityEngine;

public class Player : MonoBehaviour
{
    private void Start()
    {
        GameField.OnLineMoved += CheckNowPosition;
    }
    
    public void OnMoved(Vector2 moveTo)
    {
        if (GameManager.GameField.IsInsideField(new Vector2(transform.position.x, transform.position.y) + moveTo))
        {
            transform.position = new Vector3(transform.position.x + moveTo.x, transform.position.y + moveTo.y, 0);
        }
        else
            print("You can't leave the field!");
    }

    private void CheckNowPosition()
    {
        if (!GameManager.GameField.IsInsideField(transform.position))
        {
            print("Outside of the field!");
            GameManager.OverGame();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.GetComponent<Enemy>() != null)
        {
            print("Collision with enemy");
            GameManager.OverGame();
        }
    }
}
