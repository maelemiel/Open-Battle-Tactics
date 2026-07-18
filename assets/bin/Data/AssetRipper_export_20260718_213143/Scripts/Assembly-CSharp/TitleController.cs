public class TitleController : SceneController
{
	private static bool hasInitialized;

	public override void Awake()
	{
		Log.DebugTag("Initializing Adjust SDK", null, "AdjustSDK");
		Adjust.appDidLaunch(AppConfig.adjustToken, AppConfig.adjustEnviroment, AppConfig.adjustLogInfo, false);
		if (!hasInitialized)
		{
			Reporting.GameStartEvent();
			CrittercismUtil.Init();
			hasInitialized = true;
		}
		else
		{
			UserProfile.player = null;
			Singleton<SessionManager>.instance.Disconnect();
			TopBarController.instance.Reinitialize();
			LoadingPopupManager.ClearAll();
			CrittercismUtil.LeaveBreadcrumb("Restart Session");
			Reporting.enableStartupFunnel = false;
			Singleton<InitializationManager>.instance.GoOnline();
		}
		_showTopBar = false;
		base.Awake();
		Singleton<InitializationManager>.instance.ExecuteOnState(InitializationManager.State.OnlineReady, Init);
		Kamcord.videoSharedTo += delegate(string kamcordVideoID, string networkName, bool success)
		{
			Reporting.KamcordShare(networkName, success);
		};
	}

	private void Init()
	{
		if (UserProfile.player.tutorial.IsInTutorial)
		{
			UserProfile.player.tutorial.GotoTutorialScene();
		}
		else
		{
			SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.HomeScene, null, false);
		}
	}
}
