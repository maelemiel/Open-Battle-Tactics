using System.Collections;
using UnityEngine;

public class MixSalesController : NewsController
{
	[SerializeField]
	private tk2dTextMesh _title;

	[SerializeField]
	private tk2dTextMesh _dealText;

	[SerializeField]
	private PrefabProxy _prefabProxy;

	[SerializeField]
	private PrefabProxy _gachaPrizeSprite;

	[SerializeField]
	private tk2dTextMesh _gachaTitle;

	[SerializeField]
	private tk2dTextMesh _gachaDescription;

	[SerializeField]
	private PriceLabelController _gachaPriceLabel;

	[SerializeField]
	private EnterFromController[] _movementGO;

	public static bool CanShow(NewsDataModel newsDM)
	{
		GachaPoolsDataModel single = GachaPoolsDataModel.GetSingle(newsDM.gachaId);
		if (single == null)
		{
			return false;
		}
		if (single.DateStartTimeStamp > TimeUtility.ServerTs || TimeUtility.ServerTs > single.DateEndTimeStamp)
		{
			return false;
		}
		return true;
	}

	protected override void Awake()
	{
		base.Awake();
	}

	public override IEnumerator Init(NewsDataModel newsDM = null)
	{
		yield return StartCoroutine(base.Init(newsDM));
		if (newsDM != null)
		{
			if ((bool)_title)
			{
				_title.text = newsDM.title.Localize(_title.text);
				_title.Commit();
			}
			if ((bool)_dealText)
			{
				_dealText.text = newsDM.textDescription1.Localize(_dealText.text);
				_dealText.Commit();
			}
			if (newsDM.assetLinkageId != 0)
			{
				AssetLinkageDataModel assetDM = AssetLinkageDataModel.GetSingle(newsDM.assetLinkageId);
				_prefabProxy.ChangeAsset(assetDM);
			}
			GachaPoolsDataModel prizeGachaDM = GachaPoolsDataModel.GetSingle(newsDM.gachaId);
			InitPrizeGacha(prizeGachaDM);
		}
	}

	public override void TvButtonPress()
	{
		Reporting.NewsTemplateClick(NewsTypes.Sales, SceneTransitionManager.Scene.HomeScene.ToString(), SceneTransitionManager.Scene.ShopItemsSuppliesScene.ToString());
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ShopItemsSuppliesScene);
	}

	public override void BeforeMovingInAction()
	{
		base.BeforeMovingInAction();
	}

	public override IEnumerator AfterMovingInAction()
	{
		yield return StartCoroutine(base.AfterMovingInAction());
		EnterFromController[] movementGO = _movementGO;
		foreach (EnterFromController enterFC in movementGO)
		{
			yield return StartCoroutine(enterFC.Init());
		}
	}

	private void InitPrizeGacha(GachaPoolsDataModel prizeGachaDM)
	{
		StartCoroutine(_gachaPrizeSprite.ChangeAssetCoroutine(prizeGachaDM.AssetLinkage));
		if ((bool)_gachaTitle)
		{
			_gachaTitle.text = prizeGachaDM.Name;
		}
		if ((bool)_gachaDescription)
		{
			_gachaDescription.text = prizeGachaDM.Description;
		}
		if ((bool)_gachaPriceLabel)
		{
			_gachaPriceLabel.ConfigurePriceLabel(prizeGachaDM.GetPrice());
		}
	}
}
