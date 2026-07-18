using UnityEngine;

public class SceneTransitionTranslate : SceneTransition
{
	public Vector3 amount = new Vector3(0f, 0f, 0f);

	private Vector3 original;

	private Transform cachedTransform;

	public override void PrepareTransition()
	{
		original = base.gameObject.transform.localPosition;
		cachedTransform = base.gameObject.transform;
		base.PrepareTransition();
	}

	public override void UpdateTransition(float t)
	{
		if ((bool)cachedTransform)
		{
			cachedTransform.localPosition = new Vector3(EaseInExpo(t, original.x, amount.x), EaseInExpo(t, original.y, amount.y), EaseInExpo(t, original.z, amount.z));
		}
	}

	public static float BackEaseIn(float t, float b, float c, float d = 1f)
	{
		return c * (t /= d) * t * (2.70158f * t - 1.70158f) + b;
	}

	public static float EaseInExpo(float t, float b, float c, float d = 1f)
	{
		return c * Mathf.Pow(2f, 10f * (t / d - 1f)) + b;
	}
}
