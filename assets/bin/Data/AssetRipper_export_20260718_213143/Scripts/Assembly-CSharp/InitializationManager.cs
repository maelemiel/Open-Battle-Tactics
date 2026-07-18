using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using Holoville.HOTween;
using LCD;
using LCD.User;
using LitJson0;
using UnityEngine;

public class InitializationManager : Singleton<InitializationManager>
{
	public enum State
	{
		Booting = 0,
		BootReady = 1,
		OnlineReady = 2
	}

	public enum StartupStep
	{
		UnbundleDataModel = 0,
		ConnectDataModel = 1,
		MobageLogin = 2,
		RequestToken = 3,
		AuthorizeToken = 4,
		Login = 5,
		UpdateDataModel = 6,
		Done = 7
	}

	public delegate void DataModelUpdatedHandler();

	public delegate void StateChangedHandler(State currentState);

	private State currentState;

	public string persistentDataPath;

	public string streamingAssetsPath;

	public int dataModelVersion;

	public string dataModelHash;

	public string dataModelAssetUrl;

	public State CurrentState
	{
		get
		{
			return currentState;
		}
	}

	public event DataModelUpdatedHandler DataModelUpdated;

	private event StateChangedHandler stateChanged;

	public event StateChangedHandler StateChanged
	{
		add
		{
			this.stateChanged = (StateChangedHandler)Delegate.Combine(this.stateChanged, value);
			value(currentState);
		}
		remove
		{
			this.stateChanged = (StateChangedHandler)Delegate.Remove(this.stateChanged, value);
		}
	}

	private void Awake()
	{
		Log.InfoTag("Tank T.V. startup. Enjoy your stay!", null, "InitializationManager");
		Application.targetFrameRate = 60;
		Screen.sleepTimeout = -2;
		DeNA_LoggerInterface.InitializeLog();
		Input.gyro.enabled = false;
		AppDomain.CurrentDomain.UnhandledException += delegate(object sender, UnhandledExceptionEventArgs e)
		{
			Log.ErrorTag("Unhandled exception thrown from: {0}, Error: {1}", null, "InitializationManager", sender.GetType().Name, e.ExceptionObject.ToString());
		};
		Singleton<LCDController>.Instantiate();
		persistentDataPath = Application.persistentDataPath;
		streamingAssetsPath = Application.streamingAssetsPath;
	}

	private IEnumerator Start()
	{
		AppConfig.InitManager = this;
		yield return StartCoroutine(FetchProductionURL());
		if (Application.loadedLevel == 0)
		{
			yield return StartCoroutine(Orchestrator(false));
			yield return StartCoroutine(GoOnlineCoroutine());
		}
		else
		{
			yield return StartCoroutine(Orchestrator());
		}
		SetFrameRate();
	}

	private void Update()
	{
		CrittercismUtil.Update();
		if (!Application.isLoadingLevel)
		{
			CheckBackButton();
		}
	}

	private void SetFrameRate()
	{
		if (UserProfile.player.preferences.BatterySaverOn)
		{
			Application.targetFrameRate = 30;
		}
		else
		{
			Application.targetFrameRate = 60;
		}
	}

	private void CheckBackButton()
	{
		if (!Input.GetKeyDown(KeyCode.Escape))
		{
			return;
		}
		if ((bool)TopBarController.instance)
		{
			if (AnnouncerController.IsDialogVisible())
			{
				return;
			}
			if (TopBarController.instance.MenuBar.IsOpen)
			{
				TopBarController.instance.MenuBar.IsOpen = false;
				return;
			}
		}
		if (PopupManager.PopupCount > 0)
		{
			PopupDataModel currentPopupDM = PopupManager.CurrentPopupDM;
			if (currentPopupDM != null && (bool)currentPopupDM.controller)
			{
				currentPopupDM.controller.OnBackButtonPressed();
			}
		}
		else if (SceneTransitionManager.CurrentSceneDM != null && !SceneTransitionManager.transitionActive)
		{
			SceneController controller = SceneTransitionManager.CurrentSceneDM.controller;
			controller.OnBackButton();
		}
	}

