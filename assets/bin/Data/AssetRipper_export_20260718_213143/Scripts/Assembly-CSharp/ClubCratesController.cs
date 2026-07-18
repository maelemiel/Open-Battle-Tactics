using System.Collections;
using UnityEngine;

public class ClubCratesController : NewsController
{
	[SerializeField]
	private tk2dTextMesh _title;

	[SerializeField]
	private tk2dTextMesh _subTitle1;

	[SerializeField]
	private tk2dTextMesh _subTitle2;

	[SerializeField]
	private EnterFromController[] _movementGO;

	public static bool CanShow(NewsDataModel newsDM)
	{
		if (Constants.GetIntConstantWithID("menu_button_club") > UserProfile.player.divisionInt)
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
			if (!string.IsNullOrEmpty(newsDM.title))
			{
				_title.text = newsDM.textDescription1.Localize("TITLE");
				_title.Commit();
			}
			if (!string.IsNullOrEmpty(newsDM.textDescription1))
			{
				_subTitle1.text = newsDM.textDescription1.Localize("SUBTITLE 1");
				_subTitle1.Commit();
			}
			if (!string.IsNullOrEmpty(newsDM.textDescription2))
			{
				_subTitle2.text = newsDM.textDescription2.Localize("SUBTITLE 2");
				_subTitle2.Commit();
			}
		}
	}

	public override void TvButtonPress()
	{
		Reporting.NewsTemplateClick(NewsTypes.ClubCrates, SceneTransitionManager.Scene.HomeScene.ToString(), SceneTransitionManager.Scene.ClubScene.ToString());
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ClubScene);
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
