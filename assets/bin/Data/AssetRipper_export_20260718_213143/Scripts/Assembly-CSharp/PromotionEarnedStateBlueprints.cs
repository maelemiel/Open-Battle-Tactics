using System;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using UnityEngine;

public class PromotionEarnedStateBlueprints : SceneState
{
	private const string BLUEPRINTS_TEXT = "New tanks available to build!";

	private const string BLUEPRINTS_TEXT_LOC = "ui_postbattle_newtanks";

	public tk2dTextMesh _titleText;

	public tk2dTextMesh _infoText;

	public ScrollableAreaController blueprintsScrollableArea;

	private ProgressionDivisionDataModel _divisionDataModel;

	[SerializeField]
	private FireworksAnimation blueprintsEffect;

	public override void InitSequence(object dataObject)
	{
		base.InitSequence(dataObject);
		_divisionDataModel = (ProgressionDivisionDataModel)dataObject;
		TopBarController.instance.SectionTitle = string.Empty;
		TopBarController.instance.ShowButtons = false;
		TopBarController.instance.Visible = true;
		TopBarController.instance.LocalUserNotificationsEnabled = false;
		TopBarController.instance.ShowProgressBanner = false;
		int tier = int.Parse(_divisionDataModel.id);
		List<UnitDataModel> unitsUnlockedAtTier = UnitDataModel.GetUnitsUnlockedAtTier(tier);
		List<AbilityDataModel> abilitiesUnlockedAtTier = UnitDataModel.GetAbilitiesUnlockedAtTier(tier);
		UserProfile.player.newBlueprintsCount = unitsUnlockedAtTier.Count;
		UserProfile.player.newAbilitiesCount = abilitiesUnlockedAtTier.Count;
		blueprintsScrollableArea.gameObject.SetActive(false);
	}

	public override IEnumerator PlayStateSequence(Action<PostBattleRewardsStates, object> callback)
	{
		base.callback = callback;
		yield return StartCoroutine(SetBlueprintsState());
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
		blueprintsScrollableArea.gameObject.SetActive(true);
		_infoText.text = "ui_postbattle_newtanks".Localize("New tanks available to build!");
		_infoText.Alpha = 1f;
		if (blueprintsScrollableArea.DataSource == null)
		{
			blueprintsScrollableArea.InitializeWithData(UnitDataModel.GetUnitsUnlockedAtTier(int.Parse(_divisionDataModel.id)));
		}
		isFinished = true;
	}

	public override IEnumerator EndSequence(float delay)
	{
		yield return StartCoroutine(base.EndSequence(delay));
		object dataModel = null;
		PostBattleRewardsStates nextState = PostBattleRewardsStates.NONE;
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

	private IEnumerator SetBlueprintsState()
	{
		if (_divisionDataModel != null)
		{
			_infoText.transform.localScale = Vector3.zero;
			_infoText.text = "ui_postbattle_newtanks".Localize("New tanks available to build!");
			_infoText.color = Color.cyan;
			SimpleTween.Start(0f, 1f, 1f, EaseType.EaseInOutElastic, delegate(float val)
			{
				_infoText.transform.localScale = Vector3.one * val;
			});
			yield return new WaitForSeconds(0.5f);
			blueprintsScrollableArea.gameObject.SetActive(true);
			blueprintsScrollableArea.InitializeWithData(UnitDataModel.GetUnitsUnlockedAtTier(int.Parse(_divisionDataModel.id)));
			blueprintsEffect.PlayEffect();
			AudioTrigger.CrowdExcited.Play();
			yield return new WaitForSeconds(1f);
		}
	}
}
