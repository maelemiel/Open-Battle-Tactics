using System.Collections;
using UnityEngine;

[RequireComponent(typeof(tk2dSpineAnimation))]
public class LoopWithTimeSpineAnimation : MonoBehaviour
{
	private tk2dSpineAnimation spineAnimation;

	private string[] availableAnimations;

	public float maxWaitTime = 5f;

	private void Awake()
	{
		spineAnimation = GetComponent<tk2dSpineAnimation>();
	}

	private void OnEnable()
	{
		spineAnimation.AnimationComplete += OnCurrentAnimationComplete;
	}

	private void OnCurrentAnimationComplete(tk2dSpineAnimation spineAnimation)
	{
		StartCoroutine(ResetAnimationWithDelay(Random.Range(0f, maxWaitTime)));
	}

	private IEnumerator ResetAnimationWithDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		spineAnimation.Reset();
	}

	private void OnDisable()
	{
		spineAnimation.AnimationComplete -= OnCurrentAnimationComplete;
	}
}
