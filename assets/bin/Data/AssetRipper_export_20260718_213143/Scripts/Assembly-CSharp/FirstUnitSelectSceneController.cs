using System.Collections;
using UnityEngine;

public class FirstUnitSelectSceneController : SceneController
{
	[SerializeField]
	private tk2dTextMesh titleText;

	[SerializeField]
	private UnitProxy[] unitProxyList;

	[SerializeField]
	private GameObject _confirmButton;

	private FirstUnitSelectButton _selectedButton;

	public override void Awake()
	{
		_showHomeButton = false;
		_showTopBar = false;
		base.Awake();
		base.SectionTitle = string.Empty;
	}

	private void Start()
	{
		AudioTrigger.StandardCrowd.PlayMusic();
		SceneTransitionManager.readyToTransitionIn = true;
		Singleton<InitializationManager>.instance.ExecuteOnState(InitializationManager.State.BootReady, delegate
		{
			Init();
		});
	}

	private void Init()
	{
		StartCoroutine(StartSequence());
	}

	private void ConfigureUnitProxies()
	{
		if ((bool)titleText)
		{
			titleText.gameObject.SetActive(true);
		}
		for (int i = 0; i < unitProxyList.Length; i++)
		{
			unitProxyList[i].gameObject.SetActive(true);
		}
		ConfigureUnit(unitProxyList[0], TutorialConstants.FIRST_UNIT_OPTION_1, 0);
		ConfigureUnit(unitProxyList[1], TutorialConstants.FIRST_UNIT_OPTION_2, 1);
		ConfigureUnit(unitProxyList[2], TutorialConstants.FIRST_UNIT_OPTION_3, 2);
	}

	private IEnumerator StartSequence()
	{
		yield return new WaitForSeconds(1f);
		yield return StartCoroutine(AnnouncerController.DialogTrigger("PreFirstUnitSelect", 0f));
		AudioTrigger.CrowdHush.Play();
		ConfigureUnitProxies();
	}

	private IEnumerator EndSequence()
	{
		yield return new WaitForSeconds(0.5f);
		if ((bool)titleText)
		{
			titleText.gameObject.SetActive(false);
		}
		if ((bool)_confirmButton)
		{
			_confirmButton.SetActive(false);
		}
		for (int i = 0; i < unitProxyList.Length; i++)
		{
			unitProxyList[i].gameObject.SetActive(false);
		}
		yield return StartCoroutine(AnnouncerController.DialogTrigger("PostFirstUnitSelect", 0f));
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.AudiencePanShotScene, new SceneModel(new BattleSceneModel(MatchData.Type.TUTORIAL)));
	}

	private void ConfigureUnit(UnitProxy unitProxy, int unitLevelID, int index)
	{
		unitProxy.gameObject.SetActive(true);
		UnitLevelProgressionDataModel single = NonUnitySingleton<DMAccessManager>.instance.GetSingle<UnitLevelProgressionDataModel>(unitLevelID);
		StartCoroutine(unitProxy.ChangeAssetCoroutine(single.assetBundleId));
		tk2dUIItem component = unitProxy.GetComponent<tk2dUIItem>();
		unitProxy.GetComponent<FirstUnitSelectButton>().Init(single.UnitDataModel.name);
		component.OnClick += delegate
		{
			ChooseUnit(index);
		};
	}

	private void SelectUnit()
	{
		if (_selectedButton != null)
		{
			UserProfile.player.tutorial.CurrentStep = TutorialStep.FirstBattle;
			AudioTrigger.CrowdExcited.Play();
			AudioTrigger.EngineRev.Play();
			StartCoroutine(EndSequence());
		}
	}

	private void ChooseUnit(int index)
	{
		if (_selectedButton != null)
		{
			_selectedButton.SetEffect(false);
		}
		if ((bool)_confirmButton)
		{
			_confirmButton.SetActive(true);
		}
		AudioTrigger.SwapAbilities.Play();
		_selectedButton = unitProxyList[index].GetComponent<FirstUnitSelectButton>();
		_selectedButton.SetEffect(true);
		UserProfile.player.tutorial.FirstUnitIndex = index;
	}
}
