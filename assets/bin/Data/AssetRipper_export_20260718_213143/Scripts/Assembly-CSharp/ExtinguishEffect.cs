using System;
using UnityEngine;

public class ExtinguishEffect : MonoBehaviour
{
	public enum ExtinguishIndicatorType
	{
		LEFT = 0,
		TOP = 1,
		RIGHT = 2,
		NONE = 3
	}

	private const string DEPLOY_ANIM_NAME = "Extinguisher Deploy";

	private const string LAND_ANIM_NAME_TOP = "Extinguisher Land Top";

	private const string LAND_ANIM_NAME_LEFT = "Extinguisher Land Left";

	private const string LAND_ANIM_NAME_RIGHT = "Extinguisher Land Right";

	private const string LOOP_ANIM_NAME_TOP = "Extinguisher Idle Top";

	private const string LOOP_ANIM_NAME_LEFT = "Extinguisher Idle Left";

	private const string LOOP_ANIM_NAME_RIGHT = "Extinguisher Idle Right";

	private const string EXPLOSION_ANIM_NAME_TOP = "Extinguisher Explosion Top";

	private const string EXPLOSION_ANIM_NAME_LEFT = "Extinguisher Explosion Left";

	private const string EXPLOSION_ANIM_NAME_RIGHT = "Extinguisher Explosion Right";

	private tk2dSpineAnimation spineAnimation;

	private tk2dSpineSkeleton spineSkeleton;

	private ExtinguishIndicatorType currentType = ExtinguishIndicatorType.NONE;

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

	public void SetTarget(Transform targetTransform)
	{
		Vector3 vector = Vector3.zero;
		switch (currentType)
		{
		case ExtinguishIndicatorType.LEFT:
			vector = new Vector3(-90f, -25f, -1f);
			break;
		case ExtinguishIndicatorType.TOP:
			vector = new Vector3(0f, -75f, -1f);
			break;
		case ExtinguishIndicatorType.RIGHT:
			vector = new Vector3(90f, -25f, -1f);
			break;
		}
		base.transform.position = targetTransform.position + vector;
	}

	public void SetExtinguishEffectType(ExtinguishIndicatorType type)
	{
		currentType = type;
	}

	public void ActivateEffect(ExtinguishIndicatorType type, Transform target)
	{
		SetExtinguishEffectType(type);
		SetTarget(target);
		spineAnimation.AnimationName = GetLandAnimationName();
		spineAnimation.loop = false;
		spineAnimation.AnimationComplete += OnActivationAnimationIsCompleted;
	}

	public void PlayExplosionEffect(Action<tk2dSpineAnimation> completionCallback)
	{
		SetAnimationWithCallback(GetExplosionAnimationName(), completionCallback);
		AudioTrigger.Extinguish_Use.Play();
	}

	public void PlayDeploy()
	{
		SetAnimation("Extinguisher Deploy");
		AudioTrigger.Extinguish_Launch.Play();
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
		SetAnimation(GetLoopAnimationName());
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

	private string GetLandAnimationName()
	{
		switch (currentType)
		{
		case ExtinguishIndicatorType.LEFT:
			return "Extinguisher Land Left";
		case ExtinguishIndicatorType.TOP:
			return "Extinguisher Land Top";
		case ExtinguishIndicatorType.RIGHT:
			return "Extinguisher Land Right";
		default:
			return string.Empty;
		}
	}

	private string GetLoopAnimationName()
	{
		switch (currentType)
		{
		case ExtinguishIndicatorType.LEFT:
			return "Extinguisher Idle Left";
		case ExtinguishIndicatorType.TOP:
			return "Extinguisher Idle Top";
		case ExtinguishIndicatorType.RIGHT:
			return "Extinguisher Idle Right";
		default:
			return string.Empty;
		}
	}

	private string GetExplosionAnimationName()
	{
		switch (currentType)
		{
		case ExtinguishIndicatorType.LEFT:
			return "Extinguisher Explosion Right";
		case ExtinguishIndicatorType.TOP:
			return "Extinguisher Explosion Top";
		case ExtinguishIndicatorType.RIGHT:
			return "Extinguisher Explosion Left";
		default:
			return string.Empty;
		}
	}
}
