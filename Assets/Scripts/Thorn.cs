using System;
using UnityEngine;

public class Thorn : MonoBehaviour
{
    public bool IsGrowing;
    
    private void Start()
    { 
        GameField.OnLineMoved += ChangeThornsState;
        
        if (IsGrowing)
            gameObject.AddComponent<BoxCollider2D>();
    }
    
    private void ChangeThornsState()
    {
        var fieldDown = new Rect(0, -GameManager.GameField.ScrolledLines+1, GameManager.GameField.Width, 2);
        var fieldBottom = new Rect(0, -GameManager.GameField.ScrolledLines+GameManager.GameField.Height-1, GameManager.GameField.Width, 2);
        
        if (!IsGrowing)
        {
            IsGrowing = true;
            gameObject.AddComponent<BoxCollider2D>();
        }
        else
        {
            IsGrowing = false;
            Destroy(gameObject.GetComponent<BoxCollider2D>());
        }

        if (!fieldDown.Contains(transform.position) && !fieldBottom.Contains(transform.position))
        {
            GameField.OnLineMoved -= ChangeThornsState;
            Destroy(gameObject.transform.parent.gameObject);
        }
    }
}
