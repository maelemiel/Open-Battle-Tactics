using System.Collections;
using UnityEngine;

[RequireComponent(typeof(tk2dSpineAnimation))]
public class PlayRandomSpineAnimation : MonoBehaviour
{
	private tk2dSpineAnimation spineAnimation;

	private string[] availableAnimations;

	public float initialDelayMax = 1f;

	private void Awake()
	{
		spineAnimation = GetComponent<tk2dSpineAnimation>();
	}

	private IEnumerator Start()
	{
		yield return new WaitForSeconds(Random.Range(0f, initialDelayMax));
		if ((bool)spineAnimation)
		{
			availableAnimations = spineAnimation.GetAnimationNames();
		}
		if (availableAnimations != null)
		{
			string animationToPlay = availableAnimations[Random.Range(0, availableAnimations.Length)];
			spineAnimation.AnimationName = animationToPlay;
		}
	}
}
