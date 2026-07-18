using UnityEngine;

public class ServersSelectionPopUpController : SceneController
{
	private const string CACHEDUSERIDKEY = "id";

	public tk2dTextMesh[] guiButtonLabels;

	public tk2dTextMesh customUrlLabel;

	public tk2dTextMesh customUrlLabel2;

	public tk2dUIToggleButtonGroup _serverSelectGroup;

	public GameObject resetAccountButton;

	public GameObject loginAccountButton;

	public static bool HasSelectedServer { get; private set; }

	public override void Awake()
	{
		_showTopBar = false;
		base.Awake();
		resetAccountButton.SetActive((CachedUserID() != null) ? true : false);
		loginAccountButton.SetActive(false);
	}

	private void Init()
	{
	}

	private void Start()
	{
		SceneTransitionManager.readyToTransitionIn = true;
		Singleton<InitializationManager>.instance.ExecuteOnState(InitializationManager.State.OnlineReady, delegate
		{
			Init();
		});
	}

	private void UseNobage()
	{
		AppConfig.useNobage = true;
	}

	private void ResetTutorial()
	{
		new UserTutorialData(null).CurrentStep = TutorialStep.PickTank;
		ClearDialogTriggerData();
	}

	private void SkipTutorial()
	{
		new UserTutorialData(null).SkipTutorial();
	}

	private void ClearDialogTriggerData()
	{
		UserDialogTriggerData userDialogTriggerData = new UserDialogTriggerData(null);
		userDialogTriggerData.ClearTriggers();
	}

	private void OnPlayClicked(tk2dUIItem button)
	{
		Debug.Log("going to connect at: " + AppConfig.Server);
		HasSelectedServer = true;
		AppConfig.SaveCurrentEnvironment(AppConfig.currentEnvironmentType);
		Destroy();
	}

	private void Destroy()
	{
		Object.Destroy(base.gameObject);
	}

	public void OnLoginClicked()
	{
		Singleton<SessionManager>.instance.Init();
	}

	private string CachedUserID()
	{
		string result = null;
		KeyValueStorage keyValueStorage = KeyValueStorage.Instance(KeyValueStorage.Storage.USER_PROFILE);
		if (keyValueStorage.ContainsKey("id"))
		{
			result = keyValueStorage.GetValue<string>("id");
		}
		return result;
	}

	private void RemoveCachedUserID()
	{
		KeyValueStorage keyValueStorage = KeyValueStorage.Instance(KeyValueStorage.Storage.USER_PROFILE);
		if (keyValueStorage.ContainsKey("id"))
		{
			Debug.Log("Removing key: [id]");
			keyValueStorage.Remove("id");
		}
	}

	public void OnResetAccountClicked()
	{
		Debug.Log("OnResetAccountClicked");
		string text = CachedUserID();
		if (text != null)
		{
			string url = AppConfig.Server + "/debug/user/invalidate?user_id=" + text;
			NetworkQueue.Request request = new NetworkQueue.Request(url, "GET", AppConfig.networkRetries);
			Singleton<NetworkQueue>.instance.Enqueue(request);
			RemoveCachedUserID();
			resetAccountButton.SetActive(false);
		}
	}
}
