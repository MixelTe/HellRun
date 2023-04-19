using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
	public event Action OnColected;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.TryGetComponent<Player>(out var player))
		{
			GameManager.Score.AddCoin();
			OnColected?.Invoke();
			GameManager.SoundPlayer.PlayCoinSound();
			Destroy(gameObject);
		}
	}
}
