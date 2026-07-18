using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(tk2dSpineAnimation))]
public class LoopRandomSpineAnimationWithDefault : MonoBehaviour
{
	private tk2dSpineAnimation spineAnimation;

	[SerializeField]
	private int defaultAnimationIndex;

	[SerializeField]
	private float defaultAnimationChance = 0.5f;

	[SerializeField]
	private List<string> disabledAnimations;

	private List<string> availableAnimations;

	private void Awake()
	{
		spineAnimation = GetComponent<tk2dSpineAnimation>();
		defaultAnimationChance = Mathf.Clamp01(defaultAnimationChance);
	}

	private void Start()
	{
		if ((bool)spineAnimation)
		{
			availableAnimations = new List<string>(spineAnimation.GetAnimationNames());
			availableAnimations.RemoveAll((string x) => disabledAnimations.Contains(x));
			defaultAnimationIndex = Mathf.Clamp(defaultAnimationIndex, 0, availableAnimations.Count - 1);
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
		if (availableAnimations.Count == 1)
		{
			spineAnimation.Reset();
			return;
		}
		string text = availableAnimations[defaultAnimationIndex];
		float num = Random.Range(0f, 1f);
		if (num > defaultAnimationChance)
		{
			while (text.Equals(availableAnimations[defaultAnimationIndex]))
			{
				text = availableAnimations[Random.Range(0, availableAnimations.Count)];
			}
		}
		this.spineAnimation.AnimationName = text;
	}

	private void OnDisable()
	{
		spineAnimation.AnimationComplete -= OnCurrentAnimationComplete;
	}
}
