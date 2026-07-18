using System.Collections;
using Holoville.HOTween;
using UnityEngine;

public class EaseInEffect : MonoBehaviour
{
	public float duration = 1f;

	public float finalScale = 1f;

	public float initialDelay = 0.5f;

	public EaseType easeType = EaseType.EaseOutElastic;

	public bool setInitialScaleToZero;

	private IEnumerator Start()
	{
		if (setInitialScaleToZero)
		{
			base.transform.localScale = Vector3.zero;
		}
		if (initialDelay > 0f)
		{
			yield return new WaitForSeconds(initialDelay);
		}
		base.transform.TweenLocalScale(finalScale, duration, easeType);
	}
}
