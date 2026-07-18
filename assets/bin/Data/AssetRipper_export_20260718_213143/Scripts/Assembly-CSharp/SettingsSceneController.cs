using UnityEngine;

public class SettingsSceneController : SceneController
{
	[SerializeField]
	private tk2dUIToggleButton musicToggle;

	[SerializeField]
	private tk2dUIToggleButton sfxToggle;

	private bool disableScene;

	[SerializeField]
	private GameObject videoReplaysButton;

	[SerializeField]
	private GameObject communityButton;

	public override void Awake()
	{
		allowsBackButton = true;
		base.Awake();
	}

	private void Start()
	{
		videoReplaysButton.SetActive(Kamcord.IsEnabled());
		if (!videoReplaysButton.activeSelf && communityButton != null)
		{
			communityButton.transform.position = Vector3.Lerp(videoReplaysButton.transform.position, communityButton.transform.position, 0.5f);
		}
		SceneTransitionManager.readyToTransitionIn = true;
		Singleton<InitializationManager>.instance.ExecuteOnState(InitializationManager.State.OnlineReady, delegate
		{
			Init();
		});
	}

	private void Init()
	{
		if ((bool)Singleton<AudioManager>.instance)
		{
			musicToggle.IsOn = Singleton<AudioManager>.instance.MusicVolume > 0f;
			sfxToggle.IsOn = Singleton<AudioManager>.instance.SfxVolume > 0f;
		}
	}

	public void OnTapMusic(tk2dUIToggleButton toggleControl)
	{
		int num = (toggleControl.IsOn ? 100 : 0);
		if ((bool)Singleton<AudioManager>.instance)
		{
			Singleton<AudioManager>.instance.MusicVolume = num;
		}
	}

	public void OnTapSfx(tk2dUIToggleButton toggleControl)
	{
		int num = (toggleControl.IsOn ? 100 : 0);
		if ((bool)Singleton<AudioManager>.instance)
		{
			Singleton<AudioManager>.instance.SfxVolume = num;
		}
	}

	public void OnClickNews()
	{
		if (!disableScene)
		{
			disableScene = true;
			SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.HomeScene);
		}
	}

	public void OnClickCheat()
	{
	}

	public void OnClickPickYourName()
	{
		if (!disableScene)
		{
			PopupManager.ShowPopup(new PopupDataModel(), SceneTransitionManager.Scene.PickYourName);
		}
	}

	public void OnClickTellAFriend()
	{
		if (!disableScene)
		{
			PopupManager.ShowPopup(PopupDataModel.TellAFriendPopUp());
		}
	}

	public void ClickAccountSettings()
	{
		if (!disableScene)
		{
			PopupManager.ShowPopup(PopupDataModel.AccountSettingsPopUp());
		}
	}

	public void ClickNotificationSettings()
	{
		if (!disableScene)
		{
			PopupManager.ShowPopup(PopupDataModel.NotificationSettingsPopUp());
		}
	}

	public void ClickPerformanceSettings()
	{
		PopupManager.ShowPopup(PopupDataModel.PerformanceSettingsPopUp());
	}

	public void ClickVideoSharing()
	{
		Kamcord.ShowWatchView();
	}

	public void ClickForums()
	{
		Application.OpenURL("forums_url".Localize("http://forums.dena.com/forumdisplay.php/15-Super-Battle-Tactics"));
	}

	public void ClickHelp()
	{
		PopupManager.ShowPopup(new PopupDataModel(), SceneTransitionManager.Scene.HelpPopUp);
	}
}
