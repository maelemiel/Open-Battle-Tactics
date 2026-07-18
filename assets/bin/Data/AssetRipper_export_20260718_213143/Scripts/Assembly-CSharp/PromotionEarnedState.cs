using System;
using System.Collections;
using Holoville.HOTween;
using UnityEngine;

public class PromotionEarnedState : SceneState
{
	private const string PROMOTION_TEXT = "you've been promoted to the next tier";

	private const string PROMOTION_TEXT_LOC = "ui_postbattle_promoted";

	public tk2dTextMesh _titleText;

	public tk2dTextMesh _infoText;

	private ProgressionDivisionDataModel _divisionDataModel;

	[SerializeField]
	private PrefabProxy badgeProxy;

	[SerializeField]
	private FireworksAnimation fireworksEffect;

	[SerializeField]
	private Vector3 starAnimationOffset;

	[SerializeField]
	private GameObject starBust;

	private bool givenEnergy;

	public override void InitSequence(object dataObject)
	{
		base.InitSequence(dataObject);
		_divisionDataModel = (ProgressionDivisionDataModel)dataObject;
		TopBarController.instance.SectionTitle = string.Empty;
		TopBarController.instance.ShowButtons = false;
		TopBarController.instance.Visible = true;
		TopBarController.instance.LocalUserNotificationsEnabled = false;
		TopBarController.instance.ShowProgressBanner = false;
	}

	public override IEnumerator PlayStateSequence(Action<PostBattleRewardsStates, object> callback)
	{
		base.callback = callback;
		givenEnergy = false;
		yield return StartCoroutine(SetBadgeState());
		yield return new WaitForSeconds(2.5f);
		isFinished = true;
	}

	public override void SkipToEnd()
	{
		base.SkipToEnd();
		if (isFinished)
		{
			StartCoroutine(EndSequence(0f));
			return;
		}
		HOTween.Complete();
		badgeProxy.gameObject.SetActive(true);
		StopAllCoroutines();
		badgeProxy.transform.localScale = Vector3.one * 0.5f;
		StartCoroutine(SkipBadgeState());
		StartCoroutine(PlayerCompletedSeries());
		_infoText.text = "ui_postbattle_promoted".Localize("you've been promoted to the next tier");
		_infoText.Alpha = 1f;
		isFinished = true;
	}

	public override IEnumerator EndSequence(float delay)
	{
		yield return StartCoroutine(base.EndSequence(delay));
		object dataModel = _divisionDataModel;
		PostBattleRewardsStates nextState = PostBattleRewardsStates.REWARDS_PROMOTION_EARNED_BLUEPRINTS;
		if (callback != null)
		{
			callback(nextState, dataModel);
		}
	}

	private IEnumerator SkipBadgeState()
	{
		yield return StartCoroutine(badgeProxy.ChangeAssetCoroutine(_divisionDataModel.BadgeLinkage));
		Transform badge = badgeProxy.Prefab.transform;
		badge.transform.localScale = Vector3.one;
		int currentDivision = int.Parse(_divisionDataModel.id);
		BadgeViewController badgeController = badge.gameObject.AddComponent<BadgeViewController>();
		badgeController.starAnimationOffset = starAnimationOffset;
		badgeController.ConfigureStars(currentDivision % 5);
	}

	private IEnumerator SetBadgeState()
	{
		if (_divisionDataModel == null)
		{
			yield break;
		}
		_titleText.transform.localScale = Vector3.zero;
		_infoText.transform.localScale = Vector3.zero;
		_infoText.text = "ui_postbattle_promoted".Localize("you've been promoted to the next tier");
		if ((bool)starBust)
		{
			starBust.transform.localScale = Vector3.zero;
		}
		Transform titleTextTransform = _titleText.transform;
		SimpleTween.Start(0f, 1f, 1f, EaseType.EaseInOutElastic, delegate(float val)
		{
			if ((bool)titleTextTransform)
			{
				titleTextTransform.localScale = Vector3.one * val;
			}
		});
		Vector3 previousScale = badgeProxy.transform.localScale;
		badgeProxy.transform.localScale = Vector3.zero;
		yield return StartCoroutine(badgeProxy.ChangeAssetCoroutine(_divisionDataModel.BadgeLinkage));
		Transform badge = badgeProxy.Prefab.transform;
		badge.transform.localScale = Vector3.zero;
		badgeProxy.transform.localScale = previousScale;
		int currentDivision = int.Parse(_divisionDataModel.id);
		BadgeViewController badgeController = badge.gameObject.AddComponent<BadgeViewController>();
		badgeController.starAnimationOffset = starAnimationOffset;
		Transform infoTextTransform = _infoText.transform;
		SimpleTween.Start(0f, 1f, 1f, EaseType.EaseInOutElastic, delegate(float val)
		{
			if ((bool)infoTextTransform)
			{
				infoTextTransform.localScale = Vector3.one * val;
			}
		});
		yield return new WaitForSeconds(1f);
		AudioTrigger.CrowdCheering.Play();
		StartCoroutine(badgeController.PlayPromotionEffect((currentDivision - 1) % 5));
		if ((bool)starBust)
		{
			starBust.transform.TweenLocalScale(2.75f, 1f, EaseType.EaseInOutElastic);
		}
		yield return new WaitForSeconds(3.75f);
		fireworksEffect.PlayEffect();
		yield return StartCoroutine(PlayerCompletedSeries());
	}

	private IEnumerator PlayerCompletedSeries()
	{
		AudioTrigger.CrowdExcited.Play();
		AudioTrigger.WinBattle.Play();
		AudioTrigger.Fireworks.Play();
		if (!givenEnergy)
		{
			givenEnergy = true;
			ItemCollectionDataModel itemCollectionDataModel = ItemGiftDataModel.GetGiftPackage(Constants.PromoSeriesCompleteReward);
			if (itemCollectionDataModel.items != null && itemCollectionDataModel.items.Count > 0)
			{
				CurrencyEffect.Create(UserInventory.ItemType.Energy, itemCollectionDataModel.items[0].amount);
			}
		}
		fireworksEffect.PlayEffect();
		yield return new WaitForSeconds(0.5f);
	}
}
