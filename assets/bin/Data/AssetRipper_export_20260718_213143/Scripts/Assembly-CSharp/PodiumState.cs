using System;
using System.Collections;
using Holoville.HOTween;
using UnityEngine;

public class PodiumState : SceneState
{
	private const string ANNOUNCER_SAYS_WIN = "you won!";

	private const string ANNOUNCER_SAYS_LOSE = "Better luck next time.";

	public float panUpTime = 2f;

	public float finalYPosition = 30f;

	[SerializeField]
	private UnitProxy[] playerUnitProxies;

	[SerializeField]
	private GameObject[] wreckObjects;

	[SerializeField]
	private tk2dSpineAnimation announcerAnimation;

	[SerializeField]
	private GameObject backgroundContent;

	[SerializeField]
	private tk2dSprite stateBackground;

	private BattleRewardsSceneModel _sceneModel;

	public override void InitSequence(object dataObject)
	{
		base.InitSequence(dataObject);
		_sceneModel = (BattleRewardsSceneModel)dataObject;
		bool flag = _sceneModel == null || _sceneModel.isPlayerWinner;
		if ((bool)stateBackground)
		{
			stateBackground.SetSprite((!flag) ? PostBattleRewardsSceneController.BACKGROUND_DISABLED_SPRITENAME : PostBattleRewardsSceneController.BACKGROUND_ENABLED_SPRITENAME);
		}
	}

	public override IEnumerator PlayStateSequence(Action<PostBattleRewardsStates, object> callback)
	{
		base.callback = callback;
		_sceneModel = (BattleRewardsSceneModel)dataObject;
		bool didIWin = _sceneModel.isPlayerWinner;
		for (int i = 0; i < playerUnitProxies.Length; i++)
		{
			if (didIWin)
			{
				StartCoroutine(playerUnitProxies[i].ChangeAssetCoroutine(UserProfile.player.CurrentTeam.units[i].AssetBundleID));
			}
			else
			{
				wreckObjects[i].gameObject.SetActive(true);
			}
		}
		yield return new WaitForSeconds(0.25f);
		if ((bool)backgroundContent)
		{
			backgroundContent.transform.TweenLocalYPosition(finalYPosition, panUpTime, EaseType.EaseInCubic);
		}
		yield return new WaitForSeconds(panUpTime);
		TopBarController.instance.Visible = true;
		isFinished = true;
	}

	public override void SkipToEnd()
	{
		base.SkipToEnd();
		if (isFinished)
		{
			StartCoroutine(EndSequence(0f));
			return;
		}
		HOTween.Complete();
		Transform transform = backgroundContent.transform;
		Vector3 localPosition = new Vector3(transform.localPosition.x, finalYPosition, transform.localPosition.z);
		transform.localPosition = localPosition;
		isFinished = true;
	}

	public override IEnumerator EndSequence(float delay)
	{
		yield return StartCoroutine(base.EndSequence(delay));
		PostBattleRewardsStates nextState = PostBattleRewardsStates.REWARDS_PARTS;
		if (callback != null)
		{
			callback(nextState, null);
		}
	}
}