	private IEnumerator FetchProductionURL(bool isRetryAttempt = false)
	{
		if (isRetryAttempt)
		{
			bool retry = false;
			PopupManager.ShowPopup(PopupDataModel.ProdURLError(delegate
			{
				retry = true;
			}));
			while (!retry)
			{
				yield return 0;
			}
		}
		string url = AppConfig.ProductionMapURL + "?nocache=" + UnityEngine.Random.Range(0, int.MaxValue);
		WWW prodUrlWWW = new WWW(url);
		Log.DebugTag("Requesting Production URL from " + url, null, "InitializationManager");
		yield return prodUrlWWW;
		if (prodUrlWWW.error != null)
		{
			StartCoroutine(ThrowProdURLError("Error fetching production URL! " + prodUrlWWW.error));
			yield return StartCoroutine(FetchProductionURL(true));
			yield break;
		}
		Log.DebugTag("Production URL Map:\n" + prodUrlWWW.text, null, "InitializationManager");
		JsonObject json = JsonMapper.ToObject(prodUrlWWW.text);
		JsonObject platformObject = json.GetObject("android");
		if (platformObject == null)
		{
			StartCoroutine(ThrowProdURLError("Cannot find production url for the given platform"));
			yield return StartCoroutine(FetchProductionURL(true));
			yield break;
		}
		JsonObject clientVersionObject = platformObject.GetObject(AppConfig.clientVersion);
		if (clientVersionObject == null)
		{
			clientVersionObject = platformObject.GetObject("default");
		}
		if (clientVersionObject == null)
		{
			StartCoroutine(ThrowProdURLError("Cannot get client version info."));
			yield return StartCoroutine(FetchProductionURL(true));
			yield break;
		}
		string serverURL = clientVersionObject.GetString("server");
		Log.DebugTag("Setting production URL to '" + serverURL + "'", null, "InitializationManager");
		AppConfig.ServerProd = serverURL;
		AppConfig.UpdateEnvironmentURL(AppConfig.EnvironmentType.Production, AppConfig.ServerProd);
	}

	private IEnumerator ThrowProdURLError(string description)
	{
		yield return 0;
		CrittercismUtil.LeaveBreadcrumb("prod_url_error " + description);
		throw new Exception("Error fetching production URL!");
	}

