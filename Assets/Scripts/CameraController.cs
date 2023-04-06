using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Animator _shakeAnimator;
    
    private void Start()
    {
        GameField.ScrollStateHasContinued += Shake;
        GameField.ScrollStateHasStopped += Shake;
    }

    private void Update()
    {
        //Updating the camera position
        Vector3 position;
        position.x = transform.position.x;
        position.y = transform.position.y - GameManager.GameField.ScrollSpeed * Time.deltaTime;
        position.z = -10;
        
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
