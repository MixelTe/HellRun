using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Animator _shakeAnimator;
    
    private void Start()
    {
        GameField.ScrollStopped += Shake;
    }

    private void LateUpdate()
    {
        var position = transform.position;
        position.y =  -GameManager.GameField.ScrolledLines - GameManager.GameField.Scroll;
        
        transform.position = position;
    }

    private void Shake()
    {
        if (!GameManager.GameField.Scroling)
        {
            _shakeAnimator.SetTrigger("ShakeTriger");
        }
    }
}
