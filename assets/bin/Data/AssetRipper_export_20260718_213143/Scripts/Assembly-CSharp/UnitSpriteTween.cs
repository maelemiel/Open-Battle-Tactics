using Holoville.HOTween;
using UnityEngine;

public class UnitSpriteTween : MonoBehaviour
{
	private float[] bounceCoordinates = new float[7] { 3f, 2f, 0f, 2f, 0f, 0f, 0f };

	[HideInInspector]
	public Vector3 kickBackOffset = Vector3.zero;

	private float rand;

	private Vector3 originalPosition;

	private tk2dSprite sprite;

	private Vector3 positionOffset = Vector3.zero;

	private void Start()
	{
		rand = Random.value * 100f;
		originalPosition = base.transform.localPosition;
		sprite = GetComponent<tk2dSprite>();
	}

	private void Update()
	{
		int num = (int)((Time.time + rand) * 24f % (float)bounceCoordinates.Length);
		SetYOff(bounceCoordinates[num]);
		positionOffset += kickBackOffset;
		ApplyOffsetToPosition();
		ResetOffset();
	}

	public void KickBack()
	{
		Sequence sequence = new Sequence();
		sequence.Append(HOTween.To(this, 0.25f, new TweenParms().Prop("kickBackOffset", new Vector3(-50f, 0f, 0f))));
		sequence.Append(HOTween.To(this, 1f, new TweenParms().Prop("kickBackOffset", Vector3.zero)));
		sequence.Play();
	}

	private void SetYOff(float yOff)
	{
		positionOffset.y += yOff;
	}

	private void ApplyOffsetToPosition()
	{
		if ((bool)sprite)
		{
			positionOffset.x *= sprite.scale.x;
		}
		base.transform.localPosition = originalPosition + positionOffset;
	}

	private void ResetOffset()
	{
		positionOffset = Vector3.zero;
	}
}
