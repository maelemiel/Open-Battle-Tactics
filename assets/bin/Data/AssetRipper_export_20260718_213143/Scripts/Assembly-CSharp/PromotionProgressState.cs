using System;
using System.Collections;
using Holoville.HOTween;
using UnityEngine;

public class PromotionProgressState : SceneState
{
	private const float TROPHY_SCALE = 1f;

	private const float DEFEAT_SCALE = 1f;

	public tk2dTextMesh _titleText;

	public tk2dSprite[] _trophySprites;

	public tk2dSprite[] _emptyTrophySprites;

	public tk2dSprite[] _defeatSprites;

	public GameObject _starBust;

	public Vector3 _starBustOffset;

	[SerializeField]
	private tk2dSprite stateBackground;

	private PromotionSeriesProgressSceneModel _sceneModel;

	public override void InitSequence(object dataObject)
	{
		base.InitSequence(dataObject);
		_sceneModel = (PromotionSeriesProgressSceneModel)dataObject;
		TopBarController.instance.SectionTitle = string.Empty;
		TopBarController.instance.ShowButtons = false;
		TopBarController.instance.Visible = true;
		TopBarController.instance.LocalUserNotificationsEnabled = false;
		TopBarController.instance.ShowProgressBanner = false;
		_titleText.text = string.Format("ui_postbattle_tiernumber".Localize("tier {0}"), _sceneModel.promotionSeriesDataModel.promotionDivisionId);
		DisplayPreviousWins();
		DisplayPreviousDefeats();
		if ((bool)stateBackground)
		{
			stateBackground.SetSprite((!_sceneModel.didWin) ? PostBattleRewardsSceneController.BACKGROUND_DISABLED_EMPTY_SPRITENAME : PostBattleRewardsSceneController.BACKGROUND_ENABLED_EMPTY_SPRITENAME);
		}
	}

	public override IEnumerator PlayStateSequence(Action<PostBattleRewardsStates, object> callback)
	{
		base.callback = callback;
		if ((bool)_starBust)
		{
			_starBust.SetActive(false);
		}
		if (_sceneModel.didWin)
		{
			EnableNewWin();
		}
		else
		{
			EnableNewDefeat();
		}
		yield return new WaitForSeconds(1f);
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
		tk2dSprite tk2dSprite2 = null;
		for (int i = 0; i < _sceneModel.previousWins; i++)
		{
			tk2dSprite2 = _trophySprites[i];
			tk2dSprite2.gameObject.SetActive(true);
			tk2dSprite2.Alpha = 1f;
		}
		tk2dSprite tk2dSprite3 = null;
		for (int j = 0; j < _sceneModel.previousLosses; j++)
		{
			tk2dSprite3 = _defeatSprites[j];
			tk2dSprite3.gameObject.SetActive(true);
			tk2dSprite3.Alpha = 1f;
		}
		if (_sceneModel.didWin)
		{
			tk2dSprite2 = _trophySprites[_sceneModel.previousWins];
			tk2dSprite2.gameObject.SetActive(true);
			tk2dSprite2.Alpha = 1f;
			_starBust.SetActive(true);
			_starBust.transform.position = tk2dSprite2.transform.position + _starBustOffset;
			if ((bool)tk2dSprite2.animation)
			{
				tk2dSprite2.animation.enabled = true;
			}
		}
		else
		{
			tk2dSprite3 = _defeatSprites[_sceneModel.previousLosses];
			tk2dSprite3.gameObject.SetActive(true);
			tk2dSprite3.Alpha = 1f;
		}
		isFinished = true;
	}

	public override IEnumerator EndSequence(float delay)
	{
		yield return StartCoroutine(base.EndSequence(delay));
		PostBattleRewardsStates nextState = PostBattleRewardsStates.NONE;
		object dataModel = null;
		if (UserProfile.player.notifications.Count == 0)
		{
			nextState = PostBattleRewardsStates.REWARDS_END;
		}
		else
		{
			UserNotification userNotification = UserNotification.ExecuteNotifications();
			dataModel = userNotification.DataModel;
			nextState = userNotification.PostBattleRewardsState;
		}
		if (callback != null)
		{
			callback(nextState, dataModel);
		}
	}

	private void DisplayPreviousWins()
	{
		int num = 0;
		tk2dSprite[] trophySprites = _trophySprites;
		foreach (tk2dSprite tk2dSprite2 in trophySprites)
		{
			tk2dSprite2.Alpha = ((num >= _sceneModel.previousWins) ? 0f : 1f);
			num++;
			if (num > 2)
			{
				break;
			}
		}
	}

	private void DisplayPreviousDefeats()
	{
		int num = 0;
		tk2dSprite[] defeatSprites = _defeatSprites;
		foreach (tk2dSprite tk2dSprite2 in defeatSprites)
		{
			tk2dSprite2.Alpha = ((num >= _sceneModel.previousLosses) ? 0f : 1f);
			num++;
			if (num > 2)
			{
				break;
			}
		}
	}

	private IEnumerator SpriteFadeIn(tk2dSprite sprite, float scale, bool withFireworks = false)
	{
		yield return new WaitForSeconds(0.2f);
		tk2dSprite sprite2 = default(tk2dSprite);
		float scale2 = default(float);
		SimpleTween.Start(0f, 1f, 0.7f, EaseType.EaseOutBack, delegate(float v)
		{
			sprite2.Alpha = v;
			float num = v * scale2;
			sprite2.scale = new Vector3(num, num, num);
			_starBust.transform.localScale = new Vector3(num, num, num);
		});
		if (withFireworks)
		{
			AudioTrigger.GachaSuperRareRevealed.Play();
			AudioTrigger.CrowdExcited.Play();
			_starBust.transform.position = sprite.transform.position + _starBustOffset;
			_starBust.SetActive(withFireworks);
		}
		else
		{
			AudioTrigger.CrowdDisappointed.Play();
		}
		yield return new WaitForSeconds(0.7f);
		if ((bool)sprite.animation)
		{
			sprite.animation.enabled = true;
		}
	}

	private void EnableNewWin()
	{
		tk2dSprite tk2dSprite2 = _trophySprites[_sceneModel.previousWins];
		tk2dSprite tk2dSprite3 = _emptyTrophySprites[_sceneModel.previousWins];
		tk2dSprite2.gameObject.SetActive(true);
		tk2dSprite3.gameObject.SetActive(false);
		StartCoroutine(SpriteFadeIn(tk2dSprite2, 1f, true));
	}

	private void EnableNewDefeat()
	{
		tk2dSprite tk2dSprite2 = _defeatSprites[_sceneModel.previousLosses];
		tk2dSprite2.gameObject.SetActive(true);
		StartCoroutine(SpriteFadeIn(tk2dSprite2, 1f));
	}
}
