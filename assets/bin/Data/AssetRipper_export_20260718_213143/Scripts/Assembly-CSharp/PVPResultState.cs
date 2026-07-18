using System;
using System.Collections;
using Holoville.HOTween;
using UnityEngine;

public class PVPResultState : SceneState
{
	private const float HORIZONTAL_FINAL_X_POSITION_PLAYER_VIEW = 190f;

	[SerializeField]
	private GameObject _victoryGameObject;

	[SerializeField]
	private GameObject _defeatGameObject;

	[SerializeField]
	private PlayerView playerView;

	[SerializeField]
	private PlayerView opponentView;

	[SerializeField]
	private tk2dSprite startbustSprite;

	[SerializeField]
	private float timeToAppear = 0.75f;

	private BattleRewardsSceneModel _sceneModel;

	public override void InitSequence(object dataObject)
	{
		base.InitSequence(dataObject);
		_sceneModel = (BattleRewardsSceneModel)dataObject;
		SetBackgroundState(_sceneModel.isPlayerWinner);
		ClearScene();
	}

	public override IEnumerator PlayStateSequence(Action<PostBattleRewardsStates, object> callback)
	{
		yield return StartCoroutine(DisplayResultsSequence(callback));
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
		isFinished = true;
	}

	private void ClearScene()
	{
	}

	private IEnumerator DisplayResultsSequence(Action<PostBattleRewardsStates, object> callback)
	{
		base.callback = callback;
		if ((bool)startbustSprite)
		{
			startbustSprite.transform.parent = ((!_sceneModel.isPlayerWinner) ? opponentView.transform : playerView.transform);
			startbustSprite.transform.localPosition = Vector3.zero;
		}
		if ((bool)playerView && _sceneModel.playerData != null)
		{
			playerView.ConfigureView(_sceneModel.playerData, _sceneModel.playerStats.isWinner, _sceneModel.deltaPlayerPVP);
			playerView.transform.TweenLocalXPosition(-190f, timeToAppear);
		}
		if ((bool)opponentView && _sceneModel.enemyData != null)
		{
			opponentView.ConfigureView(_sceneModel.enemyData, _sceneModel.playerStats.isWinner, -_sceneModel.deltaPlayerPVP);
			opponentView.transform.TweenLocalXPosition(190f, timeToAppear);
		}
		yield return new WaitForSeconds(1f);
		isFinished = true;
	}

	public override IEnumerator EndSequence(float delay)
	{
		yield return StartCoroutine(base.EndSequence(delay));
		PostBattleRewardsStates nextState = PostBattleRewardsStates.REWARDS_PARTS;
		object dataModel = _sceneModel;
		if (callback != null)
		{
			callback(nextState, dataModel);
		}
	}

	private void SetBackgroundState(bool state)
	{
		if ((bool)_victoryGameObject)
		{
			_victoryGameObject.SetActive(state);
		}
		if ((bool)_defeatGameObject)
		{
			_defeatGameObject.SetActive(!state);
		}
	}
}
