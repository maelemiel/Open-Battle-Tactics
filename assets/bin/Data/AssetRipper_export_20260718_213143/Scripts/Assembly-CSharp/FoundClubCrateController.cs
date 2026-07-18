using System.Collections;
using Holoville.HOTween;
using UnityEngine;

public class FoundClubCrateController : SceneController
{
	[SerializeField]
	private tk2dTextMesh crateSentToText;

	[SerializeField]
	private ObjectShaker screenshakeTarget;

	[SerializeField]
	private GameObject crateGameObject;

	[SerializeField]
	protected tk2dTextMesh tapToContinueText;

	public override void Awake()
	{
		base.Awake();
	}

	private void Start()
	{
		SceneTransitionManager.readyToTransitionIn = true;
		if ((bool)TopBarController.instance)
		{
			TopBarController.instance.ShowProgressBanner = false;
			TopBarController.instance.Visible = false;
		}
		if ((bool)tapToContinueText)
		{
			tapToContinueText.Alpha = 0f;
		}
		AudioTrigger.StandardCrowd.PlayMusic();
		AudioTrigger.CrowdHush.Play();
		Singleton<InitializationManager>.instance.ExecuteOnState(InitializationManager.State.OnlineReady, delegate
		{
			Init();
		});
	}

	private void Init()
	{
		ClubCrateSceneModel clubCrateSceneModel = null;
		string empty = string.Empty;
		if (sceneModel == null)
		{
			Debug.LogWarning("No user!! Creating fake user as crate recepient..");
			empty = "Some fake user";
		}
		else
		{
			clubCrateSceneModel = (ClubCrateSceneModel)sceneModel;
			empty = clubCrateSceneModel.recepientUsername;
		}
		crateSentToText.text = empty;
		StartCoroutine(IntroAnimation());
	}

	private void Update()
	{
		if ((bool)tapToContinueText)
		{
			tapToContinueText.Alpha = Mathf.Min(1f, Mathf.PingPong(Time.time * 2f, 1.5f));
		}
	}

	private IEnumerator IntroAnimation()
	{
		float waitTime = 0.8f;
		float delayTime = 0f;
		Vector3 originalPosition = crateGameObject.transform.localPosition;
		crateGameObject.transform.localPosition = originalPosition + new Vector3(0f, 1000f, 0f);
		HOTween.To(crateGameObject.transform, waitTime, new TweenParms().Prop("localPosition", originalPosition).Delay(delayTime).Ease(EaseType.EaseInExpo)
			.OnComplete(OnBoxLanded));
		delayTime += Random.Range(0.1f, 0.3f);
		yield return new WaitForSeconds(delayTime + 0.3f);
		AudioTrigger.CrowdCheering.Play();
	}

	private void OnBoxLanded()
	{
		screenshakeTarget.Shake();
		AudioTrigger.CrateLand.Play();
	}

	private void TransitionOut()
	{
		if ((bool)tapToContinueText)
		{
			tapToContinueText.Alpha = 0f;
			AudioTrigger.TapToContinue.Play();
		}
		SceneTransitionManager.ClearHistory();
		if (UserProfile.player.tutorial.CurrentStep <= TutorialStep.BuildFirstTank)
		{
			SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.BlueprintsScene, null, false);
		}
		else
		{
			SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ArenaScene, null, false);
		}
	}
}
