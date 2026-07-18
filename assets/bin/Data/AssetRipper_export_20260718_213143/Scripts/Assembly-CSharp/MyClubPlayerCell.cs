using System;
using UnityEngine;

public class MyClubPlayerCell : ScrollableCell
{
	[SerializeField]
	private tk2dTextMesh playerPositionLabel;

	[SerializeField]
	private tk2dTextMesh playerNameLabel;

	[SerializeField]
	private tk2dTextMesh playerTitle;

	[SerializeField]
	private tk2dTextMesh playerVictoriesLabel;

	[SerializeField]
	private tk2dUIItem button;

	[SerializeField]
	private tk2dUIItem secondaryButton;

	[SerializeField]
	private tk2dBaseSprite backgroundSprite;

	[SerializeField]
	private GameObject eventPointsGameObject;

	[SerializeField]
	private tk2dTextMesh playerEventPointsLabel;

	[SerializeField]
	private tk2dTextMesh playerEventPointsAmount;

	[SerializeField]
	private tk2dSprite eventIcon;

	[SerializeField]
	private PrefabProxy playerBadge;

	[SerializeField]
	private StreamingThumbnail playerAvatar;

	[SerializeField]
	private GameObject availableGameObject;

	[SerializeField]
	private GameObject takenGameObject;

	[SerializeField]
	private GameObject pvpRatingPointsGameObject;

	private Vector3 eventPointInitialScale;

	public bool useOddColoring;

	public bool deactivateWhenNull = true;

	private bool objectAlive = true;

	private IPlayerClubMetadata playerCellInfo;

	private string[] EventDisplayInforation
	{
		get
		{
			EventDataModel activeEvent = UserProfile.player.GetActiveEvent();
			if (activeEvent == null)
			{
				return new string[3]
				{
					UserInventory.ItemType.EventPoint.GetIconName(),
					"ui_postbattle_eventpoints",
					"EVENT POINTS"
				};
			}
			switch (activeEvent.EventType)
			{
			case EventDataModel.EventTypes.RAIDBOSS_EVENT:
				return new string[3]
				{
					UserInventory.ItemType.RaidBossEventPoint.GetIconName(),
					"ui_postbattle_bosspoints",
					"BOSS POINTS"
				};
			case EventDataModel.EventTypes.PVP_TOURNAMENT_EVENT:
				return new string[3]
				{
					UserInventory.ItemType.VictoryPoint.GetIconName(),
					"ui_postbattle_victorypoints",
					"VICTORY POINTS"
				};
			default:
				return new string[3]
				{
					UserInventory.ItemType.EventPoint.GetIconName(),
					"ui_postbattle_eventpoints",
					"EVENT POINTS"
				};
			}
		}
	}

	private void Awake()
	{
		if ((bool)eventIcon)
		{
			eventPointInitialScale = eventIcon.transform.localScale;
		}
	}

	public override void Init(ScrollableAreaController controller, object data, int index, float cellHeight, float cellWidth, ScrollableCell parentCell)
	{
		deactivateIfNull = deactivateWhenNull;
		base.Init(controller, data, index, cellHeight, cellWidth, parentCell);
	}

	public override void ConfigureCellData()
	{
		base.ConfigureCellData();
		playerCellInfo = (IPlayerClubMetadata)dataObject;
		UserProfile player = UserProfile.player;
		bool flag = playerCellInfo == null;
		if ((bool)availableGameObject)
		{
			availableGameObject.gameObject.SetActive(flag);
		}
		if ((bool)takenGameObject)
		{
			takenGameObject.gameObject.SetActive(!flag);
		}
		if (!flag)
		{
			if (string.IsNullOrEmpty(playerCellInfo.Name))
			{
				Log.Error("Trying to set a null name on a player");
				return;
			}
			if ((bool)playerPositionLabel)
			{
				playerPositionLabel.text = (dataIndex + 1).ToString();
			}
			if ((bool)playerNameLabel)
			{
				playerNameLabel.text = playerCellInfo.Name;
			}
			if ((bool)playerTitle)
			{
				playerTitle.gameObject.SetActive(playerCellInfo.ID == playerCellInfo.Club.leaderID);
			}
			if ((bool)playerBadge)
			{
				StartCoroutine(playerBadge.ChangeAssetCoroutine(playerCellInfo.TierAssetLinkage));
			}
			if ((bool)playerAvatar)
			{
				playerAvatar.ChangeThumbnail(playerCellInfo.ThumbnailURL);
			}
			if ((bool)button)
			{
				button.gameObject.SetActive(player.id == playerCellInfo.ID);
			}
			if ((bool)secondaryButton)
			{
				secondaryButton.gameObject.SetActive(player.id != playerCellInfo.ID && player.id == playerCellInfo.Club.leaderID);
			}
			EventDataModel activeEvent = player.GetActiveEvent();
			if (activeEvent != null)
			{
				int currentEventUserClubPoints = playerCellInfo.Club.GetCurrentEventUserClubPoints(playerCellInfo.ID);
				if ((bool)eventPointsGameObject)
				{
					eventPointsGameObject.SetActive(true);
				}
				if ((bool)playerEventPointsAmount)
				{
					playerEventPointsAmount.text = currentEventUserClubPoints.ToString();
				}
				string[] eventDisplayInforation = EventDisplayInforation;
				if ((bool)playerEventPointsLabel)
				{
					playerEventPointsLabel.text = eventDisplayInforation[1].Localize(eventDisplayInforation[2]);
				}
				if ((bool)eventIcon)
				{
					eventIcon.SetSprite(eventDisplayInforation[0]);
					if (activeEvent.EventType == EventDataModel.EventTypes.RAIDBOSS_EVENT)
					{
						eventIcon.transform.localScale = eventPointInitialScale * 0.5f;
					}
					else if (activeEvent.EventType == EventDataModel.EventTypes.PVP_TOURNAMENT_EVENT)
					{
						eventIcon.transform.localScale = eventPointInitialScale * 0.5f;
					}
				}
			}
			else if ((bool)eventPointsGameObject)
			{
				eventPointsGameObject.SetActive(false);
			}
			if ((bool)playerVictoriesLabel)
			{
				playerVictoriesLabel.text = playerCellInfo.PVPRating.ToString();
			}
		}
		if (useOddColoring)
		{
			if (dataIndex % 2 == 0)
			{
				backgroundSprite.gameObject.SetActive(false);
			}
			else
			{
				backgroundSprite.gameObject.SetActive(true);
			}
		}
	}

