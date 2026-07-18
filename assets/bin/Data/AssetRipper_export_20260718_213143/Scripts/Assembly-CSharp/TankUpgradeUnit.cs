using System.Collections.Generic;
using UnityEngine;

public class TankUpgradeUnit : MonoBehaviour
{
	[SerializeField]
	private UnitInfoView unitView;

	[SerializeField]
	private GameObject cashUpgrade;

	[SerializeField]
	private GameObject partsUpgrade;

	[SerializeField]
	private GameObject notification;

	[SerializeField]
	private List<UpgradePartRequirementController> requirementControllers;

	[SerializeField]
	private tk2dSlicedSprite unitRarityBanner;

	[SerializeField]
	private GameObject maxLeveledGO;

	private Dictionary<int, string> unityRarityBanners = new Dictionary<int, string>
	{
		{ 1, "banner_bronze" },
		{ 2, "banner_silver" },
		{ 3, "banner_gold" },
		{ 4, "banner_purple" },
		{ 5, "banner_red" }
	};

	private Dictionary<int, int> bannerSizes = new Dictionary<int, int>
	{
		{ 1, 80 },
		{ 2, 105 },
		{ 3, 130 },
		{ 4, 155 },
		{ 5, 170 }
	};

	public void Init(UserUnit unit)
	{
		if ((bool)unitRarityBanner)
		{
			Vector2 dimensions = new Vector2(unitRarityBanner.dimensions.x, bannerSizes[unit.Rarity]);
			unitRarityBanner.dimensions = dimensions;
			unitRarityBanner.SetSprite(unityRarityBanners[unit.Rarity]);
		}
		unitView.ConfigureUnitView(unit.UnitDataModel, unit.level, unit.partialLevel);
		List<UnitPartialLevelDataModel> partialLevelsForCurrentLevel = unit.GetPartialLevelsForCurrentLevel();
		if (partialLevelsForCurrentLevel.Count > 0)
		{
			maxLeveledGO.SetActive(false);
			cashUpgrade.SetActive(false);
			partsUpgrade.SetActive(true);
			SetupPartUpgrades(unit, partialLevelsForCurrentLevel);
		}
		else if (unit.IsMaxLevel)
		{
			maxLeveledGO.SetActive(true);
			cashUpgrade.SetActive(false);
			partsUpgrade.SetActive(false);
			notification.SetActive(false);
		}
		else
		{
			maxLeveledGO.SetActive(false);
			cashUpgrade.SetActive(true);
			partsUpgrade.SetActive(false);
			notification.SetActive(!unit.IsMaxLevel && UserProfile.player.CanAfford(unit.GetUpgradePrice()));
		}
	}

	private void SetupPartUpgrades(UserUnit unit, List<UnitPartialLevelDataModel> partialLevels)
	{
		int num = 0;
		bool flag = false;
		for (int i = 0; i < requirementControllers.Count; i++)
		{
			if (i < partialLevels.Count)
			{
				requirementControllers[i].gameObject.SetActive(true);
				UserPriceDataModel priceForID = ItemPriceDataModel.GetPriceForID(partialLevels[i].requirementPriceId);
				bool flag2 = (unit.partialLevel >> i + 1) % 2 == 1;
				requirementControllers[num].ConfigureWithPrice(priceForID, partialLevels[i], unit.ID, flag2, i);
				flag = flag || (!flag2 && UserProfile.player.CanAfford(partialLevels[i].requirementPriceId));
				num++;
			}
			else
			{
				requirementControllers[i].gameObject.SetActive(false);
			}
		}
		notification.SetActive(flag);
	}
}
