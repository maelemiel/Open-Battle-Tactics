public class TemplateSceneController : SceneController
{
	public override void Awake()
	{
		_showHomeButton = true;
		_showTopBar = true;
		base.Awake();
		base.SectionTitle = "Template Scene";
	}

	public override bool OnHomeButton()
	{
		return true;
	}

	private void Start()
	{
		SceneTransitionManager.readyToTransitionIn = true;
		Singleton<InitializationManager>.instance.ExecuteOnState(InitializationManager.State.OnlineReady, delegate
		{
			Init();
		});
	}

	private void Init()
	{
	}
}
