using System.Collections;
using Holoville.HOTween;
using UnityEngine;

public class UnitMultiTeamReportView : MonoBehaviour
{
	private const int MAX_WIDTH_ALLOWED = 205;

	public UnitProxy unitProxy;

	public Transform platformTransform;

	public tk2dTextMesh eventPointsLabel;

	[Range(1f, 10f)]
	public float eventPointsUpdateSpeed;

	public Vector3 platformInitialLocalPosition;

	public Vector3 platformFinalLocalPosition;

	public float pointsVerticalOffset = 120f;

	private Tweener localPositionTweener;

	private Tweener scaleTweener;

	private Tweener eventPointsTweener;

	private float animationTime = 1f;

	private float sizeRatio = 1f;

	private AudioManager.Sfx platformSound;

	private bool isHidden;

	public float AnimationTime
	{
		get
		{
			return animationTime;
		}
		set
		{
			animationTime = value;
		}
	}

	public void ShowUnitImmediate(UserMultiTeamReportUnit multiTeamReportUnit)
	{
		StopAllCoroutines();
		if (localPositionTweener != null)
		{
			localPositionTweener.Kill();
		}
		if (scaleTweener != null)
		{
			scaleTweener.Kill();
		}
		if (eventPointsTweener != null)
		{
			eventPointsTweener.Kill();
		}
		platformTransform.localPosition = platformFinalLocalPosition;
		unitProxy.transform.localScale = Vector3.one;
		if (multiTeamReportUnit.bonusEventPointsEarned > 0)
		{
			eventPointsLabel.gameObject.SetActive(true);
			eventPointsLabel.Alpha = 1f;
			eventPointsLabel.text = "+" + multiTeamReportUnit.bonusEventPointsEarned;
			eventPointsLabel.transform.SetLocalYPosition(pointsVerticalOffset);
		}
		else
		{
			eventPointsLabel.gameObject.SetActive(false);
		}
		if (platformSound != null)
		{
			platformSound.Stop(0f);
		}
		StartCoroutine(unitProxy.ChangeAssetCoroutine(multiTeamReportUnit.unitLevelProgressionDataModel.assetBundleId));
		isHidden = false;
	}

	public IEnumerator ShowUnitWithMultiTeamReportUnit(UserMultiTeamReportUnit multiTeamReportUnit)
	{
		if (localPositionTweener != null)
		{
			localPositionTweener.Kill();
		}
		platformTransform.localPosition = platformInitialLocalPosition;
		unitProxy.transform.localScale = Vector3.one;
		yield return StartCoroutine(unitProxy.ChangeAssetCoroutine(multiTeamReportUnit.unitLevelProgressionDataModel.assetBundleId));
		while (!unitProxy.AssetReady)
		{
			yield return 0;
		}
		tk2dBaseSprite unitSprite = unitProxy.Prefab.GetComponent<tk2dBaseSprite>();
		if ((bool)unitSprite)
		{
			Bounds bounds = unitSprite.GetUntrimmedBounds();
			sizeRatio = 1f;
			if (bounds.size.x > 205f)
			{
				sizeRatio = 205f / bounds.size.x;
			}
			if (scaleTweener != null)
			{
				scaleTweener.Kill();
			}
			unitProxy.transform.localScale = Vector3.one * sizeRatio;
			scaleTweener = unitProxy.transform.TweenLocalScale(1f, animationTime);
		}
		if (platformSound != null)
		{
			platformSound.Stop(0f);
		}
		platformSound = AudioTrigger.MovingPlatform.Play();
		localPositionTweener = platformTransform.TweenLocalPosition(platformFinalLocalPosition, animationTime);
		isHidden = false;
	}

	public IEnumerator HideUnit()
	{
		if (isHidden)
		{
			yield break;
		}
		if (localPositionTweener != null)
		{
			localPositionTweener.Kill();
		}
		if (platformSound != null)
		{
			platformSound.Stop(0f);
		}
		platformSound = AudioTrigger.MovingPlatform.Play();
		if (sizeRatio != 1f)
		{
			if (scaleTweener != null)
			{
				scaleTweener.Kill();
			}
			unitProxy.transform.localScale = Vector3.one;
			scaleTweener = unitProxy.transform.TweenLocalScale(sizeRatio, animationTime);
		}
		platformTransform.localPosition = platformFinalLocalPosition;
		localPositionTweener = platformTransform.TweenLocalPosition(platformInitialLocalPosition, animationTime);
		if (eventPointsLabel.gameObject.activeSelf)
		{
			if (eventPointsTweener != null)
			{
				eventPointsTweener.Kill();
			}
			eventPointsTweener = eventPointsLabel.TweenAlpha(0f, 0.5f);
		}
		isHidden = true;
		yield return new WaitForSeconds(animationTime);
	}

	public IEnumerator ShowUnitPoints(float eventPoints, float delay = 0f)
	{
		if (isHidden)
		{
			yield break;
		}
		if (delay > 0f)
		{
			yield return new WaitForSeconds(delay);
		}
		if (eventPointsTweener != null)
		{
			eventPointsTweener.Kill();
		}
		Transform eventPointsLabelTransform = eventPointsLabel.transform;
		eventPointsLabel.gameObject.SetActive(true);
		eventPointsLabelTransform.SetLocalYPosition(0f);
		eventPointsLabel.Alpha = 0f;
		eventPointsTweener = eventPointsLabel.TweenAlpha(1f, 0.5f);
		eventPointsTweener = SimpleTween.Start(0f, 1f, 0.5f, delegate(float val)
		{
			if ((bool)eventPointsLabel)
			{
				eventPointsLabel.Alpha = val;
				eventPointsLabelTransform.SetLocalYPosition(Mathf.Lerp(0f, pointsVerticalOffset, val));
			}
		});
		if (eventPointsUpdateSpeed <= 0f)
		{
			eventPointsUpdateSpeed = 1f;
		}
		eventPointsLabel.text = "+" + eventPoints;
	}

	public void Reset()
	{
		if (localPositionTweener != null)
		{
			localPositionTweener.Kill();
		}
		if (scaleTweener != null)
		{
			scaleTweener.Kill();
		}
		if (eventPointsTweener != null)
		{
			eventPointsTweener.Kill();
		}
		if (platformSound != null)
		{
			platformSound.Stop(0f);
		}
		platformTransform.localPosition = platformInitialLocalPosition;
		unitProxy.transform.localScale = Vector3.one;
		eventPointsLabel.Alpha = 0f;
		eventPointsLabel.transform.SetLocalYPosition(0f);
		eventPointsLabel.gameObject.SetActive(false);
		isHidden = true;
	}

	private void OnDestroy()
	{
		if (localPositionTweener != null)
		{
			localPositionTweener.Kill();
		}
	}
}
