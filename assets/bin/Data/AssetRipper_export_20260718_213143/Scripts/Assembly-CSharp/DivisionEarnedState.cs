using System;
using System.Collections;
using Holoville.HOTween;
using UnityEngine;

public class DivisionEarnedState : SceneState
{
	public TweenPositionEffect _title;

	public TweenPositionEffect _description;

	public TweenPositionEffect _details;

	public GameObject trophies;

	private Tweener trophiesTween;

	private ProgressionPromotionSeriesDataModel _dataModel;

	private void Awake()
	{
		if ((bool)_title)
		{
			_title.gameObject.SetActive(false);
		}
		if ((bool)_description)
		{
			_description.gameObject.SetActive(false);
		}
		if ((bool)_details)
		{
			_details.gameObject.SetActive(false);
		}
	}

	public override void InitSequence(object dataObject)
	{
		base.InitSequence(dataObject);
		_dataModel = dataObject as ProgressionPromotionSeriesDataModel;
		trophies.transform.localScale = Vector3.zero;
		TopBarController.instance.SectionTitle = string.Empty;
		TopBarController.instance.ShowButtons = false;
		TopBarController.instance.Visible = true;
		TopBarController.instance.LocalUserNotificationsEnabled = false;
		TopBarController.instance.ShowProgressBanner = false;
	}

	public override IEnumerator PlayStateSequence(Action<PostBattleRewardsStates, object> callback)
	{
		base.callback = callback;
		yield return StartCoroutine(AnnouncerController.DialogTrigger("Promo Series"));
		_title.gameObject.SetActive(true);
		yield return new WaitForSeconds(0.2f);
		_description.gameObject.SetActive(true);
		yield return new WaitForSeconds(0.2f);
		trophiesTween = trophies.transform.TweenLocalScale(1f, 1f, EaseType.EaseOutBack);
		yield return new WaitForSeconds(0.2f);
		_details.gameObject.SetActive(true);
		yield return new WaitForSeconds(1.5f);
		isFinished = true;
	}

	public override void SkipToEnd()
	{
		if (isFinished)
		{
			StartCoroutine(EndSequence(0f));
			return;
		}
		if (_title != null)
		{
			_title.gameObject.SetActive(true);
			_title.CompleteTween();
		}
		if (_description != null)
		{
			_description.gameObject.SetActive(true);
			_description.CompleteTween();
		}
		if (_details != null)
		{
			_details.gameObject.SetActive(true);
			_details.CompleteTween();
		}
		if (trophiesTween != null)
		{
			trophiesTween.Kill();
		}
		trophies.transform.localScale = Vector3.one;
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
}
