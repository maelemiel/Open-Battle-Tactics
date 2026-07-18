using Holoville.HOTween;
using UnityEngine;

public class ItemsMixerBouncerEffect : MonoBehaviour
{
	private tk2dBaseSprite bouncerSprite;

	private Sequence currentColorSequence;

	private Sequence currentScaleSequence;

	public float effectDuration = 0.4f;

	public Color initialColor;

	private Transform cachedTransform;

	private void Awake()
	{
		bouncerSprite = GetComponent<tk2dBaseSprite>();
		cachedTransform = base.transform;
	}

	public void PlayEffect(float finalScale)
	{
		if (!bouncerSprite)
		{
			return;
		}
		bouncerSprite.Alpha = 0f;
		Color currentColor = initialColor;
		currentColor.a = 1f;
		if (currentColorSequence != null)
		{
			currentColorSequence.Kill();
		}
		if (currentScaleSequence != null)
		{
			currentScaleSequence.Kill();
		}
		currentColorSequence = SimpleTween.StartInAndOut(0f, 1f, effectDuration, EaseType.EaseOutExpo, delegate(float val)
		{
			currentColor.a = val;
			if ((bool)bouncerSprite)
			{
				bouncerSprite.color = currentColor;
			}
		});
		currentScaleSequence = SimpleTween.StartInAndOut(1f, 0.35f, effectDuration, EaseType.EaseOutExpo, delegate(float val)
		{
			if ((bool)cachedTransform)
			{
				cachedTransform.localScale = Vector3.one * finalScale * val;
			}
		});
	}
}
