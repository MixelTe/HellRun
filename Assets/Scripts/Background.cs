using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Background : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
	{
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

	private void Update()
    {
        var h = _camera.orthographicSize;
        var w = _camera.aspect * h;
        _spriteRenderer.size = new Vector2(w * 2 + 1, h * 2 + 1);
    }
}
