using System.Collections.Generic;
using UnityEngine;

public class UnlockBlueprintCell : ScrollableCell
{
	private const string ACTIVE_BACKGROUND_SPRITE_NAME = "glowing_large_button";

	private const string UNACTIVE_BACKGROUND_SPRITE_NAME = "large_button";

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

	[SerializeField]
	private tk2dTextMesh textMesh;

	[SerializeField]
	private ChartBarView rarityStarsView;

	[SerializeField]
	private UnitProxy unitIcon;

	[SerializeField]
	private tk2dSlicedSprite rarityBanner;

	public override void ConfigureCell()
	{
		if (dataObject != null)
		{
			UnitDataModel unitDataModel = (UnitDataModel)dataObject;
			if ((bool)textMesh && dataObject != null)
			{
				textMesh.text = unitDataModel.name;
			}
			if ((bool)rarityStarsView)
			{
				rarityStarsView.SetBarLevel(unitDataModel.rarity - 1);
			}
			if ((bool)rarityBanner)
			{
				Vector2 dimensions = new Vector2(rarityBanner.dimensions.x, bannerSizes[unitDataModel.rarity]);
				rarityBanner.dimensions = dimensions;
				rarityBanner.SetSprite(unityRarityBanners[unitDataModel.rarity]);
			}
			if (dataIndex > 0 && controller.DataSource[dataIndex - 1] == null)
			{
				base.transform.position += new Vector3((0f - controller.cellWidth) * 0.5f, 0f, 0f);
			}
			StartCoroutine(unitIcon.ChangeAssetCoroutine(unitDataModel.Levels[0].assetBundleId));
		}
	}

	public void OnBlueprintPressed()
	{
		UnitDataModel unitDataModel = (UnitDataModel)dataObject;
		PopupManager.ShowPopup(PopupDataModel.InspectUnitPopUp(unitDataModel, null));
	}
}
