using System.Collections;
using UnityEngine;

public class Chain : MonoBehaviour
{
	[SerializeField] private BoxCollider2D _collider;
	[SerializeField] private SpriteRenderer _renderer;
	[SerializeField] private Transform _chain;
	[SerializeField] private float _strikeTime;
	[SerializeField] private AnimationCurve _strikeScale;
	[SerializeField] private AnimationCurve _strikeAlpha;
	[SerializeField] private float _shakeSpeed;
	[SerializeField] private float _shakeAmplitude;
	[SerializeField] private Sprite[] _appearSprites;

	public void Strike(float duration)
	{
		StartCoroutine(StrikeImp(duration));
	}

	private IEnumerator StrikeImp(float duration)
	{
		_collider.enabled = false;
		yield return Fade(duration);
		
		yield return StrikeAnim(_strikeTime);

		Destroy(gameObject);
	}

	private IEnumerator Fade(float duration)
	{
		var color = _renderer.color;
		var pos = _chain.localPosition;
		var rand = Random.value * Mathf.PI;
		var shakeX = Random.value < 0.5f;
		if (_appearSprites.Length > 0)
			_renderer.sprite = _appearSprites[0];
		for (float t = 0; t < 1; t += Time.fixedDeltaTime / duration)
		{
			color.a = Mathf.Lerp(0, 0.5f, t);
			_renderer.color = color;

			var shake = Mathf.Sin(t * _shakeSpeed + rand) * _shakeAmplitude;
			if (shakeX) pos.x = shake;
			else pos.y = shake;
			_chain.localPosition = pos;

			if (_appearSprites.Length > 0)
				_renderer.sprite = _appearSprites[Mathf.FloorToInt(t * (_appearSprites.Length - 1))];

			yield return new WaitForFixedUpdate();
		}
		color.a = 0;
		_renderer.color = color;
		if (_appearSprites.Length > 0)
			_renderer.sprite = _appearSprites[^1];	
	}

	private IEnumerator StrikeAnim(float duration)
	{
		var color = _renderer.color;
		var scale = transform.localScale;
		_collider.enabled = true;

		for (float t = 0; t < 1; t += Time.deltaTime / duration)
		{
			color.a = _strikeAlpha.Evaluate(t);
			_renderer.color = color;

			scale.x = _strikeScale.Evaluate(t);
			transform.localScale = scale;

			if (t > 0.5f)
				_collider.enabled = false;

			yield return new WaitForEndOfFrame();
		}
		_collider.enabled = false;
	}
}
