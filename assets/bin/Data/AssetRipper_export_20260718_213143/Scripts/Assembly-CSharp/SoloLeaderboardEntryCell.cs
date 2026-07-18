using System.Collections;
using System.Globalization;
using UnityEngine;

public class SoloLeaderboardEntryCell : ScrollableCell
{
	[SerializeField]
	private tk2dTextMesh rank;

	[SerializeField]
	private PrefabProxy tierBadgeProxy;

	[SerializeField]
	private PrefabProxy tankPictureProxy;

	[SerializeField]
	private tk2dTextMesh playerName;

	[SerializeField]
	private tk2dTextMesh pvpRating;

	[SerializeField]
	private GameObject[] topRankBadges;

	[SerializeField]
	private GameObject selfEntryGameObject;

	[SerializeField]
	private GameObject regularBackgroundGameObject;

	[SerializeField]
	private GameObject specialBackgroundGameObject;

	[SerializeField]
	private GameObject backgroundDynamic;

	[SerializeField]
	private GameObject backgroundDynamicOdd;

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
	private tk2dSprite eventPointsIcon;

	[SerializeField]
	private Transform leftAnchorInternal;

	[SerializeField]
	private Transform rightAnchorInternal;

	public bool useOddColoring;

	private Vector3 originalSize;

	[SerializeField]
	protected bool requiresAnchors;

	private Transform _leftAnchor;

	private Transform _rightAnchor;

	private void Start()
	{
		originalSize = eventPointsIcon.scale;
	}

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
			SoloLeaderboardEntryData soloLeaderboardEntryData = base.DataObject as SoloLeaderboardEntryData;
			rank.text = soloLeaderboardEntryData.rank.ToString();
			StartCoroutine(SetBadge(soloLeaderboardEntryData.tier));
			if (soloLeaderboardEntryData.tankID != null)
			{
				StartCoroutine(SetTank(soloLeaderboardEntryData.tankID, soloLeaderboardEntryData.tankLevel));
			}
			else
			{
				StartCoroutine(SetTank("11001", 1));
			}
			playerName.text = soloLeaderboardEntryData.playerName;
			pvpRating.text = string.Format(CultureInfo.InvariantCulture, "ui_event_reward_text_solo".Localize("{0:#,#}"), soloLeaderboardEntryData.pvpRating);
			bool flag = false;
			for (int i = 0; i < topRankBadges.Length; i++)
			{
				bool flag2 = i == soloLeaderboardEntryData.rank - 1;
				topRankBadges[i].SetActive(flag2);
				flag = flag || flag2;
			}
			bool flag3 = soloLeaderboardEntryData.rank == 1;
			if ((bool)regularBackgroundGameObject)
			{
				regularBackgroundGameObject.SetActive(!flag3);
			}
			if ((bool)specialBackgroundGameObject)
			{
				specialBackgroundGameObject.SetActive(flag3);
			}
			if (useOddColoring && !flag3)
			{
				backgroundDynamic.SetActive(dataIndex % 2 == 0);
				backgroundDynamicOdd.SetActive(dataIndex % 2 != 0);
			}
			EventDataModel activeEvent = UserProfile.player.GetActiveEvent();
			switch (activeEvent.EventType)
			{
			case EventDataModel.EventTypes.PVP_TOURNAMENT_EVENT:
				eventPointsIcon.SetSprite(UserInventory.ItemType.VictoryPoint.GetIconName());
				eventPointsIcon.scale = Vector3.one * 0.5f;
				break;
			case EventDataModel.EventTypes.RAIDBOSS_EVENT:
				eventPointsIcon.SetSprite(UserInventory.ItemType.RaidBossEventPoint.GetIconName());
				eventPointsIcon.scale = Vector3.one * 0.5f;
				break;
			default:
				eventPointsIcon.scale = originalSize;
				eventPointsIcon.SetSprite(UserInventory.ItemType.EventPoint.GetIconName());
				break;
			}
			SetSelfEntry(soloLeaderboardEntryData.userId == UserProfile.player.id);
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

	public IEnumerator SetBadge(string divisionId)
	{
		ProgressionDivisionDataModel currentDivision = NonUnitySingleton<DMAccessManager>.instance.GetSingle<ProgressionDivisionDataModel>(divisionId);
		if (currentDivision != null)
		{
			yield return StartCoroutine(tierBadgeProxy.ChangeAssetCoroutine(currentDivision.BadgeLinkage));
		}
	}

	public IEnumerator SetTank(string unitId, int level)
	{
		UnitDataModel currentTank = UnitDataModel.GetSingle(unitId);
		if (currentTank != null)
		{
			yield return StartCoroutine(tankPictureProxy.ChangeAssetCoroutine("Prefab.prefab", currentTank.GetLevel(level - 1).assetBundleId));
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
}
