using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterAnimation : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.gameObject.TryGetComponent<ThornAuto>(out var thorn))
		{
			thorn.DestroyToPool();
		}
		else
		{
			Destroy(animator.gameObject, stateInfo.length);
		}
    }
}