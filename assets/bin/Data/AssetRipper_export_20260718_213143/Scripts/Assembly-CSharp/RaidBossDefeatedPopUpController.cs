public class RaidBossDefeatedPopUpController : PopupController
{
	private new void Start()
	{
	}

	private new void Update()
	{
	}

	private void OnConfirm()
	{
		Singleton<SessionManager>.instance.ClaimEnergy();
		EventDataModel activeEvent = UserProfile.player.GetActiveEvent();
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ArenaSelectionScene, new ArenaSelectionSceneModel(activeEvent));
		Close();
	}
}
