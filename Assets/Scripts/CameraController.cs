using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameField _gameField;
    [SerializeField] private Animator _shakeAnimator;
    
    private void Start()
    {
        GameField.ScrollStateHasContinued += Shake;
        GameField.ScrollStateHasStopped += Shake;
    }

    private void Update()
    {
        Vector3 position;
        position.x = transform.position.x;
        position.y = transform.position.y - _gameField._scrollSpeed * Time.deltaTime;
        position.z = -10;
        
        transform.position = position;
    }

    private void Shake()
    {
        if (!_gameField._scroling)
        {
            _shakeAnimator.SetTrigger("ShakeTriger");
        }
    }
}
