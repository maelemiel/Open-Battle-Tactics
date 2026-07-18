using System;
using UnityEngine;

public class BuildingEffect : MonoBehaviour
{
	private const string ACTIVATION_ANIM_NAME = "Tank Building Welding Arm Deploy";

	private const string DEACTIVATION_ANIM_NAME = "Tank Building Welding Arm End";

	private const string IDLE_ANIM_NAME = "Tank Building Welding Arm Welding";

	private tk2dSpineAnimation spineAnimation;

	private tk2dSpineSkeleton spineSkeleton;

	private bool activating;

	private void Awake()
	{
		spineAnimation = GetComponent<tk2dSpineAnimation>();
		if (!spineAnimation)
		{
			Debug.LogError("No reference to Spine Animation!", this);
		}
		spineSkeleton = GetComponent<tk2dSpineSkeleton>();
		if (!spineSkeleton)
		{
			Debug.LogError("No reference to Spine Skeleton!", this);
		}
	}

	public void ActivateEffect()
	{
		spineAnimation.loop = false;
		spineAnimation.AnimationComplete += OnActivationAnimationIsCompleted;
		SetAnimation("Tank Building Welding Arm Deploy");
		activating = true;
	}

	public void ActivateIdleEffect()
	{
		if (!activating)
		{
			spineAnimation.AnimationComplete -= OnActivationAnimationIsCompleted;
			spineAnimation.loop = true;
			SetAnimation("Tank Building Welding Arm Welding");
		}
	}

	public void DeactivateEffect(Action<tk2dSpineAnimation> completionCallback)
	{
		SetAnimationWithCallback("Tank Building Welding Arm End", completionCallback);
	}

	private void OnActivationAnimationIsCompleted(tk2dSpineAnimation receivedSpineAnimation)
	{
		if (receivedSpineAnimation != spineAnimation)
		{
			Log.Debug("SpineAnimationIsCompleted is receiving a wrong component reference");
			return;
		}
		spineAnimation.AnimationComplete -= OnActivationAnimationIsCompleted;
		spineAnimation.loop = true;
		SetAnimation("Tank Building Welding Arm Welding");
		activating = false;
	}

	private void SetAnimation(string animationName)
	{
		spineAnimation.AnimationName = animationName;
	}

	private void SetAnimationWithCallback(string animationName, Action<tk2dSpineAnimation> completionCallback)
	{
		spineAnimation.ResetAnimationComplete();
		spineAnimation.loop = false;
		SetAnimation(animationName);
		if (completionCallback != null)
		{
			spineAnimation.AnimationComplete += completionCallback;
		}
	}

	private float GetAnimationLength()
	{
		return spineAnimation.state.Animation.Duration;
	}
}
