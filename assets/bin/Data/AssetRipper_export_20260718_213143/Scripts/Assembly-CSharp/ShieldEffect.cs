using System;
using UnityEngine;

public class ShieldEffect : MonoBehaviour
{
	private const string ACTIVATION_ANIM_NAME = "Initiate";

	private const string DEACTIVATION_ANIM_NAME = "Deactivation";

	private const string DESTRUCTION_ANIM_NAME = "Destruction";

	private const string IDLE_ANIM_NAME = "Idle";

	private tk2dSpineAnimation spineAnimation;

	private tk2dSpineSkeleton spineSkeleton;

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

	public void ActivateShield()
	{
		spineAnimation.AnimationName = "Initiate";
		spineAnimation.loop = false;
		spineAnimation.AnimationComplete += OnActivationAnimationIsCompleted;
		SetAnimation("Initiate");
		AudioTrigger.PowerUp.Play();
	}

	public void DeactivateShield(Action<tk2dSpineAnimation> completionCallback)
	{
		SetAnimationWithCallback("Deactivation", completionCallback);
		AudioTrigger.PowerDown.Play();
	}

	public void DestroyShield(Action<tk2dSpineAnimation> completionCallback)
	{
		SetAnimationWithCallback("Destruction", completionCallback);
		AudioTrigger.PowerDown.Play();
	}

	public void SetTarget(Transform targetTransform)
	{
		Vector3 vector = new Vector3(0f, -25f, -1f);
		base.transform.position = targetTransform.position + vector;
	}

	public void SetAlpha(float alphaValue)
	{
		spineSkeleton.skeleton.A = 0.8f + alphaValue * 0.2f;
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
		SetAnimation("Idle");
	}

	public void SetAnimation(string animationName)
	{
		spineAnimation.AnimationName = animationName;
	}

	private void SetAnimationWithCallback(string animationName, Action<tk2dSpineAnimation> completionCallback)
	{
		spineAnimation.loop = false;
		SetAnimation(animationName);
		if (completionCallback != null)
		{
			spineAnimation.AnimationComplete += completionCallback;
		}
	}
}
