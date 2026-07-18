using Holoville.HOTween;
using UnityEngine;

[RequireComponent(typeof(tk2dSpineAnimation))]
public class ExplosionEffect : MonoBehaviour
{
	[SerializeField]
	private int direction = 1;

	[SerializeField]
	private float tweenTime = 5f;

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

	public void SetAnimation(ExplosionTypes animationName)
	{
		spineAnimation.AnimationName = animationName.GetAnimationName();
	}

	public void PlayRandomAnimation()
	{
		int num = Random.Range(0, 4);
		SetAnimation((ExplosionTypes)num);
	}

	public void PlayLargeAnimation()
	{
		ExplosionTypes explosionTypes = ((!(Random.Range(0f, 1f) > 0.5f)) ? ExplosionTypes.BIG_B : ExplosionTypes.BIG_A);
		SetAnimation(explosionTypes);
	}

	public void PlaySmallAnimation()
	{
		ExplosionTypes explosionTypes = ((!(Random.Range(0f, 1f) > 0.5f)) ? ExplosionTypes.SMALL_B : ExplosionTypes.SMALL_A);
		SetAnimation(explosionTypes);
	}

	public void SetAlpha(float alphaValue)
	{
		spineSkeleton.skeleton.A = 0.8f + alphaValue * 0.2f;
	}

	public void MoveOutOfScreen()
	{
		base.transform.TweenLocalXPosition(base.transform.localPosition.x + (float)(1000 * direction), tweenTime, EaseType.Linear);
	}
}
