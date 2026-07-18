using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemSalesController : NewsController
{
	[SerializeField]
	private tk2dTextMesh _title;

	[SerializeField]
	private tk2dTextMesh _dealText;

	[SerializeField]
	private tk2dTextMesh _aditionalText;

	[SerializeField]
	private ShopItemView _shopItem;

	[SerializeField]
	private PrefabProxy _prefabProxy;

	[SerializeField]
	private EnterFromController[] _movementGO;

	protected override void Awake()
	{
		base.Awake();
	}

	public override IEnumerator Init(NewsDataModel newsDM = null)
	{
		yield return StartCoroutine(base.Init(newsDM));
		if (newsDM == null)
		{
			yield break;
		}
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
		if ((bool)_aditionalText)
		{
			_aditionalText.text = newsDM.textDescription2.Localize(_aditionalText.text);
			_aditionalText.Commit();
		}
		if (newsDM.assetLinkageId != 0)
		{
			AssetLinkageDataModel assetDM = AssetLinkageDataModel.GetSingle(newsDM.assetLinkageId);
			_prefabProxy.ChangeAsset(assetDM);
		}
		Singleton<BankService>.instance.GetASCItems(delegate(bool querySuccess, List<ShopItem> shopItems)
		{
			if (querySuccess)
			{
				_shopItem.ConfigureView(shopItems[0]);
			}
		});
	}

	public override void TvButtonPress()
	{
		Reporting.NewsTemplateClick(NewsTypes.Sales, SceneTransitionManager.Scene.HomeScene.ToString(), SceneTransitionManager.Scene.ShopWindowPopUp.ToString());
		TopBarController.instance.LoadShop();
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
}
