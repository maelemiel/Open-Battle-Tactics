using System;
using System.Collections;
using Holoville.HOTween;
using Spine;
using UnityEngine;

public class IonStrikeEffect : MonoBehaviour
{
	public const string ACTIVATION_ANIM_NAME = "Ion Initiate 1";

	public const string ACTIVATION_ANIM_NAME_2 = "Ion Initiate 2";

	public const string EXPLOSION_ANIM_NAME = "Ion Strike";

	public const string IDLE_ANIM_NAME = "Ion Loop";

	private float JAMMED_ANIMATION_TIME = 1f;

	private tk2dSpineAnimation spineAnimation;

	private tk2dSpineSkeleton spineSkeleton;

	private Vector3 OFFSET_POSITION_EFFECT = new Vector3(0f, -50f, -5f);

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

	public float GetAnimationDuration(string animationName)
	{
		Spine.Animation animation = spineAnimation.state.Data.SkeletonData.FindAnimation(animationName);
		float result = 0f;
		if (animation != null)
		{
			result = animation.Duration;
		}
		return result;
	}

	public float SetAnimationTime(string animationName)
	{
		Spine.Animation animation = spineAnimation.state.Data.SkeletonData.FindAnimation(animationName);
		float result = GetAnimationDuration(animationName);
		if (animation != null)
		{
			result = animation.Duration;
		}
		return result;
	}

	public void SetAnimation(string animationName, bool loop)
	{
		spineAnimation.loop = loop;
		spineAnimation.AnimationName = animationName;
	}

	public void ActivateEffect()
	{
		SetAnimation("Ion Initiate 2", false);
		spineAnimation.AnimationComplete += OnActivationAnimationIsCompleted;
	}

	public void ExplosionEffect()
	{
		SetAnimation("Ion Strike", false);
	}

	public void DeactivateEffect(Action<tk2dSpineAnimation> completionCallback)
	{
		SetAnimationWithCallback("Ion Initiate 2", -1f, completionCallback);
	}

	public void SetTarget(Transform targetTransform)
	{
		base.transform.position = targetTransform.position + OFFSET_POSITION_EFFECT;
	}

	public IEnumerator JammedAnimation(Transform target)
	{
		HOTween.To(base.transform, JAMMED_ANIMATION_TIME, new TweenParms().Ease(EaseType.EaseOutSine).Prop("position", target.position + OFFSET_POSITION_EFFECT));
		yield return new WaitForSeconds(JAMMED_ANIMATION_TIME);
	}

	private void OnActivationAnimationIsCompleted(tk2dSpineAnimation receivedSpineAnimation)
	{
		if (receivedSpineAnimation != spineAnimation)
		{
			Log.Debug("SpineAnimationIsCompleted is receiving a wrong component reference");
			return;
		}
		spineAnimation.AnimationComplete -= OnActivationAnimationIsCompleted;
		SetAnimation("Ion Loop", true);
	}

	private void SetAnimationWithCallback(string animationName, float speed, Action<tk2dSpineAnimation> completionCallback)
	{
		SetAnimation(animationName, false);
		if (completionCallback != null)
		{
			spineAnimation.AnimationComplete += completionCallback;
		}
	}
}
