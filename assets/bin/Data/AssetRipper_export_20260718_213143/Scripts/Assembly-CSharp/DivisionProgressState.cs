using System;
using System.Collections;
using Holoville.HOTween;
using UnityEngine;

public class DivisionProgressState : SceneState
{
	public float duration = 2.5f;

	public tk2dTextMesh _titleText;

	public tk2dTextMesh _infoText;

	public tk2dTextMesh _pointsInfoText;

	[SerializeField]
	private GameObject[] fireworks;

	[SerializeField]
	private tk2dUIProgressBar progressBar;

	[SerializeField]
	private tk2dUIProgressBar bonusProgressBar;

	[SerializeField]
	private tk2dSprite stateBackground;

	private DivisionProgressSceneModel _sceneModel;

	private bool levelIncreased;

	private bool givenEnergy;

	private int deltaProgressBar;

	private int pointsToPromotionSeries;

	private int currentPoints;

	private int previousPoints;

	private int pointsBoost;

	private int pointsMinusPointBoost;

	public override void InitSequence(object dataObject)
	{
		base.InitSequence(dataObject);
		TopBarController.instance.SectionTitle = string.Empty;
		TopBarController.instance.ShowButtons = false;
		TopBarController.instance.Visible = true;
		TopBarController.instance.LocalUserNotificationsEnabled = false;
		TopBarController.instance.ShowProgressBanner = false;
		_sceneModel = dataObject as DivisionProgressSceneModel;
		pointsToPromotionSeries = _sceneModel.divisionDataModel.totalPointToPromotionSeries;
		currentPoints = Mathf.Min(_sceneModel.currentPoints, pointsToPromotionSeries);
		previousPoints = _sceneModel.previousPoints;
		deltaProgressBar = currentPoints - previousPoints;
		levelIncreased = ((deltaProgressBar >= 0) ? true : false);
		pointsBoost = _sceneModel.pointsBoost;
		if (currentPoints < _sceneModel.currentPoints)
		{
			pointsMinusPointBoost = currentPoints;
		}
		else
		{
			pointsMinusPointBoost = currentPoints - pointsBoost;
		}
		progressBar.Value = _sceneModel.currentPoints / pointsToPromotionSeries;
		bonusProgressBar.Value = 0f;
		if ((bool)stateBackground)
		{
			stateBackground.SetSprite((!levelIncreased) ? PostBattleRewardsSceneController.BACKGROUND_DISABLED_EMPTY_SPRITENAME : PostBattleRewardsSceneController.BACKGROUND_ENABLED_EMPTY_SPRITENAME);
		}
	}

	public override IEnumerator PlayStateSequence(Action<PostBattleRewardsStates, object> callback)
	{
		base.callback = callback;
		givenEnergy = false;
		_sceneModel = (DivisionProgressSceneModel)dataObject;
		yield return StartCoroutine(ConfigureInfoBoard(levelIncreased, _sceneModel.didPlayerWinBattle, duration));
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
		ShowPointCountup(0f);
		if (pointsBoost > 0)
		{
			ShowPointBoost(0f);
		}
		if (currentPoints >= pointsToPromotionSeries)
		{
			StartCoroutine(PlayerPromoted());
		}
		isFinished = true;
	}

	public override IEnumerator EndSequence(float delay)
	{
		yield return StartCoroutine(base.EndSequence(delay));
		PostBattleRewardsStates nextState = PostBattleRewardsStates.NONE;
		object dataModel = null;
		if (UserProfile.player.notifications.Count == 0)
		{
			nextState = PostBattleRewardsStates.REWARDS_END;
		}
		else
		{
			UserNotification userNotification = UserNotification.ExecuteNotifications();
			dataModel = userNotification.DataModel;
			nextState = userNotification.PostBattleRewardsState;
		}
		if (callback != null)
		{
			callback(nextState, dataModel);
		}
	}

	private IEnumerator ConfigureInfoBoard(bool levelIncreased, bool didWinBattle, float duration)
	{
		if (levelIncreased)
		{
			if (didWinBattle)
			{
				AudioTrigger.CrowdCheering.Play();
				_titleText.text = "ui_postbattle_divisionprogress_congrats".Localize("Congratulations!");
				_infoText.text = "ui_postbattle_divisionprogress_congratsdetails".Localize("You've earned new fans");
			}
			else
			{
				AudioTrigger.CrowdDisappointed.Play();
				_titleText.text = "ui_postbattle_divisionprogress_battle_lost".Localize("Good effort!");
				_infoText.text = "ui_postbattle_divisionprogress_battle_lost_details".Localize("You've still earned some fans!");
			}
		}
		else
		{
			AudioTrigger.CrowdDisappointed.Play();
			_titleText.text = "ui_postbattle_divisionprogress_fail".Localize("Try again...");
			_infoText.text = "ui_postbattle_divisionprogress_faildetails".Localize("You've lost some fans");
		}
		ShowPointCountup(duration);
		yield return new WaitForSeconds(duration);
		AudioTrigger.HighFirstStrikeResult.Play();
		yield return new WaitForSeconds(0.5f);
		if (pointsBoost > 0)
		{
			ShowPointBoost(duration * 0.5f);
			yield return new WaitForSeconds(duration * 0.5f);
			AudioTrigger.HighFirstStrikeResult.Play();
		}
		if (currentPoints >= pointsToPromotionSeries)
		{
			yield return StartCoroutine(PlayerPromoted());
		}
		yield return new WaitForSeconds(1f);
	}

	private void ShowPointCountup(float duration)
	{
		SimpleTween.Start(previousPoints, pointsMinusPointBoost, duration, EaseType.Linear, delegate(float val)
		{
			progressBar.Value = val / (float)pointsToPromotionSeries;
			_pointsInfoText.text = string.Format("{0:000} / {1:000}", val, pointsToPromotionSeries);
		});
	}

	private void ShowPointBoost(float duration)
	{
		bonusProgressBar.Value = pointsMinusPointBoost / pointsToPromotionSeries;
		SimpleTween.Start(pointsMinusPointBoost, currentPoints, duration, EaseType.Linear, delegate(float val)
		{
			bonusProgressBar.Value = val / (float)pointsToPromotionSeries;
			_pointsInfoText.text = string.Format("{0:000} / {1:000}", val, pointsToPromotionSeries);
		});
	}

	private IEnumerator PlayerPromoted()
	{
		AudioTrigger.CrowdExcited.Play();
		AudioTrigger.WinBattle.Play();
		AudioTrigger.Fireworks.Play();
		if (!givenEnergy && _sceneModel.didEarnEnergy)
		{
			givenEnergy = true;
			ItemCollectionDataModel itemCollectionDataModel = ItemGiftDataModel.GetGiftPackage(Constants.PromoSeriesStartReward);
			if (itemCollectionDataModel.items != null && itemCollectionDataModel.items.Count > 0)
			{
				CurrencyEffect.Create(UserInventory.ItemType.Energy, itemCollectionDataModel.items[0].amount);
			}
		}
		GameObject[] array = fireworks;
		foreach (GameObject obj in array)
		{
			obj.SetActive(true);
			yield return new WaitForSeconds(0.5f);
		}
	}
}