	private void OnStateChanged(State newState)
	{
		Log.InfoTag("InitializationManager: State changed to: {0}", null, "InitializationManager", newState.ToString());
		Log.InfoTag("SessionManager: State : {0}", null, "InitializationManager", Singleton<SessionManager>.instance.connectedState.ToString());
		currentState = newState;
		if (this.stateChanged != null)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			Log.InfoTag("InitializationManager.OnStateChanged: Calling {0} callbacks", null, "InitializationManager", newState.ToString());
			this.stateChanged(currentState);
			Log.InfoTag("InitializationManager.OnStateChanged: {0} callbacks done, took: {1}", null, "InitializationManager", newState.ToString(), stopwatch.ElapsedMilliseconds);
		}
	}

	public void StartOrchestrator()
	{
		StartCoroutine(Orchestrator());
	}

	public IEnumerator Orchestrator(bool goOnline = true)
	{
		TestTimeUtility.AddLoginRequest("START", "Orchestrator: UnbundleRequiredFiles");
		yield return StartCoroutine(UnbundleRequiredFiles());
		TestTimeUtility.AddLoginRequest("READY", "Orchestrator: UnbundleRequiredFiles");
		SetEnvironment();
		bool facebookInitialized = false;
		Singleton<SocialManager>.instance.ConfigureFacebook(delegate
		{
			facebookInitialized = true;
		});
		while (!facebookInitialized)
		{
			yield return 0;
		}
		Singleton<BankService>.Instantiate();
		Singleton<BankService>.instance.gameObject.AddComponent<BankPopupHandler>();
		yield return StartCoroutine(BootSequence());
		if (goOnline)
		{
			yield return StartCoroutine(GoOnlineCoroutine());
		}
		int loadingPopupId = LoadingPopupManager.ShowLoadingPopup(0f);
		LoadingPopupManager.ClearLoadingPopup(loadingPopupId);
	}

	private void SetEnvironment()
	{
		AppConfig.InitEnvironment();
		AppConfig.LoadCurrentEnvironment();
	}

	public void GoOnline()
	{
		StartCoroutine(GoOnlineCoroutine());
	}

	private IEnumerator GoOnlineCoroutine(bool showLoadingPopup = true)
	{
		int loadingPopupId = LoadingPopupManager.ShowLoadingPopup(0f);
		Log.InfoTag("InitializationManager.Orchestrator: Starting OnlineSequence", null, "InitializationManager");
		Stopwatch stopwatch = Stopwatch.StartNew();
		yield return StartCoroutine(OnlineSequence());
		Log.InfoTag("InitializationManager.Orchestrator: OnlineSequence took: " + stopwatch.ElapsedMilliseconds, null, "InitializationManager");
		LoadingPopupManager.ClearLoadingPopup(loadingPopupId);
		if (UserProfile.player != null && Constants.UseOtherLevelsLogin)
		{
			Log.DebugTag("Initializing Other Levels", null, "OtherLevels");
			OtherLevelsHelper.SetTrackingID(UserProfile.player);
		}
		OnStateChanged(State.OnlineReady);
	}

	private IEnumerator UnbundleRequiredFiles()
	{
		string error = null;
		string to = Path.Combine(persistentDataPath, "keyValue.db");
		if (!File.Exists(to))
		{
			string from = Path.Combine(streamingAssetsPath, "keyValue.db");
			Log.DebugTag("Unbundling file: " + from, null, "InitializationManager");
			yield return StartCoroutine(UnbundleUtility.UnbundleCoroutine(from, to, delegate(string inError)
			{
				error = inError;
			}));
			if (!string.IsNullOrEmpty(error))
			{
				Log.Error("Error unbundling keyValue.db: " + error);
				PopupManager.ShowPopup(PopupDataModel.FileIOError());
				yield break;
			}
			DeNAiCloudUtil.SetNoBackupFlag(to);
		}
		KeyValueStorage.Init();
		if (!Singleton<DataModelFile>.instance.UnbundleNeeded())
		{
			yield break;
		}
		bool done = false;
		Singleton<DataModelFile>.instance.UnbundleDynamic(delegate(string outError)
		{
			done = true;
			error = outError;
		});
		while (!done)
		{
			yield return 0;
		}
		if (!string.IsNullOrEmpty(error))
		{
			Log.Error("Error unbundling keyValue.db: " + error);
			if (error.ToLower().Contains("webexception"))
			{
				PopupManager.ShowPopup(PopupDataModel.NetworkError(null));
			}
			else
			{
				PopupManager.ShowPopup(PopupDataModel.FileIOError());
			}
		}
	}

	private IEnumerator BootSequence()
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		Log.InfoTag("InitializationManager.Orchestrator: Starting BootSequence", null, "InitializationManager");
		int loadingPopupId = LoadingPopupManager.ShowLoadingPopup(0f);
		Singleton<DataModelQueue>.Instantiate();
		Log.InfoTag("InitializationManager.BootSequence: DataModelConnect Start", null, "InitializationManager");
		Reporting.StartupFunnelEvent(StartupStep.ConnectDataModel);
		yield return StartCoroutine(DataModelConnect());
		Log.InfoTag("InitializationManager.BootSequence: DataModelConnect Finish", null, "InitializationManager");
		Log.InfoTag("InitializationManager.BootSequence: LocalizationManager Start", null, "InitializationManager");
		Singleton<LocalizationManager>.Instantiate();
		yield return StartCoroutine(Singleton<LocalizationManager>.instance.Init());
		Log.InfoTag("InitializationManager.BootSequence: LocalizationManager Finish", null, "InitializationManager");
		Singleton<AudioManager>.Instantiate();
		Singleton<AudioCacheManager>.Instantiate();
		Singleton<AudioCacheManager>.instance.Init();
		Singleton<GlobalEffectsManager>.Instantiate();
		InitTopBar();
		NonUnitySingleton<TimeManager>.Instantiate();
		Singleton<WorkQueue>.Instantiate();
		Singleton<NetworkQueue>.Instantiate();
		Singleton<UserProfileManager>.Instantiate();
		StartCoroutine(Reporting.SendReportsLoop());
		yield return StartCoroutine(Singleton<AssetBundleManager>.instance.Init());
		Singleton<SponsorPayManager>.Instantiate();
		LoadingPopupManager.ClearLoadingPopup(loadingPopupId);
		Log.InfoTag("InitializationManager.Orchestrator: BootSequence took: " + stopwatch.ElapsedMilliseconds, null, "InitializationManager");
		OnStateChanged(State.BootReady);
	}

	private IEnumerator OnlineSequence()
	{
		TestTimeUtility.AddLoginRequest("START", "Orchestrator: OnlineSequence");
		Singleton<SessionManager>.instance.Init();
		while (Singleton<SessionManager>.instance.connectedState != SessionManager.ConnectedState.Connected)
		{
			yield return 0;
		}
		Log.InfoTag("State Connected ", null, "InitializationManager");
		User lcdUser = LCDSDK.GetCurrentUser();
		CrittercismUtil.SetUsername(UserProfile.player.nickname);
		CrittercismUtil.SetMetadata(new string[3] { "lcdID", "playerID", "clientVer" }, new string[3]
		{
			lcdUser.userId.ToString(),
			UserProfile.player.id.ToString(),
			AppConfig.clientVersion
		});
		Log.InfoTag("Crittercism setup complete ", null, "InitializationManager");
		yield return new WaitForSeconds(1f);
		NonUnitySingleton<DMAccessManager>.instance.DataModelAccess.CacheRequiredTables();
		Log.InfoTag("DMAccessMananger cached Tables complete ", null, "InitializationManager");
		bool unitRequestDone = false;
		Singleton<SessionManager>.instance.GetUserUnits(true, delegate
		{
			unitRequestDone = true;
		});
		while (!unitRequestDone)
		{
			yield return 0;
		}
		Log.InfoTag("Unit request complete ", null, "InitializationManager");
		if (UserProfile.player.tutorial.CurrentStep >= TutorialStep.Complete)
		{
			bool firstUnitClaimed = false;
			UserProfile.player.TryClaimFirstUnit(delegate
			{
				firstUnitClaimed = true;
			});
			while (!firstUnitClaimed)
			{
				yield return 0;
			}
		}
		if (UserProfile.player.locale != Singleton<LocalizationManager>.instance.currentLanguageCode)
		{
			UserProfile.player.locale = Singleton<LocalizationManager>.instance.currentLanguageCode;
			Singleton<SessionManager>.instance.SendLocale(UserProfile.player.locale, null);
		}
		Singleton<SessionManager>.instance.PostLoginTasks();
		TestTimeUtility.AddLoginRequest("READY", "Orchestrator: OnlineSequence");
	}

	public void InitTopBar()
	{
		if (TopBarController.instance != null)
		{
			TopBarController.instance.Destroy();
		}
		LoadLevel(SceneTransitionManager.Scene.TopBarScene.ToString());
	}

	public void ExecuteOnState(State state, Action cb)
	{
		if (state <= currentState)
		{
			cb();
			return;
		}
		StateChangedHandler handler = delegate(State newState)
		{
			if (state <= newState)
			{
				StateChanged -= handler;
				cb();
			}
		};
		StateChanged += handler;
	}

	public void ExecuteIfStateEquals(State state, Action cb)
	{
		if (state == currentState)
		{
			cb();
			return;
		}
		StateChangedHandler handler = delegate(State newState)
		{
			if (state == newState)
			{
				StateChanged -= handler;
				cb();
			}
		};
		StateChanged += handler;
	}

	public void ClearStateChangeListeners()
	{
		if (this.stateChanged != null)
		{
			Delegate[] invocationList = this.stateChanged.GetInvocationList();
			for (int i = 0; i < invocationList.Length; i++)
			{
				StateChangedHandler value = (StateChangedHandler)invocationList[i];
				this.stateChanged = (StateChangedHandler)Delegate.Remove(this.stateChanged, value);
			}
		}
	}

	public IEnumerator DataModelConnect()
	{
		bool done = false;
		Singleton<DataModelQueue>.instance.Enqueue(DataModelQueue.Request.Connect(Singleton<DataModelFile>.instance.GetPath(), delegate(DataModelQueue.Response response)
		{
			if (response.error != null)
			{
				Singleton<DataModelFile>.instance.DiscardDynamic();
				throw new Exception("Error connecting to DataModel: " + response.error.description);
			}
			ConnectDataModel connectDataModel = response.dataModel as ConnectDataModel;
			dataModelVersion = connectDataModel.version;
			dataModelHash = connectDataModel.hash;
			dataModelAssetUrl = connectDataModel.assetUrl;
			Singleton<SessionManager>.instance.UpdateAssetBundleHost(dataModelAssetUrl);
			done = true;
		}));
		while (!done)
		{
			yield return 0;
		}
		if (this.DataModelUpdated != null)
		{
			this.DataModelUpdated();
		}
	}

	public void Restart()
	{
		StartCoroutine(RestartSequence());
	}

	private IEnumerator RestartSequence()
	{
		if (Kamcord.IsEnabled() && Kamcord.IsRecording())
		{
			Kamcord.StopRecording();
		}
		Log.DebugTag("Restarting Sequence ", null, "InitializationManager");
		OnStateChanged(State.BootReady);
		TimeManager.Reset();
		Singleton<DataModelQueue>.instance.Reset();
		Singleton<WorkQueue>.Instantiate();
		HOTween.Kill();
		TopBarController.instance.Hide(0f);
		Singleton<NetworkQueue>.instance.Reset();
		PopupManager.DestroyAllPopups();
		Singleton<UserProfileManager>.Instantiate();
		ClearStateChangeListeners();
		SceneTransitionManager.SetSceneInstant(SceneTransitionManager.Scene.TitleScene);
		yield break;
	}

	public static AsyncOperation LoadLevel(string levelName)
	{
		Log.DebugTag("Loading Level: " + levelName, null, "InitializationManager");
		if (AppConfig.offlineMode)
		{
			Application.LoadLevelAdditive(levelName);
			return null;
		}
		return Application.LoadLevelAdditiveAsync(levelName);
	}
}
