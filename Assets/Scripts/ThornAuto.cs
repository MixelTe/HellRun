using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ThornAuto : MonoBehaviour, IPollable
{
	private Animator _animator;

	private void Awake()
	{
		_animator = GetComponent<Animator>();
	}

	bool IPollable.IsDestroyedToPool { get; set; }

	void IPollable.InitAsNew() 
	{
		_animator.SetTrigger("Reset");
	}

	public void SetAnimSpeed(float speed)
	{
		_animator.SetFloat("Speed", speed);
	}

	public void PlaySound()
	{
		GameManager.SoundPlayer.PlayThornsSound();
	}
}