using System.Collections;
using Holoville.HOTween;
using Spine;
using UnityEngine;

public class IntelParachuteController : MonoBehaviour
{
	[SerializeField]
	private GameObject dice;

	[SerializeField]
	private GameObject boostValue;

	[SerializeField]
	private tk2dTextMesh boostValueText;

	[SerializeField]
	private tk2dSpineAnimation firstStrikeDiceEffect;

	private string DICE_BONE_NAME = "Dice Face";

	private Bone diceBone;

	private tk2dSpineAnimation parachuteAnimation;

	private Vector3 _origScale;

	public tk2dTextMesh BoosValueTextMesh
	{
		get
		{
			return boostValueText;
		}
	}

	private void OnEnable()
	{
		firstStrikeDiceEffect.gameObject.SetActive(false);
		tk2dSpineAnimation component = GetComponent<tk2dSpineAnimation>();
		diceBone = component.Skeleton.skeleton.FindBone(DICE_BONE_NAME);
		dice.transform.position = new Vector3(0f, 800f, 0f);
		_origScale = Vector3.one;
		dice.transform.localScale = _origScale;
	}

	public void SetBoostValue(int value)
	{
		boostValueText.text = value.ToString();
		boostValueText.Commit();
	}

	public void ExpandBoostDice()
	{
		StartCoroutine(PlayAnimation(boostValue.transform.localPosition));
	}

	private void Update()
	{
		if (diceBone.WorldX != 0f && diceBone.WorldY != 0f)
		{
			dice.transform.localPosition = new Vector3(diceBone.WorldX, diceBone.WorldY, 0f);
			dice.transform.localRotation = Quaternion.Euler(0f, 0f, diceBone.WorldRotation);
		}
		firstStrikeDiceEffect.transform.localPosition = new Vector3(0f, 0f, 0f);
	}

	private IEnumerator PlayAnimation(Vector3 position)
	{
		yield return new WaitForSeconds(2f);
		ExpandActiveDieFace(2.5f, 0.3f);
		yield return new WaitForSeconds(0.2f);
		ActivateFirstStrikeEffect();
		yield return new WaitForSeconds(0.2f);
		RestoreActiveDieFace(1f);
		yield return new WaitForSeconds(2f);
		firstStrikeDiceEffect.gameObject.SetActive(false);
	}

	private void ActivateFirstStrikeEffect()
	{
		firstStrikeDiceEffect.gameObject.SetActive(true);
		firstStrikeDiceEffect.state.Time = 0f;
	}

	private void ExpandActiveDieFace(float scaleFactor, float animTime)
	{
		Vector3 localPosition = dice.transform.localPosition;
		Vector3 vector = Vector3.one * scaleFactor;
		HOTween.To(dice.transform, animTime, new TweenParms().Ease(EaseType.EaseOutSine).Prop("localScale", vector).Prop("localPosition", localPosition + new Vector3(0f, 0f, -30f)));
		localPosition = firstStrikeDiceEffect.transform.localPosition;
		HOTween.To(firstStrikeDiceEffect.transform, animTime, new TweenParms().Ease(EaseType.EaseOutSine).Prop("localScale", vector).Prop("localPosition", localPosition + new Vector3(0f, 0f, -30f)));
	}

	private void RestoreActiveDieFace(float animTime)
	{
		HOTween.To(dice.transform, animTime, new TweenParms().Ease(EaseType.Linear).Prop("localScale", _origScale).Prop("localPosition", dice.transform.localPosition));
		HOTween.To(firstStrikeDiceEffect.transform, animTime, new TweenParms().Ease(EaseType.Linear).Prop("localScale", _origScale).Prop("localPosition", firstStrikeDiceEffect.transform.localPosition));
	}
}
