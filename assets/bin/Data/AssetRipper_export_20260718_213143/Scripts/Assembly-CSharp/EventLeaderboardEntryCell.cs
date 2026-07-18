using System.Collections;
using System.Globalization;
using UnityEngine;

public class EventLeaderboardEntryCell : ScrollableCell
{
	public bool useOddColoring;

	[SerializeField]
	private tk2dTextMesh rank;

	[SerializeField]
	private PrefabProxy badgeProxy;

	[SerializeField]
	private tk2dTextMesh clubName;

	[SerializeField]
	private tk2dTextMesh clubNameDownButton;

	[SerializeField]
	private tk2dTextMesh winCountText;

	[SerializeField]
	private GameObject[] topRankBadges;

	[SerializeField]
	private GameObject selfEntryGameObject;

	[SerializeField]
	private GameObject backgroundDynamic;

	[SerializeField]
	private GameObject backgroundDynamicOdd;

	[SerializeField]
	private GameObject specialBackgroundGameObject;

	[SerializeField]
	private GameObject regularBackgroundGameObject;

	[SerializeField]
	private tk2dSlicedSprite backgroundSprite;

	[SerializeField]
	private tk2dSlicedSprite backgroundSpriteOdd;

	[SerializeField]
	private tk2dSlicedSprite selfEntrySpriteBg;

	[SerializeField]
	private tk2dSlicedSprite goldEntrySprite;

	[SerializeField]
	private tk2dTiledSprite backgroundPattern;

	[SerializeField]
	private tk2dSprite eventPointIcon;

	[SerializeField]
	private GameObject eventWinTrophyIcon;

	[SerializeField]
	private Transform leftAnchorInternal;

	[SerializeField]
	private Transform rightAnchorInternal;

	private MyClubView clubView;

	private string clubId;

	[SerializeField]
	protected bool requiresAnchors;

	private Transform _leftAnchor;

	private Transform _rightAnchor;

	public override bool GetAnchorRequirement()
	{
		return requiresAnchors;
	}

	public override void SetAnchors(Transform leftAnchor, Transform rightAnchor)
	{
		_leftAnchor = leftAnchor;
		_rightAnchor = rightAnchor;
		base.SetAnchors(leftAnchor, rightAnchor);
	}

	public override void ConfigureCellData()
	{
		selfEntryGameObject.SetActive(false);
		backgroundDynamic.SetActive(true);
		backgroundDynamicOdd.SetActive(true);
		if (base.DataObject != null)
		{
			selfEntryGameObject.SetActive(false);
			backgroundDynamic.SetActive(true);
			backgroundDynamicOdd.SetActive(true);
			EventLeaderboardEntryData eventLeaderboardEntryData = base.DataObject as EventLeaderboardEntryData;
			if (eventLeaderboardEntryData.error)
			{
				base.gameObject.SetActive(false);
				return;
			}
			rank.text = eventLeaderboardEntryData.rank.ToString();
			clubView = eventLeaderboardEntryData.clubView;
			clubId = eventLeaderboardEntryData.clubId;
			StartCoroutine(SetBadge(eventLeaderboardEntryData.badgeId));
			clubName.text = eventLeaderboardEntryData.clubName;
			if ((bool)clubNameDownButton)
			{
				clubNameDownButton.text = eventLeaderboardEntryData.clubName;
			}
			winCountText.text = string.Format(CultureInfo.InvariantCulture, "ui_event_reward_text_club".Localize("{0:#,#}"), eventLeaderboardEntryData.winCount);
			bool flag = false;
			for (int i = 0; i < topRankBadges.Length; i++)
			{
				bool flag2 = i == eventLeaderboardEntryData.rank - 1;
				topRankBadges[i].SetActive(flag2);
				flag = flag || flag2;
			}
			if (useOddColoring)
			{
				backgroundDynamic.SetActive(dataIndex % 2 == 0);
				backgroundDynamicOdd.SetActive(dataIndex % 2 != 0);
			}
			bool flag3 = eventLeaderboardEntryData.rank == 1;
			if ((bool)regularBackgroundGameObject)
			{
				regularBackgroundGameObject.SetActive(!flag3);
			}
			if ((bool)specialBackgroundGameObject)
			{
				specialBackgroundGameObject.SetActive(flag3);
			}
			SetSelfEntry(eventLeaderboardEntryData.clubId == UserProfile.player.clubID);
			SetEventType();
		}
		SetAnchoredData();
		base.ConfigureCellData();
	}

	private void SetAnchoredData()
	{
		backgroundSprite.dimensions = new Vector2(_rightAnchor.position.x - _leftAnchor.position.x, backgroundSprite.dimensions.y);
		backgroundSpriteOdd.dimensions = backgroundSprite.dimensions;
		selfEntrySpriteBg.dimensions = backgroundSprite.dimensions;
		goldEntrySprite.dimensions = backgroundSprite.dimensions;
		backgroundPattern.dimensions = backgroundSprite.dimensions;
		leftAnchorInternal.position = new Vector3(_leftAnchor.position.x, leftAnchorInternal.position.y, leftAnchorInternal.position.z);
		rightAnchorInternal.position = new Vector3(_rightAnchor.position.x, rightAnchorInternal.position.y, rightAnchorInternal.position.z);
	}

	public IEnumerator SetBadge(int clubBadgeId)
	{
		AssetLinkageDataModel clubBadgeAssetLinkage = AssetLinkageDataModel.GetSingle(clubBadgeId.ToString());
		if (clubBadgeAssetLinkage != null)
		{
			yield return StartCoroutine(badgeProxy.ChangeAssetCoroutine(clubBadgeAssetLinkage));
		}
	}

	public void SetSelfEntry(bool isSelf)
	{
		if (isSelf)
		{
			selfEntryGameObject.SetActive(isSelf);
			backgroundDynamic.SetActive(!isSelf);
			backgroundDynamicOdd.SetActive(!isSelf);
		}
	}

	public void ShowClanView()
	{
		if (!clubView || clubView.gameObject.activeSelf)
		{
			return;
		}
		Singleton<SessionManager>.instance.GetClubWithID(clubId, delegate(UserClub fetchedClub)
		{
			if (!(this == null) && fetchedClub != null)
			{
				clubView.gameObject.SetActive(true);
				clubView.ConfigureView(fetchedClub);
			}
		});
	}

	private void SetEventType()
	{
		if ((bool)eventWinTrophyIcon)
		{
			eventWinTrophyIcon.SetActive(false);
		}
		EventDataModel activeEvent = UserProfile.player.GetActiveEvent();
		switch (activeEvent.EventType)
		{
		case EventDataModel.EventTypes.PVP_TOURNAMENT_EVENT:
			eventPointIcon.SetSprite(UserInventory.ItemType.VictoryPoint.GetIconName());
			eventPointIcon.scale = Vector3.one * 0.5f;
			break;
		case EventDataModel.EventTypes.RAIDBOSS_EVENT:
			eventPointIcon.SetSprite(UserInventory.ItemType.RaidBossEventPoint.GetIconName());
			eventPointIcon.scale = Vector3.one * 0.5f;
			break;
		default:
			eventPointIcon.SetSprite(UserInventory.ItemType.EventPoint.GetIconName());
			break;
		}
	}
}
