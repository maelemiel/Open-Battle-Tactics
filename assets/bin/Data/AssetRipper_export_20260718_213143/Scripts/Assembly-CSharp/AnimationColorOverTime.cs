using UnityEngine;

[RequireComponent(typeof(tk2dSpineAnimation))]
public class AnimationColorOverTime : MonoBehaviour
{
	public AnimationCurve colorFadeCurve;

	public Color initialColor = Color.white;

	public Color finalColor = Color.white;

	private tk2dSpineAnimation spineAnimation;

	private tk2dSpineSkeleton spineAnimationSkeleton;

	private float animationTime;

	private float currentProgress;

	private Color nextColor = Color.white;

	private void Start()
	{
		spineAnimation = GetComponent<tk2dSpineAnimation>();
		spineAnimationSkeleton = spineAnimation.Skeleton;
	}

	private void Update()
	{
		if (spineAnimation.state != null)
		{
			animationTime += Time.deltaTime;
			currentProgress = GetAnimationProgress();
			nextColor = Color.Lerp(initialColor, finalColor, colorFadeCurve.Evaluate(animationTime));
			SetSkeletonColor(nextColor);
		}
	}

	private void LateUpdate()
	{
		if (currentProgress >= 1f)
		{
			animationTime = 0f;
		}
	}

	private void SetSkeletonColor(Color color)
	{
		spineAnimationSkeleton.skeleton.R = color.r;
		spineAnimationSkeleton.skeleton.G = color.g;
		spineAnimationSkeleton.skeleton.B = color.b;
		spineAnimationSkeleton.skeleton.A = color.a;
	}

	private float GetAnimationProgress()
	{
		return animationTime / spineAnimation.state.Animation.Duration;
	}
}