	private void OnLeave()
	{
		if (UserProfile.player.GetActiveEvent() != null)
		{
			int currentEventUserClubPoints = UserProfile.player.userClub.GetCurrentEventUserClubPoints(UserProfile.player.id);
			currentEventUserClubPoints = (int)Math.Floor((double)currentEventUserClubPoints * ((double)Constants.EventPointsKeptOnLeave / 100.0));
			if (currentEventUserClubPoints > 0 && UserProfile.player.userClub.members.Count > 1)
			{
				string message = string.Format("ui_clubs_leave_club_description_event".Localize("Are you sure you want to leave this club?\n^cf22f You will keep {0} event points!"), currentEventUserClubPoints);
				PopupManager.ShowPopup(PopupDataModel.NoYes("ui_clubs_leave_club_title".Localize("Leave Club"), message, OnLeaveConfirmation));
				return;
			}
		}
		PopupManager.ShowPopup(PopupDataModel.NoYes("ui_clubs_leave_club_title".Localize("Leave Club"), "ui_clubs_leave_club_description".Localize("Are you sure you want to leave this club?"), OnLeaveConfirmation));
	}

	private void OnLeaveConfirmation()
	{
		Singleton<SessionManager>.instance.LeaveClub(ClubLeft);
	}

	private void OnKick()
	{
		if (!objectAlive)
		{
			return;
		}
		if (UserProfile.player.GetActiveEvent() != null)
		{
			int currentEventUserClubPoints = UserProfile.player.userClub.GetCurrentEventUserClubPoints(playerCellInfo.ID);
			currentEventUserClubPoints = (int)Math.Floor((double)currentEventUserClubPoints * ((double)Constants.EventPointsKeptOnKick / 100.0));
			if (currentEventUserClubPoints > 0)
			{
				string message = string.Format("ui_clubs_kick_description_event".Localize("Are you sure you want to kick this player?\n^cf22f Your club will lose {0} event points!"), currentEventUserClubPoints);
				PopupManager.ShowPopup(PopupDataModel.NoYes("ui_clubs_kick_title".Localize("Kick User"), message, OnKickConfirmation));
				return;
			}
		}
		PopupManager.ShowPopup(PopupDataModel.NoYes("ui_clubs_kick_title".Localize("Kick User"), "ui_clubs_kick_description".Localize("Are you sure you want to kick this player?"), OnKickConfirmation));
	}

	private void OnKickConfirmation()
	{
		Singleton<SessionManager>.instance.KickFromClub(playerCellInfo.ID, PlayerKickedOut);
	}

	private void ClubLeft(bool state)
	{
		controller.OnDataChanged();
		if (state && objectAlive)
		{
			PopupManager.ShowPopup(PopupDataModel.Ok("ui_clubs_club_left_title".Localize("Club left"), "ui_clubs_club_left_description".Localize("You've left the club"), null));
		}
	}

	private void PlayerKickedOut(bool state)
	{
		if (state && objectAlive)
		{
			controller.OnDataChanged();
			PopupManager.ShowPopup(PopupDataModel.Ok("ui_clubs_club_member_kicked_out_title".Localize("User Kicked Out"), "ui_clubs_club_member_kicked_out_description".Localize("The user has been kicked out"), null));
		}
	}

	private void OnDestroy()
	{
		objectAlive = false;
	}
}
