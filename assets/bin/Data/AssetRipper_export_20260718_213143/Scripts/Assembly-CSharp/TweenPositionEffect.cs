using System.Collections;
using Holoville.HOTween;
using UnityEngine;

public class TweenPositionEffect : MonoBehaviour
{
	public float duration = 1f;

	public Vector3 finalPosition;

	public float initialDelay = 0.5f;

	public EaseType easeType = EaseType.EaseOutElastic;

	private Tweener positioningTweener;

	private IEnumerator Start()
	{
		if (initialDelay > 0f)
		{
			yield return new WaitForSeconds(initialDelay);
		}
		positioningTweener = base.transform.TweenLocalPosition(finalPosition, duration, easeType);
	}

	public void CompleteTween()
	{
		StopAllCoroutines();
		if (positioningTweener == null)
		{
			positioningTweener = base.transform.TweenLocalPosition(finalPosition, duration, easeType);
		}
		positioningTweener.Complete();
	}
}
