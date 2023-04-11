using System;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;

public class Thorn : MonoBehaviour
{
    public int GrowingState = 0;
    
    private void Start()
    { 
        GameField.OnLineMoved += ChangeThornsStateOnMovedLine;
        ChangeThornsState(GrowingState);
    }
    
    private void ChangeThornsStateOnMovedLine()
    {
        GrowingState++;
        ChangeThornsState(GrowingState);
    }

    private void ChangeThornsState(int growingState)
    {
        if (growingState == 0)
        {
            GetComponent<BoxCollider2D>().enabled = false;
        }
        else if (growingState == 1)
        {
            GetComponent<BoxCollider2D>().enabled = true;
        }
        else if (growingState == 2)
        {
            GetComponent<BoxCollider2D>().enabled = false;
        }
        else
        {
            GameField.OnLineMoved -= ChangeThornsStateOnMovedLine;
            Destroy(gameObject);
        }
    }
}
