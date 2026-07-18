using System.Collections;
using UnityEngine;

[RequireComponent(typeof(tk2dSpineAnimation))]
public class DelayPlaySpineAnimation : MonoBehaviour
{
	private tk2dSpineAnimation spineAnimation;

	public float delay = 0.5f;

	private void Awake()
	{
		spineAnimation = GetComponent<tk2dSpineAnimation>();
		spineAnimation.enabled = false;
	}

	private void Start()
	{
		StartCoroutine(PlayAnimationWithDelay(delay));
	}

	private IEnumerator PlayAnimationWithDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		spineAnimation.enabled = true;
	}
}
