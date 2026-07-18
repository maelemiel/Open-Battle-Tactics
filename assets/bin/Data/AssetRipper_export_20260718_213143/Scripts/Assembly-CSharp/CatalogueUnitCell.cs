using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatalogueUnitCell : ScrollableCell
{
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
		{ 4, 145 },
		{ 5, 160 }
	};

	[SerializeField]
	private UnitInfoView unitInfoView;

	[SerializeField]
	private tk2dSlicedSprite rarityBanner;

	[SerializeField]
	private tk2dTextMesh unitRarity;

	[SerializeField]
	private List<Color> unitRarityColors;

	[SerializeField]
	private tk2dTextMesh dataModelID;

	[SerializeField]
	private tk2dTextMesh spriteName;

	public UnitDataModel UnitDataModel
	{
		get
		{
			return base.DataObject as UnitDataModel;
		}
	}

	public override void ConfigureCellData()
	{
		StopAllCoroutines();
		if (UnitDataModel != null)
		{
			unitInfoView.gameObject.SetActive(true);
			unitInfoView.ConfigureUnitView(UnitDataModel, 1);
			if ((bool)rarityBanner)
			{
				Vector2 dimensions = new Vector2(rarityBanner.dimensions.x, bannerSizes[UnitDataModel.rarity]);
				rarityBanner.dimensions = dimensions;
				rarityBanner.SetSprite(unityRarityBanners[UnitDataModel.rarity]);
			}
			if ((bool)dataModelID)
			{
				dataModelID.text = UnitDataModel.id;
			}
			if ((bool)spriteName)
			{
				spriteName.text = string.Empty;
			}
			StartCoroutine(SetSpriteID());
		}
		else
		{
			unitInfoView.gameObject.SetActive(false);
		}
		base.ConfigureCellData();
	}

	private IEnumerator SetSpriteID()
	{
		while (!unitInfoView.unitProxy.AssetReady)
		{
			yield return new WaitForEndOfFrame();
		}
		tk2dBaseSprite tankSprite = unitInfoView.unitProxy.Prefab.GetComponent<tk2dBaseSprite>();
		if ((bool)spriteName && (bool)tankSprite)
		{
			spriteName.text = tankSprite.CurrentSprite.name;
		}
	}

	private IEnumerator FitUnitProxy(UnitInfoView uiv)
	{
		yield return StartCoroutine(uiv.unitProxy.WaitForAssetReady());
		uiv.UnitGameObject.FitWithinBounds(uiv.unitProxy.collider.bounds);
	}
}
