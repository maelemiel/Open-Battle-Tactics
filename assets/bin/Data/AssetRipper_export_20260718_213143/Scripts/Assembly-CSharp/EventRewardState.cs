using System;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using UnityEngine;

public class EventRewardState : SceneState
{
	public float addedScale;

	public float baseScale;

	public float moveUpDistance = 50f;

	public float bounceTime = 0.3f;

	public tk2dSprite templateIcon;

	public Transform icon;

	public Transform container;

	public Transform totalPointsContainer;

	[SerializeField]
	private GameObject entryPrefab;

	[SerializeField]
	private GameObject victory;

	[SerializeField]
	private GameObject loss;

	[SerializeField]
	private tk2dTextMesh totalPoints;

	[SerializeField]
	private tk2dTextMesh totalPointLabelWin;

	[SerializeField]
	private tk2dTextMesh totalPointLabelDefeat;

	[SerializeField]
	private tk2dSpineAnimation specialEffectAnimation;

	private BattleRewardsSceneModel _sceneModel;

	private List<GameObject> rewardEntries = new List<GameObject>();

	private List<string> toDisplay = new List<string>();

	private List<string> toDisplayTypes = new List<string>();

	private static string EventPointIcon
	{
		get
		{
			if (UserProfile.player.GetActiveEvent() == null)
			{
				return UserInventory.ItemType.EventPoint.GetIconName();
			}
			if (UserProfile.player.GetActiveEvent().EventType == EventDataModel.EventTypes.RAIDBOSS_EVENT)
			{
				return UserInventory.ItemType.RaidBossEventPoint.GetIconName();
			}
			if (UserProfile.player.GetActiveEvent().EventType == EventDataModel.EventTypes.PVP_TOURNAMENT_EVENT)
			{
				return UserInventory.ItemType.VictoryPoint.GetIconName();
			}
			return UserInventory.ItemType.EventPoint.GetIconName();
		}
	}

	public override void InitSequence(object dataObject)
	{
		if (dataObject == null)
		{
			dataObject = CreateFakeEventPointsReceived();
		}
		base.InitSequence(dataObject);
		_sceneModel = ((UserNotification.EventPointsReceived)dataObject).rewards;
		toDisplay = ((UserNotification.EventPointsReceived)dataObject).toDisplay;
		toDisplayTypes = ((UserNotification.EventPointsReceived)dataObject).toDisplayTypes;
		ClearScene();
		icon.GetComponent<tk2dSprite>().SetSprite(EventPointIcon);
		if (EventPointIcon == UserInventory.ItemType.RaidBossEventPoint.GetIconName())
		{
			icon.localScale *= 0.5f;
			totalPointLabelWin.text = "ui_postbattle_userstanding".Localize("BOSS POINTS");
			totalPointLabelDefeat.text = "ui_postbattle_userstanding".Localize("BOSS POINTS");
		}
		else if (EventPointIcon == UserInventory.ItemType.VictoryPoint.GetIconName())
		{
			icon.localScale *= 0.6f;
			totalPointLabelWin.text = "ui_postbattle_victorypoints".Localize("VICTORY POINTS");
			totalPointLabelDefeat.text = "ui_postbattle_victorypoints".Localize("VICTORY POINTS");
		}
		else
		{
			totalPointLabelWin.text = "ui_postbattle_clubstanding".Localize("CLUB POINTS:");
			totalPointLabelDefeat.text = "ui_postbattle_clubstanding".Localize("CLUB POINTS");
		}
	}

	public override IEnumerator PlayStateSequence(Action<PostBattleRewardsStates, object> callback)
	{
		yield return StartCoroutine(DisplayResultsSequence(callback));
	}

	private void ClearScene()
	{
		totalPoints.scale = Vector3.one * baseScale;
		totalPoints.text = string.Empty;
		icon.gameObject.SetActive(false);
		if (rewardEntries != null)
		{
			for (int num = rewardEntries.Count - 1; num >= rewardEntries.Count; num--)
			{
				UnityEngine.Object.Destroy(rewardEntries[num]);
			}
			rewardEntries.Clear();
		}
	}

