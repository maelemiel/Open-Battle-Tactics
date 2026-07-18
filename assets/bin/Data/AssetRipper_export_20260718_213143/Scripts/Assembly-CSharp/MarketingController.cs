using System.Collections;
using UnityEngine;

public class MarketingController : NewsController
{
	[SerializeField]
	private tk2dTextMesh _title;

	[SerializeField]
	private tk2dTextMesh _dealText;

	[SerializeField]
	private PrefabProxy _prefabProxy;

	[SerializeField]
	private EnterFromController[] _movementGO;

	public static bool CanShow(NewsDataModel newsDM)
	{
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
		}
	}

	public override void TvButtonPress()
	{
		Reporting.NewsTemplateClick(NewsTypes.Marketing, SceneTransitionManager.Scene.HomeScene.ToString(), SceneTransitionManager.Scene.ShopItemsSuppliesScene.ToString());
		Application.OpenURL(Constants.TransformerURL);
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
