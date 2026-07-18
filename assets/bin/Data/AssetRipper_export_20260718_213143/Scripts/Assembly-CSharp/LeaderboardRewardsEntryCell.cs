using UnityEngine;

public class LeaderboardRewardsEntryCell : ScrollableCell
{
	[SerializeField]
	private tk2dTextMesh rankRange;

	[SerializeField]
	private GameObject backgroundDynamic;

	[SerializeField]
	private GameObject backgroundDynamicOdd;

	[SerializeField]
	private RewardsAssigner rewardAssigner;

	[SerializeField]
	private tk2dSlicedSprite backgroundSprite;

	[SerializeField]
	private tk2dSlicedSprite backgroundSpriteOdd;

	[SerializeField]
	private tk2dTiledSprite backgroundPattern;

	[SerializeField]
	private GameObject[] topRankBadges;

	[SerializeField]
	private GameObject moreInfoButton;

	[SerializeField]
	private GameObject expandedModeObjects;

	[SerializeField]
	private Transform leftAnchorInternal;

	[SerializeField]
	private Transform rightAnchorInternal;

	public bool useOddColoring;

	[SerializeField]
	protected bool requiresAnchors;

	public SizingParameters smallSize;

	public SizingParameters largeSize;

	public bool expandedState;

	private Transform _leftAnchor;

	private Transform _rightAnchor;

	public void Awake()
	{
		expandedModeObjects.SetActive(false);
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
		if (base.DataObject != null)
		{
			LeaderboardRewardsEntryData leaderboardRewardsEntryData = base.DataObject as LeaderboardRewardsEntryData;
			if (leaderboardRewardsEntryData.rankStart == leaderboardRewardsEntryData.rankEnd)
			{
				rankRange.text = leaderboardRewardsEntryData.rankStart.ToString();
			}
			else
			{
				rankRange.text = string.Format("ui_leaderboards_rewardrank".Localize("{0} - {1}"), leaderboardRewardsEntryData.rankStart, leaderboardRewardsEntryData.rankEnd);
			}
			rewardAssigner.ConfigureRewardDisplay(leaderboardRewardsEntryData.items);
			if (useOddColoring)
			{
				backgroundDynamic.SetActive(dataIndex % 2 == 0);
				backgroundDynamicOdd.SetActive(dataIndex % 2 != 0);
			}
			bool flag = false;
			for (int i = 0; i < topRankBadges.Length; i++)
			{
				bool flag2 = i == leaderboardRewardsEntryData.rankStart - 1;
				topRankBadges[i].SetActive(flag2);
				flag = flag || flag2;
			}
		}
		SetAnchoredData();
		base.ConfigureCellData();
	}

	private void SetAnchoredData()
	{
		backgroundSprite.dimensions = new Vector2(_rightAnchor.position.x - _leftAnchor.position.x, backgroundSprite.dimensions.y);
		backgroundSpriteOdd.dimensions = backgroundSprite.dimensions;
		backgroundPattern.dimensions = backgroundSprite.dimensions;
		leftAnchorInternal.position = new Vector3(_leftAnchor.position.x, leftAnchorInternal.position.y, leftAnchorInternal.position.z);
		rightAnchorInternal.position = new Vector3(_rightAnchor.position.x, rightAnchorInternal.position.y, rightAnchorInternal.position.z);
	}

	public void OnTouch()
	{
		switch (expandedState)
		{
		case true:
			backgroundSprite.dimensions = new Vector2(_rightAnchor.position.x - _leftAnchor.position.x, 85f);
			backgroundSpriteOdd.dimensions = backgroundSprite.dimensions;
			backgroundPattern.dimensions = backgroundSprite.dimensions;
			cellHeight = 85f;
			expandedModeObjects.SetActive(!expandedState);
			moreInfoButton.SetActive(expandedState);
			break;
		case false:
			backgroundSprite.dimensions = new Vector2(_rightAnchor.position.x - _leftAnchor.position.x, 145f);
			backgroundSpriteOdd.dimensions = backgroundSprite.dimensions;
			backgroundPattern.dimensions = backgroundSprite.dimensions;
			cellHeight = 145f;
			expandedModeObjects.SetActive(!expandedState);
			moreInfoButton.SetActive(expandedState);
			break;
		}
		expandedState = !expandedState;
	}
}