	private int GetPointsEarned()
	{
		EventDataModel activeOnCooldownEvent = UserProfile.player.GetActiveOnCooldownEvent();
		if (activeOnCooldownEvent == null)
		{
			return 0;
		}
		switch (activeOnCooldownEvent.EventType)
		{
		case EventDataModel.EventTypes.POINTS_EVENT:
			return _sceneModel.playerStats.eventPointsEarned;
		case EventDataModel.EventTypes.RAIDBOSS_EVENT:
			return _sceneModel.playerStats.raidBossDamageDealt;
		case EventDataModel.EventTypes.PVP_TOURNAMENT_EVENT:
			return _sceneModel.playerStats.victoryPointsEarned;
		case EventDataModel.EventTypes.WINS_EVENT:
			return _sceneModel.playerStats.isWinner ? 1 : 0;
		default:
			return 0;
		}
	}

	private IEnumerator DisplayResultsSequence(Action<PostBattleRewardsStates, object> callback)
	{
		base.callback = callback;
		int pointsEarned = GetPointsEarned();
		icon.gameObject.SetActive(false);
		if (_sceneModel.playerStats.isWinner)
		{
			victory.SetActive(true);
			loss.SetActive(false);
		}
		else
		{
			victory.SetActive(false);
			loss.SetActive(true);
		}
		int initialPoints = 0;
		initialPoints = ((UserProfile.player.userClub == null) ? UserProfile.player.soloEventPoints : UserProfile.player.userClub.GetTotalEventClubPoints());
		SetPoints(initialPoints, 0, pointsEarned);
		yield return new WaitForSeconds(1f);
		Vector3 initialScale;
		for (int i = 0; i < toDisplay.Count; i++)
		{
			GameObject newEntry = CreateEventPointRewardEntry(toDisplay[i], toDisplayTypes[i]);
			PlaceTextFields();
			rewardEntries.Add(newEntry);
			initialScale = newEntry.transform.localScale;
			SimpleTween.Start(0f, 2f, 0.4f, EaseType.EaseInExpo, delegate(float val)
			{
				newEntry.transform.localScale = initialScale * (3f - val);
			});
			yield return new WaitForSeconds(0.4f);
			specialEffectAnimation.gameObject.SetActive(true);
			specialEffectAnimation.gameObject.transform.position = newEntry.GetComponentInChildren<tk2dSprite>().transform.position;
			specialEffectAnimation.Reset();
			AudioTrigger.LowDamageResult.Play();
			yield return new WaitForSeconds(0.6f);
		}
		PlaceTextFields();
		GameObject totalTallyEntry = CreateEventPointRewardEntry(string.Format("ui_postbattle_total_points".Localize("Total points: {0}"), 0), EventPointIcon);
		rewardEntries.Add(totalTallyEntry);
		SimpleTween.Start(0f, 1f, 0.6f, EaseType.EaseInExpo, delegate(float val)
		{
			SetPoints(initialPoints, Mathf.RoundToInt((float)pointsEarned * val), pointsEarned);
			SetupEventPointRewardEntry(totalTallyEntry, string.Format("ui_postbattle_total_points".Localize("Total points: {0}"), Mathf.RoundToInt((float)pointsEarned * val)), EventPointIcon);
		}, delegate
		{
			SetPoints(initialPoints, pointsEarned, pointsEarned);
			SetupEventPointRewardEntry(totalTallyEntry, string.Format("ui_postbattle_total_points".Localize("Total points: {0}"), pointsEarned), EventPointIcon);
		});
		yield return new WaitForSeconds(0.6f);
		specialEffectAnimation.gameObject.SetActive(true);
		specialEffectAnimation.gameObject.transform.position = totalTallyEntry.GetComponentInChildren<tk2dSprite>().transform.position;
		specialEffectAnimation.Reset();
		AudioTrigger.LowDamageResult.Play();
		icon.gameObject.SetActive(true);
		initialScale = icon.localScale;
		AudioTrigger.SpecialResult.Play();
		SimpleTween.Start(0f, 2f, 0.4f, EaseType.EaseInExpo, delegate(float val)
		{
			icon.localScale = initialScale * (3f - val);
		});
		yield return new WaitForSeconds(1f);
		isFinished = true;
	}

