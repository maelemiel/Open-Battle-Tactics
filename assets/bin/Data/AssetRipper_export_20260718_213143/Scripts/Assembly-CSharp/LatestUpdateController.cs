using System.Collections;
using UnityEngine;

public class LatestUpdateController : NewsController
{
	[SerializeField]
	private tk2dTextMesh _description1;

	[SerializeField]
	private tk2dTextMesh _description2;

	[SerializeField]
	private tk2dTextMesh _description3;

	[SerializeField]
	private tk2dTextMesh _description4;

	[SerializeField]
	private tk2dTextMesh _description5;

	[SerializeField]
	private UnitProxy _unitProxy;

	protected override void Awake()
	{
		base.Awake();
	}

	public override IEnumerator Init(NewsDataModel newsDM = null)
	{
		yield return StartCoroutine(base.Init(newsDM));
		if (newsDM != null)
		{
			UnitDataModel unit = UnitDataModel.GetSingle(newsDM.unitId);
			UnitLevelProgressionDataModel maxLevelProg = unit.Levels[unit.Levels.Count - 1];
			StartCoroutine(_unitProxy.ChangeAssetCoroutine("Prefab.prefab", maxLevelProg.assetBundleId));
			if (!string.IsNullOrEmpty(newsDM.textDescription1))
			{
				_description1.text = newsDM.textDescription1.Localize("DESCRIPTION 1");
				_description1.Commit();
			}
			if (!string.IsNullOrEmpty(newsDM.textDescription2))
			{
				_description2.text = newsDM.textDescription2.Localize("DESCRIPTION 2");
				_description2.Commit();
			}
			if (!string.IsNullOrEmpty(newsDM.textDescription3))
			{
				_description3.text = newsDM.textDescription3.Localize("DESCRIPTION 3");
				_description3.Commit();
			}
			if (!string.IsNullOrEmpty(newsDM.textDescription4))
			{
				_description4.text = newsDM.textDescription4.Localize("DESCRIPTION 4");
				_description4.Commit();
			}
			if (!string.IsNullOrEmpty(newsDM.textDescription5))
			{
				_description5.text = newsDM.textDescription5.Localize("DESCRIPTION 5");
				_description5.Commit();
			}
		}
	}

	public override void TvButtonPress()
	{
		Reporting.NewsTemplateClick(NewsTypes.LatestUpdate, SceneTransitionManager.Scene.HomeScene.ToString(), SceneTransitionManager.Scene.ArenaScene.ToString());
		base.TvButtonPress();
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
