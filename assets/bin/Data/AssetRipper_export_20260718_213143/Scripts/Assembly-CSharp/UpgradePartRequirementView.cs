using System;
using System.Collections;
using UnityEngine;

public class UpgradePartRequirementView : MonoBehaviour
{
	[SerializeField]
	private Transform partObject;

	[SerializeField]
	private tk2dSprite upSprite;

	[SerializeField]
	private GameObject priceLabel;

	[SerializeField]
	private GameObject plusIcon;

	[SerializeField]
	private GameObject requirementText;

	[SerializeField]
	private tk2dSpineAnimation glowAnimtion;

	[SerializeField]
	private CameraShake cameraShake;

	public Transform partGlow;

	public AnimationCurve outlineGrowth;

	public AnimationCurve shrinkCurve;

	public float time;

	public float sizeIncrease = 0.5f;

	public float partCommitAnimLength = 0.5f;

	private bool animating;

	[ContextMenu("Play Anim")]
	public void StartReadyAnimation()
	{
		if (base.gameObject.activeInHierarchy && base.gameObject.activeSelf)
		{
			StopAnimations();
			StartCoroutine(PlayPartReadyAnimation());
		}
	}

	public IEnumerator PlayPartReadyAnimation()
	{
		if (animating)
		{
			yield break;
		}
		animating = true;
		float speed = 5f;
		float sinuloid = 0.1f;
		partGlow.gameObject.SetActive(true);
		while (true)
		{
			time += Time.deltaTime;
			if (partObject != null)
			{
				partObject.localScale = Vector3.Max(Vector3.one * 0.8f + Vector3.one * (0.5f + Mathf.Sin(time * speed)) * sinuloid, Vector3.forward);
			}
			if (partGlow != null)
			{
				partGlow.localScale = Vector3.one * 0.75f + Vector3.one * (0.5f + Mathf.Sin(time)) * 0.5f;
			}
			yield return new WaitForEndOfFrame();
		}
	}

	[ContextMenu("HOTDOG!")]
	public void PlayUpgradeAnim()
	{
		StopAnimations();
		StartCoroutine(PlayUpgradePartAnimation());
	}

	public IEnumerator PlayUpgradePartAnimation(Action onComplete = null)
	{
		if (!animating)
		{
			animating = true;
			plusIcon.SetActive(false);
			requirementText.SetActive(false);
			partGlow.gameObject.SetActive(true);
			partObject.gameObject.SetActive(true);
			priceLabel.SetActive(false);
			upSprite.SetSprite("LevelUp_Blue_Up");
			float length = (time = partCommitAnimLength);
			while (time > 0f)
			{
				partObject.transform.localScale = Vector3.one + sizeIncrease * Vector3.one * outlineGrowth.Evaluate(1f - time / length);
				time -= Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
			priceLabel.SetActive(true);
			partGlow.gameObject.SetActive(false);
			glowAnimtion.gameObject.SetActive(true);
			partObject.gameObject.SetActive(false);
			upSprite.SetSprite("LevelUp_Green_Up");
			cameraShake.Shake();
			glowAnimtion.loop = false;
			yield return StartCoroutine(glowAnimtion.PlayAnimCoroutine("Rare Glow Loop"));
			glowAnimtion.gameObject.SetActive(false);
			if (onComplete != null)
			{
				onComplete();
			}
		}
	}

	public IEnumerator PlayUpgradeAnimation(float length, Action onComplete = null)
	{
		if (!animating)
		{
			animating = true;
			glowAnimtion.loop = false;
			glowAnimtion.Reset();
			glowAnimtion.gameObject.SetActive(true);
			Coroutine routine = StartCoroutine(glowAnimtion.PlayAnimCoroutine("Rare Glow Loop"));
			time = length;
			while (time >= 0f)
			{
				priceLabel.transform.localScale = Vector3.one * shrinkCurve.Evaluate(1f - time / length);
				time -= Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
			priceLabel.transform.localScale = Vector3.one;
			glowAnimtion.gameObject.SetActive(false);
			if (onComplete != null)
			{
				onComplete();
			}
			animating = false;
		}
	}

	public void StopAnimations()
	{
		StopAllCoroutines();
		animating = false;
	}

	public void DisableIncompleteItems()
	{
		partGlow.gameObject.SetActive(false);
		partObject.gameObject.SetActive(false);
	}
}