	private GameObject CreateEventPointRewardEntry(string text, string iconPath)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(entryPrefab) as GameObject;
		gameObject.transform.parent = container;
		gameObject.transform.localPosition = Vector3.zero;
		SetupEventPointRewardEntry(gameObject, text, iconPath);
		return gameObject;
	}

	private void SetupEventPointRewardEntry(GameObject entryGO, string text, string iconPath)
	{
		tk2dTextMesh componentInChildren = entryGO.GetComponentInChildren<tk2dTextMesh>();
		componentInChildren.text = text;
		tk2dSprite componentInChildren2 = entryGO.GetComponentInChildren<tk2dSprite>();
		componentInChildren2.SetSprite(iconPath);
		float iconScale = GetIconScale(iconPath);
		componentInChildren2.scale = new Vector3(iconScale, iconScale, iconScale);
		entryGO.transform.localPosition = new Vector3(-5f - (float)componentInChildren.text.Length * 5f, entryGO.transform.localPosition.y, entryGO.transform.localPosition.z);
	}

	private void PlaceTextFields()
	{
		for (int i = 0; i < rewardEntries.Count; i++)
		{
			Transform transform = rewardEntries[i].transform;
			transform.localPosition = new Vector3(transform.localPosition.x, moveUpDistance * (float)(rewardEntries.Count - i), transform.localPosition.z);
		}
	}

	private void SetPoints(int initial, int points, int total)
	{
		float num = baseScale;
		if (total != 0)
		{
			num += Mathf.Lerp(0f, addedScale, (float)points / (float)total);
		}
		totalPoints.scale = Vector3.one * num;
		totalPoints.text = (initial + points).ToString();
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

	public override void SkipToEnd()
	{
		base.SkipToEnd();
		if (isFinished)
		{
			StartCoroutine(EndSequence(0f));
			return;
		}
		int count = rewardEntries.Count;
		for (int i = count; i < toDisplay.Count; i++)
		{
			GameObject item = CreateEventPointRewardEntry(toDisplay[i], toDisplayTypes[i]);
			rewardEntries.Add(item);
		}
		GameObject gameObject;
		if (rewardEntries.Count < toDisplay.Count + 1)
		{
			PlaceTextFields();
			gameObject = CreateEventPointRewardEntry(string.Empty, EventPointIcon);
			rewardEntries.Add(gameObject);
		}
		else
		{
			gameObject = rewardEntries[rewardEntries.Count - 1];
		}
		int num = 0;
		num = ((UserProfile.player.userClub == null) ? UserProfile.player.soloEventPoints : UserProfile.player.userClub.GetTotalEventClubPoints());
		int pointsEarned = GetPointsEarned();
		SetupEventPointRewardEntry(gameObject, string.Format("ui_postbattle_total_points".Localize("Total points: {0}"), pointsEarned), EventPointIcon);
		SetPoints(num, pointsEarned, pointsEarned);
		icon.gameObject.SetActive(true);
		isFinished = true;
	}

	private static float GetIconScale(string iconName)
	{
		if (iconName == UserInventory.ItemType.RaidBossEventPoint.GetIconName())
		{
			return 0.375f;
		}
		if (iconName == UserInventory.ItemType.VictoryPoint.GetIconName())
		{
			return 0.35f;
		}
		return 0.75f;
	}

	private UserNotification.EventPointsReceived CreateFakeEventPointsReceived()
	{
		BattleRewardsSceneModel battleRewardsSceneModel = new BattleRewardsSceneModel(MatchData.Type.PVP, true, new ServerTeamStatsState(), new ServerTeamStatsState(), new OpponentData(), new OpponentData(), 0);
		battleRewardsSceneModel.playerStats.eventPointsEarned = 215;
		battleRewardsSceneModel.playerStats.unitsDestroyed = 3;
		battleRewardsSceneModel.playerStats.isWinner = true;
		battleRewardsSceneModel.enemyData.type = TeamType.Player;
		return new UserNotification.EventPointsReceived(battleRewardsSceneModel, 40, 50, null, 323);
	}
}
