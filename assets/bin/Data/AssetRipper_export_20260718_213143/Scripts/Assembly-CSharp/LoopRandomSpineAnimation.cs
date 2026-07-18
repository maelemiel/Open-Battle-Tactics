using UnityEngine;

[RequireComponent(typeof(tk2dSpineAnimation))]
public class LoopRandomSpineAnimation : MonoBehaviour
{
	private tk2dSpineAnimation spineAnimation;

	private string[] availableAnimations;

	private void Awake()
	{
		spineAnimation = GetComponent<tk2dSpineAnimation>();
	}

	private void Start()
	{
		if ((bool)spineAnimation)
		{
			availableAnimations = spineAnimation.GetAnimationNames();
		}
	}

	private void OnEnable()
	{
		spineAnimation.AnimationComplete += OnCurrentAnimationComplete;
	}

	private void OnCurrentAnimationComplete(tk2dSpineAnimation spineAnimation)
	{
		if (availableAnimations == null)
		{
			return;
		}
		if (availableAnimations.Length == 1)
		{
			spineAnimation.Reset();
			return;
		}
		string text = this.spineAnimation.animationName;
		while (text.Equals(spineAnimation.animationName))
		{
			text = availableAnimations[Random.Range(0, availableAnimations.Length)];
		}
		this.spineAnimation.AnimationName = text;
	}

	private void OnDisable()
	{
		spineAnimation.AnimationComplete -= OnCurrentAnimationComplete;
	}
}
