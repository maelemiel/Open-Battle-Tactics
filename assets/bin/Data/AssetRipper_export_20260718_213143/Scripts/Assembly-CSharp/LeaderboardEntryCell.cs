using System.Collections;
using UnityEngine;

public class LeaderboardEntryCell : ScrollableCell
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
	private Transform leftAnchorInternal;

	[SerializeField]
	private Transform rightAnchorInternal;

	public bool useOddColoring;

	[SerializeField]
	protected bool requiresAnchors;

	private Transform _leftAnchor;

	private Transform _rightAnchor;

	private UnitDataModel unitinfo;

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
			LeaderboardEntryData leaderboardEntryData = base.DataObject as LeaderboardEntryData;
			rank.text = leaderboardEntryData.rank.ToString();
			StartCoroutine(SetBadge(leaderboardEntryData.tier));
			if (leaderboardEntryData.tankID != null)
			{
				unitinfo = UnitDataModel.GetSingle(leaderboardEntryData.tankID);
				StartCoroutine(SetTank(leaderboardEntryData.tankID, leaderboardEntryData.tankLevel));
			}
			else
			{
				unitinfo = UnitDataModel.GetSingle("11001");
				StartCoroutine(SetTank("11001", 1));
			}
			playerName.text = leaderboardEntryData.playerName;
			pvpRating.text = leaderboardEntryData.pvpRating.ToString();
			bool flag = false;
			for (int i = 0; i < topRankBadges.Length; i++)
			{
				bool flag2 = i == leaderboardEntryData.rank - 1;
				topRankBadges[i].SetActive(flag2);
				flag = flag || flag2;
			}
			bool flag3 = leaderboardEntryData.rank == 1;
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
			SetSelfEntry(leaderboardEntryData.userId == UserProfile.player.id);
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
		if (currentTank != null && currentTank.GetLevel(level - 1) != null)
		{
			yield return StartCoroutine(tankPictureProxy.ChangeAssetCoroutine("Prefab.prefab", currentTank.GetLevel(level - 1).assetBundleId));
		}
	}

	public void SetSelfEntry(bool isSelf)
	{
		if (isSelf)
		{
			selfEntryGameObject.SetActive(isSelf);
			backgroundDynamic.SetActive(false);
			backgroundDynamicOdd.SetActive(false);
		}
	}

	public void ShowTankDetails()
	{
		if (unitinfo != null)
		{
			PopupManager.ShowPopup(PopupDataModel.InspectUnitPopUp(unitinfo, delegate
			{
			}));
		}
	}
}
