using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitionManager : MonoBehaviour
{
	public enum Scene
	{
		Default = 0,
		BackGroundScene = 1,
		TitleScene = 2,
		HomeScene = 3,
		EditTeamUnitsScene = 4,
		EditTeamAbilitiesScene = 5,
		ShopScene = 6,
		GachaScene = 7,
		BlueprintsScene = 8,
		ArenaScene = 9,
		ShopItemsSuppliesScene = 10,
		AudiencePanShotScene = 11,
		BattleScene = 12,
		ContractsScene = 13,
		CheatsScene = 14,
		LoadingPopupScene = 15,
		PopupScene = 16,
		PopupSceneLarge = 17,
		PopupScenePreview = 18,
		BlueprintsBuildPopUp = 19,
		BlueprintsInspectTankPopUp = 20,
		BlueprintsInspectLockedTankPopUp = 21,
		BlueprintsClaimTankPopUp = 22,
		SkipWaitPopupScene = 23,
		SkipConstructionPopup = 24,
		BuyItemPopUp = 25,
		PriceConfirmationPopUp = 26,
		SkipCooldownPopUpScene = 27,
		AbilitiesInspectPopUp = 28,
		UpgradeUnitPopUp = 29,
		UpgradeUnitPartsPopUp = 30,
		UpgradeUnitPartDetailPopUp = 31,
		TierRewardPopUp = 32,
		GachaResultPopUp = 33,
		TellAFriendPopUp = 34,
		AccountSettingsPopUp = 35,
		WebViewScene = 36,
		TopBarScene = 37,
		SettingsScene = 38,
		FirstUnitSelectScene = 39,
		PostBattleRewardsScene = 40,
		ArenaSelectionScene = 41,
		LoginScene = 42,
		AutoBotBattleCalculationScene = 43,
		UpgradeUserAccountPopUpScene = 44,
		LeaderboardsScene = 45,
		PickYourName = 46,
		PersonalParts = 47,
		GachaInfoPopUp = 48,
		CatalogueScene = 49,
		ClubScene = 50,
		PasswordPopUpScene = 51,
		LeaderboardRewardsPopUp = 52,
		ClubLeaderboardsRewardsPopUp = 53,
		ClubRewardsPopUp = 54,
		SoloRewardsPopUp = 55,
		LeaderboardsEntryPopUp = 56,
		ChatPopUp = 57,
		PerformanceSettingsPopUp = 58,
		NotificationSettingsPopUp = 59,
		LikeUsFBPopUp = 60,
		EventAlreadyClubPopUp = 61,
		EventJoinClubPopUp = 62,
		EventInfoPopUp = 63,
		FastForwardWaitingPopUp = 64,
		ItemsMixer = 65,
		ShopWindowPopUp = 66,
		PersonalPartsPopUp = 67,
		ClubCratesPopUp = 68,
		Announcers = 69,
		RevivePopup = 70,
		FoundClubCrate = 71,
		BountyBoardPopup = 72,
		SoloLeaderboardRewardsPopUp = 73,
		HelpPopUp = 74,
		RaidBossDefeatedPopUp = 75,
		MultiTeamReportPopUp = 76,
		MultiTeamInitialPopUp = 77,
		MultiTeamFinalReportPopUp = 78,
		NormalTicketBoostPopup = 79,
		PointEventTicketBoostPopup = 80,
		PvpEventTicketBoostPopup = 81,
		RaidBossTicketBoostPopUp = 82,
		BotBattlePopUp = 83,
		ResetBotBattleCapPopUpScene = 84,
		TankUpgradeScene = 85,
		_NULL = 86,
		OpenGiftNotification = 87,
		ConnectionErrorPopUpScene = 88,
		ServersSelectionPopUpScene = 89
	}

	public delegate IEnumerator TransitionInPrepareDelegate();

	private static SceneTransitionManager instance = null;

	private static string reportFromScene;

	private static string reportToScene;

	private static SceneTransition.Type reportTransitionType;

	private static long reportTimeToLoad;

	private static Scene[] allowDuplicatesInHistory = new Scene[0];

	private static Scene[] avoidHistoryScenes = new Scene[2]
	{
		Scene.BattleScene,
		Scene.GachaScene
	};

	public static int multiSceneCounter = 0;

	private static SceneModel pendingPopScene;

	private static SceneModel pendingPushScene;

	private static bool _readyToTransitionIn = false;

	private static Action OnLoadSceneDoneCallback;

	public static bool transitionActive = false;

	private static SceneModel currentSceneDM;

	private List<SceneController> allOldSceneRoots = new List<SceneController>();

	private List<SceneController> pendingOldSceneRoots = new List<SceneController>();

	private List<SceneController> allNewSceneRoots = new List<SceneController>();

	private List<SceneController> pendingNewSceneRoots = new List<SceneController>();

	private bool sceneLoaded;

	private bool startedTransitionIn;

	private SceneTransition.Type transitionType;

	private static List<TransitionInPrepareDelegate> transitionInPrepareHandlers = new List<TransitionInPrepareDelegate>();

	private static List<Action> transitionInDoneHandlers = new List<Action>();

	private static List<Action> transitionOutDoneHandlers = new List<Action>();

	private static List<Action> transitionOutBeginHandlers = new List<Action>();

	public static Scene sceneAfterMultiScene = Scene.Default;

	public static bool readyToTransitionIn
	{
		get
		{
			return _readyToTransitionIn;
		}
		set
		{
			_readyToTransitionIn = value;
			if (_readyToTransitionIn)
			{
				if (reportFromScene != null)
				{
					Log.DebugTag("Transition Ready:  {0}, {1}, {2}, {3}", null, "scenetransitionmanager", reportFromScene, reportToScene, reportTransitionType.ToString(), TimeUtility.ServerTs - reportTimeToLoad);
					reportFromScene = null;
					reportToScene = null;
				}
				else
				{
					Log.DebugTag("No Scene to finish Transition", null, "scenetransitionmanager");
				}
			}
		}
	}

	public static SceneModel CurrentSceneDM
	{
		get
		{
			return currentSceneDM;
		}
	}

	public static event TransitionInPrepareDelegate TransitionInPrepare
	{
		add
		{
			if (instance != null)
			{
				transitionInPrepareHandlers.Add(value);
			}
		}
		remove
		{
			transitionInPrepareHandlers.Remove(value);
		}
	}

	public static event Action TransitionInDone
	{
		add
		{
			if (instance == null)
			{
				value();
			}
			else
			{
				transitionInDoneHandlers.Add(value);
			}
		}
		remove
		{
			transitionInDoneHandlers.Remove(value);
		}
	}

	public static event Action TransitionOutDone
	{
		add
		{
			if (instance == null)
			{
				value();
			}
			else
			{
				transitionOutDoneHandlers.Add(value);
			}
		}
		remove
		{
			transitionOutDoneHandlers.Remove(value);
		}
	}

	public static event Action TransitionOutBegin
	{
		add
		{
			transitionOutBeginHandlers.Add(value);
		}
		remove
		{
			transitionOutBeginHandlers.Remove(value);
		}
	}

	public static void ClearHistory()
	{
		if (currentSceneDM != null)
		{
			currentSceneDM._previous = null;
		}
	}

	public static void ProcessPendingScenes()
	{
		if (!(instance != null) && PopupManager.PopupCount == 0 && !PopupManager.Acting)
		{
			if (pendingPopScene != null)
			{
				Scene outScene = ((currentSceneDM == null) ? Scene._NULL : currentSceneDM._scene);
				currentSceneDM = pendingPopScene;
				pendingPushScene = null;
				pendingPopScene = null;
				GotoScene(outScene, currentSceneDM._scene, SceneTransition.Type.Pop);
			}
			else if (pendingPushScene != null)
			{
				Scene outScene2 = ((currentSceneDM == null) ? Scene._NULL : currentSceneDM._scene);
				currentSceneDM = pendingPushScene;
				pendingPushScene = null;
				pendingPopScene = null;
				GotoScene(outScene2, currentSceneDM._scene, SceneTransition.Type.Push);
			}
		}
	}

	public static void PushToScene(Scene scene, Action onLoadSceneDoneCB = null)
	{
		OnLoadSceneDoneCallback = onLoadSceneDoneCB;
		PushToScene(scene, null, true);
	}

	public static void PushToScene(Scene scene, SceneModel sceneDM, Action onLoadSceneDoneCB = null)
	{
		OnLoadSceneDoneCallback = onLoadSceneDoneCB;
		PushToScene(scene, sceneDM, true);
	}

	public static void PushToScene(Scene scene, SceneModel sceneDM, bool keepOnHistory, Action onLoadSceneDoneCB)
	{
		OnLoadSceneDoneCallback = onLoadSceneDoneCB;
		PushToScene(scene, sceneDM, true, keepOnHistory);
	}

	public static void PushToScene(Scene scene, SceneModel sceneDM, bool keepOnHistory, bool force = false)
	{
		Log.DebugTag("PushToScene: " + scene, null, "scenetransitionmanager");
		if (currentSceneDM != null && scene == currentSceneDM._scene && !force)
		{
			Log.WarningTag("Trying to navigate to the same scene: [" + scene.ToString() + "]. Cancelling request", null, "scenetransitionmanager");
			return;
		}
		if (scene == Scene.Default)
		{
			scene = AppConfig.defaultScene;
		}
		if (sceneDM == null)
		{
			sceneDM = new SceneModel();
		}
		Scene[] array = avoidHistoryScenes;
		foreach (Scene scene2 in array)
		{
			if (currentSceneDM != null && scene2 == currentSceneDM._scene)
			{
				keepOnHistory = false;
			}
		}
		if (currentSceneDM != null)
		{
			reportFromScene = currentSceneDM._scene.ToString();
			reportToScene = scene.ToString();
			reportTransitionType = SceneTransition.Type.Push;
			reportTimeToLoad = TimeUtility.ServerTs;
			Log.DebugTag("Transition Start: {0}, {1}, {2}", null, "scenetransitionmanager", reportFromScene, reportToScene, reportTransitionType.ToString());
		}
		if (currentSceneDM != null)
		{
			if (keepOnHistory)
			{
				RemoveDuplicateScenes(scene);
			}
			sceneDM._previous = ((!keepOnHistory) ? currentSceneDM._previous : currentSceneDM);
		}
		sceneDM._scene = scene;
		if (instance == null && PopupManager.PopupCount == 0 && !PopupManager.Acting)
		{
			Scene outScene = ((currentSceneDM == null) ? Scene._NULL : currentSceneDM._scene);
			currentSceneDM = sceneDM;
			Log.DebugTag("PushToScene->GotoScene: " + scene, null, "scenetransitionmanager");
			GotoScene(outScene, scene, SceneTransition.Type.Push);
		}
		else
		{
			Log.DebugTag("PushToScene->pendingPushScene: " + scene, null, "scenetransitionmanager");
			pendingPushScene = sceneDM;
		}
	}

	private static void RemoveDuplicateScenes(Scene scene)
	{
		if (currentSceneDM == null)
		{
			return;
		}
		for (int i = 0; i < allowDuplicatesInHistory.Length; i++)
		{
			if (allowDuplicatesInHistory[i] == scene)
			{
				return;
			}
		}
		SceneModel sceneModel = null;
		SceneModel previous = currentSceneDM;
		int num = 0;
		while (previous != null)
		{
			if (previous._scene == scene)
			{
				num++;
				if (num == 2)
				{
					sceneModel._previous = previous._previous;
					break;
				}
			}
			sceneModel = previous;
			previous = previous._previous;
		}
	}

	public static bool PopScene()
	{
		if (currentSceneDM != null && currentSceneDM._previous != null)
		{
			PopToScene(currentSceneDM._previous._scene);
			return true;
		}
		if (currentSceneDM == null || currentSceneDM._scene != AppConfig.defaultScene)
		{
			PopToScene(AppConfig.defaultScene);
			return true;
		}
		return false;
	}

	public static void PopToScene(Scene scene)
	{
		PopToScene(scene, null);
	}

	public static void PopToScene(Scene scene, SceneModel replacementDataModel)
	{
		Log.Debug("PopToScene: " + scene);
		if (scene == Scene.Default)
		{
			scene = AppConfig.defaultScene;
		}
		if (currentSceneDM != null)
		{
			reportFromScene = currentSceneDM._scene.ToString();
			reportToScene = scene.ToString();
			reportTransitionType = SceneTransition.Type.Pop;
			reportTimeToLoad = TimeUtility.ServerTs;
			Log.DebugTag("Transition Start: {0}, {1}, {2}", null, "scenetransitionmanager", reportFromScene, reportToScene, reportTransitionType.ToString());
		}
		SceneModel sceneModel = null;
		if (currentSceneDM != null)
		{
			SceneModel previous = currentSceneDM;
			while ((previous = previous._previous) != null)
			{
				if (previous._scene == scene)
				{
					sceneModel = previous;
					break;
				}
			}
		}
		if (sceneModel == null)
		{
			Log.WarningTag("PopToScene: Scene {0} was not found on the Stack.", null, "scenetransitionmanager", scene.ToString());
			SceneModel sceneModel2 = new SceneModel();
			sceneModel2._scene = scene;
			sceneModel = sceneModel2;
		}
		if (replacementDataModel != null)
		{
			replacementDataModel._previous = sceneModel._previous;
			replacementDataModel._scene = scene;
			sceneModel = replacementDataModel;
		}
		if (instance == null && PopupManager.PopupCount == 0 && !PopupManager.Acting)
		{
			Scene outScene = ((currentSceneDM == null) ? Scene._NULL : currentSceneDM._scene);
			currentSceneDM = sceneModel;
			Log.DebugTag("PopToScene->GoToScene: " + scene, null, "scenetransitionmanager");
			GotoScene(outScene, scene, SceneTransition.Type.Push);
		}
		else
		{
			Log.DebugTag("PopToScene->pendingPopScene: " + scene, null, "scenetransitionmanager");
			pendingPopScene = sceneModel;
		}
	}

	private static void GotoScene(Scene outScene, Scene inScene, SceneTransition.Type type)
	{
		if (instance != null)
		{
			throw new Exception("Scene transition already taking place");
		}
		GameObject gameObject = new GameObject("SceneTransitionManager");
		instance = gameObject.AddComponent<SceneTransitionManager>();
		Log.InfoTag("GotoScene: outScene: {0}, inScene: {1}, navigation: {2}", null, "scenetransitionmanager", outScene, inScene, GetNavigationHistory());
		instance.transitionType = type;
		readyToTransitionIn = false;
		transitionActive = true;
		instance.StartCoroutine(instance.loadLevelAsync(inScene));
	}

	public static void SetSceneInstant(Scene newScene)
	{
		UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(SceneController));
		for (int i = 0; i < array.Length; i++)
		{
			SceneController sceneController = (SceneController)array[i];
			UnityEngine.Object.Destroy(sceneController.gameObject);
		}
		SceneTransitionManager obj = instance;
		GameObject gameObject = new GameObject("SceneTransitionManager");
		instance = gameObject.AddComponent<SceneTransitionManager>();
		instance.transitionType = SceneTransition.Type.None;
		readyToTransitionIn = false;
		transitionActive = true;
		instance.StartCoroutine(instance.loadLevelAsync(newScene, true));
		currentSceneDM = null;
		UnityEngine.Object.DestroyImmediate(obj);
	}

	private static string GetNavigationHistory()
	{
		SceneModel previous = currentSceneDM;
		if (previous == null)
		{
			return string.Empty;
		}
		List<string> list = new List<string>();
		list.Add(previous._scene.ToString());
		while ((previous = previous._previous) != null)
		{
			list.Add(previous._scene.ToString());
		}
		list.Reverse();
		string[] array = new string[list.Count];
		list.CopyTo(array);
		return string.Join("->", array);
	}

	public static void RegisterSceneRoot(SceneController o)
	{
		if ((bool)instance)
		{
			instance.allNewSceneRoots.Add(o);
			instance.pendingNewSceneRoots.Add(o);
			o.OnPrepareTransitionIn(instance.transitionType);
		}
	}

	private IEnumerator loadLevelAsync(Scene scene, bool cleanupOnComplete = false)
	{
		SceneTransition.Type outType = SceneTransition.Type.None;
		switch (transitionType)
		{
		case SceneTransition.Type.Pop:
			outType = SceneTransition.Type.Push;
			break;
		case SceneTransition.Type.Push:
			outType = SceneTransition.Type.Pop;
			break;
		case SceneTransition.Type.Overlay:
			outType = SceneTransition.Type.None;
			break;
		case SceneTransition.Type.None:
			outType = SceneTransition.Type.None;
			break;
		default:
			throw new Exception("Invalid transition type");
		}
		if (outType != SceneTransition.Type.None)
		{
			object[] oldObjects = UnityEngine.Object.FindObjectsOfType(typeof(SceneController));
			object[] array = oldObjects;
			for (int i = 0; i < array.Length; i++)
			{
				SceneController o = (SceneController)array[i];
				if (o.destroyOnTransition && !o.Destroyed)
				{
					allOldSceneRoots.Add(o);
					pendingOldSceneRoots.Add(o);
					o.OnPrepareTransitionOut(outType);
				}
			}
			foreach (SceneController o2 in allOldSceneRoots)
			{
				o2.OnBeginTransitionOut();
			}
			foreach (Action a in transitionOutBeginHandlers)
			{
				a();
			}
			transitionOutBeginHandlers.Clear();
		}
		yield return 0;
		while (pendingOldSceneRoots != null && pendingOldSceneRoots.Count != 0)
		{
			yield return 0;
		}
		float t1 = Time.realtimeSinceStartup;
		yield return Application.LoadLevelAdditiveAsync(scene.ToString());
		Log.Info("LoadAsync in: {0}ms", (Time.realtimeSinceStartup - t1) * 1000f);
		sceneLoaded = true;
		yield return 0;
		yield return 0;
		Singleton<AssetBundleManager>.instance.UnloadUnusedBundles();
		yield return Resources.UnloadUnusedAssets();
		GC.Collect();
		if (cleanupOnComplete)
		{
			instance.AllTransitionInDone();
		}
		ProcessSceneLoadDone();
	}

	private void Update()
	{
		if (pendingOldSceneRoots != null)
		{
			if (pendingOldSceneRoots.Count == 0 && sceneLoaded && !startedTransitionIn)
			{
				startedTransitionIn = true;
				StartCoroutine(AllTransitionOutDone());
			}
			else
			{
				for (int num = pendingOldSceneRoots.Count - 1; num >= 0; num--)
				{
					pendingOldSceneRoots[num].PumpTransitions();
				}
			}
		}
		if (pendingNewSceneRoots == null)
		{
			return;
		}
		if (pendingNewSceneRoots.Count == 0 && sceneLoaded && startedTransitionIn)
		{
			AllTransitionInDone();
			return;
		}
		for (int num2 = pendingNewSceneRoots.Count - 1; num2 >= 0; num2--)
		{
			pendingNewSceneRoots[num2].PumpTransitions();
		}
	}

	public static void FinishedTransitionOut(SceneController o)
	{
		if (!instance)
		{
			throw new Exception("Not transitioning");
		}
		if (instance.startedTransitionIn)
		{
			throw new Exception("FinishedTransitionOut before starting transition in");
		}
		instance.pendingOldSceneRoots.Remove(o);
	}

	private IEnumerator AllTransitionOutDone()
	{
		foreach (SceneController o in allOldSceneRoots)
		{
			o.OnEndTransitionOut();
		}
		allOldSceneRoots = null;
		pendingOldSceneRoots = null;
		foreach (SceneController o2 in allNewSceneRoots)
		{
			o2.OnWarmTransitionIn();
		}
		yield return 0;
		Coroutine[] coroutines = new Coroutine[transitionInPrepareHandlers.Count];
		for (int i = 0; i < coroutines.Length; i++)
		{
			coroutines[i] = StartCoroutine(transitionInPrepareHandlers[i]());
		}
		transitionInPrepareHandlers.Clear();
		Coroutine[] array = coroutines;
		for (int j = 0; j < array.Length; j++)
		{
			yield return array[j];
			yield return 0;
		}
		for (int k = 0; k < transitionOutDoneHandlers.Count; k++)
		{
			transitionOutDoneHandlers[k]();
		}
		transitionOutDoneHandlers.Clear();
		while (!readyToTransitionIn)
		{
			yield return 0;
		}
		foreach (SceneController o3 in allNewSceneRoots)
		{
			o3.OnBeginTransitionIn();
		}
	}

	public static void FinishedTransitionIn(SceneController o)
	{
		if (!instance)
		{
			throw new Exception("Not transitioning");
		}
		if (!instance.startedTransitionIn)
		{
			throw new Exception("FinishedTransitionIn when already started transition in");
		}
		instance.pendingNewSceneRoots.Remove(o);
	}

	private void AllTransitionInDone()
	{
		foreach (SceneController allNewSceneRoot in allNewSceneRoots)
		{
			allNewSceneRoot.OnEndTransitionIn();
		}
		transitionActive = false;
		allNewSceneRoots = null;
		pendingNewSceneRoots = null;
		UnityEngine.Object.Destroy(base.gameObject);
		instance = null;
		for (int i = 0; i < transitionInDoneHandlers.Count; i++)
		{
			transitionInDoneHandlers[i]();
		}
		transitionInDoneHandlers.Clear();
		ProcessPendingScenes();
	}

	public static void NextMultiScene()
	{
		NextMultiScene(sceneAfterMultiScene);
	}

	public static void NextMultiScene(Scene scene)
	{
		sceneAfterMultiScene = scene;
		if (multiSceneCounter == 0)
		{
			if (CurrentSceneDM._scene != Scene.HomeScene)
			{
				PushToScene(sceneAfterMultiScene, null, false);
				sceneAfterMultiScene = Scene.Default;
			}
		}
		else
		{
			multiSceneCounter--;
		}
	}

	public static void ProcessSceneLoadDone()
	{
		if (OnLoadSceneDoneCallback != null)
		{
			OnLoadSceneDoneCallback();
		}
		OnLoadSceneDoneCallback = null;
	}
}
