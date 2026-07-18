using System.Collections;
using UnityEngine;

public class AudiencePanShotSceneController : SceneController
{
	[SerializeField]
	private float panDuration = 4f;

	[SerializeField]
	private bool skipLogin;

	public override void Awake()
	{
		if (!skipLogin)
		{
			base.Awake();
		}
	}

	private void Start()
	{
		SceneTransitionManager.readyToTransitionIn = true;
		TopBarController.instance.Visible = false;
		AudioTrigger.StandardCrowd.PlayMusic();
		Singleton<InitializationManager>.instance.ExecuteOnState(InitializationManager.State.OnlineReady, delegate
		{
			Init();
		});
	}

	private void Init()
	{
		StartCoroutine(LoadBattleSceneWithDelay(panDuration));
	}

	private IEnumerator LoadBattleSceneWithDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.BattleScene, sceneModel.payload as BattleSceneModel);
	}
}
