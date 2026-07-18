using System.Collections;
using UnityEngine;

public class MultiTeamInitialPopUpController : PopupController
{
	[SerializeField]
	private MovingObjectController popupContentController;

	[SerializeField]
	private UnitProxy[] units;

	[SerializeField]
	private tk2dTextMesh bonusDescription;

	[SerializeField]
	private tk2dTextMesh bonusValue;

	[SerializeField]
	private tk2dBaseSprite gradeStamp;

	[SerializeField]
	private ObjectShaker shaker;

	public int initialBonusValueYPosition = -100;

	public int finalBonusValueYPosition = -37;

	public int minBonusValue = 100;

	public int maxBonusValue = 300;

	protected override void Start()
	{
		base.Start();
		Singleton<InitializationManager>.instance.ExecuteOnState(InitializationManager.State.OnlineReady, delegate
		{
			Init();
		});
	}

	private void Init()
	{
		if ((bool)popupContentController)
		{
			popupContentController.IsOpen = true;
			popupContentController.OnClosed += DestroyPopUp;
		}
		if ((bool)_closeButton)
		{
			_closeButton.gameObject.SetActive(false);
		}
		ClearSequence();
		Singleton<AudioCacheManager>.instance.RegisterSingleAudioClip(AudioTrigger.CrateLand);
		Singleton<AudioCacheManager>.instance.RegisterSingleAudioClip(AudioTrigger.DieFaceSpin);
		StartCoroutine(PlaySequence());
	}

	private IEnumerator PlaySequence()
	{
		if ((bool)popupContentController)
		{
			yield return new WaitForSeconds(popupContentController.timeToOpen);
		}
		int currentAssetBundleId = 0;
		for (int i = 0; i < units.Length - 1; i++)
		{
			currentAssetBundleId = Constants.MultiTeamPopUpUnit(i);
			yield return StartCoroutine(units[i].ChangeAssetCoroutine(currentAssetBundleId));
		}
		EventDataModel activeEvent = UserProfile.player.GetActiveOnCooldownEvent();
		currentAssetBundleId = ((activeEvent == null || activeEvent.EventUnitLevel == null) ? Constants.MultiTeamPopUpUnit(units.Length - 1) : activeEvent.EventUnitLevel.assetBundleId);
		yield return StartCoroutine(units[units.Length - 1].ChangeAssetCoroutine(currentAssetBundleId));
		yield return new WaitForSeconds(0.5f);
		bonusDescription.TweenAlpha(1f, 0.5f);
		int totalPoints = Random.Range(minBonusValue, maxBonusValue);
		Transform bonusValueTransform = bonusValue.transform;
		SimpleTween.Start(0f, 1f, 0.3f, delegate(float val)
		{
			if ((bool)bonusValue)
			{
				bonusValue.text = "+" + (int)Mathf.Lerp(0f, totalPoints, val);
				bonusValue.Alpha = val;
				bonusValueTransform.SetLocalYPosition(Mathf.Lerp(initialBonusValueYPosition, finalBonusValueYPosition, val));
			}
		});
		AudioTrigger.DieFaceSpin.Play();
		yield return new WaitForSeconds(0.3f);
		if ((bool)shaker)
		{
			shaker.Shake(true);
		}
		AudioTrigger.CrateLand.Play();
		Vector3 doubleScale = gradeStamp.scale * 2f;
		Vector3 scaleTo = gradeStamp.scale;
		SimpleTween.Start(0f, 1f, 0.5f, delegate(float val)
		{
			gradeStamp.Alpha = val;
			gradeStamp.scale = doubleScale - scaleTo * val;
		});
		yield return new WaitForSeconds(0.5f);
		if ((bool)_closeButton)
		{
			_closeButton.gameObject.SetActive(true);
		}
	}

	private void ClearSequence()
	{
		bonusValue.Alpha = 0f;
		bonusValue.transform.SetLocalYPosition(initialBonusValueYPosition);
		bonusDescription.Alpha = 0f;
		gradeStamp.Alpha = 0f;
	}

	public override void OnCloseButton()
	{
		if ((bool)popupContentController)
		{
			popupContentController.IsOpen = false;
		}
	}

	private void DestroyPopUp()
	{
		PopupManager.DestroyPopup(model);
	}
}
