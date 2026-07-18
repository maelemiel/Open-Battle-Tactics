using System.Collections;

public class StepUpGachaController : SalesController
{
	public new static bool CanShow(NewsDataModel newsDM)
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

	private void OnDestroy()
	{
		StopAllCoroutines();
	}

	protected override void Awake()
	{
		base.Awake();
	}

	public override IEnumerator Init(NewsDataModel newsDM = null)
	{
		yield return StartCoroutine(base.Init(newsDM));
	}

	public override void TvButtonPress()
	{
		Reporting.NewsTemplateClick(NewsTypes.StepUpGacha, SceneTransitionManager.Scene.HomeScene.ToString(), SceneTransitionManager.Scene.ShopItemsSuppliesScene.ToString());
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ShopItemsSuppliesScene);
	}

	public override void BeforeMovingInAction()
	{
		base.BeforeMovingInAction();
	}

	public override IEnumerator AfterMovingInAction()
	{
		yield return StartCoroutine(base.AfterMovingInAction());
	}
}
