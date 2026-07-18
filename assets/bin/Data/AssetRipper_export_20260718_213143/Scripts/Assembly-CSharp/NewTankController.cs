using System.Collections;
using UnityEngine;

public class NewTankController : NewsController
{
	[SerializeField]
	private tk2dTextMesh _maxHpText;

	[SerializeField]
	private tk2dTextMesh _unityNameText;

	[SerializeField]
	private tk2dTextMesh _unityDescriptionText;

	[SerializeField]
	private tk2dTextMesh _habilityText;

	[SerializeField]
	private UnitProxy _unitProxy1;

	[SerializeField]
	private UnitProxy _unitProxy2;

	[SerializeField]
	private UnitProxy _unitProxy3;

	[SerializeField]
	private EnterFromController[] _movementGO;

	private UnitDataModel _unit;

	public static bool CanShow(NewsDataModel newsDM)
	{
		UnitDataModel single = UnitDataModel.GetSingle(newsDM.unitId);
		if (single == null)
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
		if (newsDM == null)
		{
			yield break;
		}
		_unit = UnitDataModel.GetSingle(newsDM.unitId);
		int maxHp = 0;
		int i = 0;
		int assetBundleId = 0;
		int countPrefab = 0;
		for (; i < _unit.Levels.Count; i++)
		{
			if (_unit.Levels[i].hp > maxHp)
			{
				maxHp = _unit.Levels[i].hp;
			}
			if (_unit.Levels[i].assetBundleId != assetBundleId)
			{
				assetBundleId = _unit.Levels[i].assetBundleId;
				switch (countPrefab)
				{
				case 0:
					StartCoroutine(_unitProxy1.ChangeAssetCoroutine("Prefab.prefab", assetBundleId));
					break;
				case 1:
					StartCoroutine(_unitProxy2.ChangeAssetCoroutine("Prefab.prefab", assetBundleId));
					break;
				case 2:
					StartCoroutine(_unitProxy3.ChangeAssetCoroutine("Prefab.prefab", assetBundleId));
					break;
				}
				countPrefab++;
			}
		}
		_unityNameText.text = _unit.name;
		_unityNameText.Commit();
		_unityDescriptionText.text = newsDM.textDescription1.Localize();
		_unityDescriptionText.Commit();
		_maxHpText.text = maxHp.ToString();
		_maxHpText.Commit();
		UnitLevelProgressionDataModel maxLevelProg = _unit.Levels[_unit.Levels.Count - 1];
	}

	public override void TvButtonPress()
	{
		Reporting.NewsTemplateClick(NewsTypes.NewTank, SceneTransitionManager.Scene.HomeScene.ToString(), SceneTransitionManager.Scene.BlueprintsScene.ToString());
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.BlueprintsScene, new SceneModel(_unit));
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
