using System.Collections;
using UnityEngine;

public class RaidBossController : NewsController
{
	[SerializeField]
	private tk2dTextMesh _title;

	[SerializeField]
	private tk2dTextMesh _description1;

	[SerializeField]
	private tk2dTextMesh _description2;

	[SerializeField]
	private tk2dTextMesh _description3;

	[SerializeField]
	private tk2dTextMesh _description4;

	[SerializeField]
	private UnitProxy _unitProxy;

	[SerializeField]
	private EnterFromController[] _movementGO;

	public static bool CanShow(NewsDataModel newsDM)
	{
		UnitDataModel single = UnitDataModel.GetSingle(newsDM.unitId);
		if (single == null)
		{
			return false;
		}
		if (UserProfile.player.divisionInt <= Constants.TierEventNewsLockOut)
		{
			return false;
		}
		EventDataModel activeOnCooldownEvent = UserProfile.player.GetActiveOnCooldownEvent();
		if (activeOnCooldownEvent == null)
		{
			return false;
		}
		if (activeOnCooldownEvent.EventType != EventDataModel.EventTypes.RAIDBOSS_EVENT)
		{
			return false;
		}
		if (!activeOnCooldownEvent.IsActive)
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
			UnitDataModel unit = UnitDataModel.GetSingle(newsDM.unitId);
			if (unit != null)
			{
				UnitLevelProgressionDataModel maxLevelProg = unit.Levels[unit.Levels.Count - 1];
				StartCoroutine(_unitProxy.ChangeAssetCoroutine("Prefab.prefab", maxLevelProg.assetBundleId));
			}
			if (_title != null)
			{
				_title.text = newsDM.title.Localize("TITLE");
			}
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
		}
	}

	public override void TvButtonPress()
	{
		Reporting.NewsTemplateClick(NewsTypes.RaidBoss, SceneTransitionManager.Scene.HomeScene.ToString(), SceneTransitionManager.Scene.ArenaScene.ToString());
		base.TvButtonPress();
	}

	public override void BeforeMovingInAction()
	{
		base.BeforeMovingInAction();
	}

	public override void BeforeMovingOutAction()
	{
		base.BeforeMovingOutAction();
		_unitProxy.gameObject.SetActive(false);
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
