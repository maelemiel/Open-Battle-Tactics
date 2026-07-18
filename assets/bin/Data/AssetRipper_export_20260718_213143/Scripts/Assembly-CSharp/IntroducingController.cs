using System.Collections;
using UnityEngine;

public class IntroducingController : NewsController
{
	[SerializeField]
	private tk2dTextMesh _title;

	[SerializeField]
	private tk2dTextMesh _unityNameText;

	[SerializeField]
	private UnitProxy _unitProxy;

	[SerializeField]
	private tk2dTextMesh _maxHpText;

	[SerializeField]
	private tk2dTextMesh _habilityText;

	[SerializeField]
	private tk2dTextMesh _stampText;

	[SerializeField]
	private tk2dSpineAnimation[] _shineAnimations;

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
		if (newsDM == null)
		{
			yield break;
		}
		_unit = UnitDataModel.GetSingle(newsDM.unitId);
		int maxHp = 0;
		for (int i = 0; i < _unit.Levels.Count; i++)
		{
			if (_unit.Levels[i].hp > maxHp)
			{
				maxHp = _unit.Levels[i].hp;
			}
		}
		_unityNameText.text = _unit.name;
		_unityNameText.Commit();
		_maxHpText.text = maxHp.ToString();
		_maxHpText.Commit();
		UnitLevelProgressionDataModel maxLevelProg = _unit.Levels[_unit.Levels.Count - 1];
		_stampText.text = newsDM.textDescription1.Localize();
		yield return StartCoroutine(_unitProxy.ChangeAssetCoroutine("Prefab.prefab", _unit.Levels[0].assetBundleId));
		Transform[] unitChildrens = _unitProxy.GetComponentsInChildren<Transform>();
		Transform[] array = unitChildrens;
		foreach (Transform transform in array)
		{
			if (transform.gameObject.name != _unitProxy.Prefab.name && transform.gameObject.name != _unitProxy.gameObject.name)
			{
				transform.gameObject.SetActive(false);
			}
		}
		UnitSpriteTween unitSpriteTween = _unitProxy.Prefab.GetComponent<UnitSpriteTween>();
		if (unitSpriteTween != null)
		{
			unitSpriteTween.enabled = false;
		}
	}

	public override void TvButtonPress()
	{
		Reporting.NewsTemplateClick(NewsTypes.IntroducingTank, SceneTransitionManager.Scene.HomeScene.ToString(), SceneTransitionManager.Scene.BlueprintsScene.ToString());
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.BlueprintsScene, new SceneModel(_unit));
	}

	public override IEnumerator AfterMovingInAction()
	{
		EnterFromController[] movementGO = _movementGO;
		foreach (EnterFromController enterFC in movementGO)
		{
			yield return StartCoroutine(enterFC.Init());
		}
		StartCoroutine(ShineAnimCoroutine());
	}

	private IEnumerator ShineAnimCoroutine()
	{
		while (true)
		{
			tk2dSpineAnimation[] shineAnimations = _shineAnimations;
			foreach (tk2dSpineAnimation shineAnim in shineAnimations)
			{
				yield return StartCoroutine(shineAnim.PlayAnimCoroutine("Shine 2"));
				yield return new WaitForSeconds(1f);
			}
		}
	}
}
