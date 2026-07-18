using System;
using System.Collections;
using Holoville.HOTween;
using UnityEngine;

public class AbilityRewardState : SceneState
{
	[SerializeField]
	private tk2dTextMesh abilityName;

	[SerializeField]
	private tk2dTextMesh abilityDescription;

	[SerializeField]
	private tk2dSprite abilityIcon;

	private AbilityDataModel _sceneModel;

	public override void InitSequence(object dataObject)
	{
		base.InitSequence(dataObject);
		_sceneModel = (AbilityDataModel)dataObject;
		if ((bool)abilityIcon)
		{
			abilityIcon.transform.localScale = Vector3.zero;
			abilityIcon.SetSprite(_sceneModel.ButtonIconArtName);
		}
		if ((bool)abilityName)
		{
			abilityName.transform.localScale = Vector3.zero;
			abilityName.text = _sceneModel.Name;
		}
		if ((bool)abilityDescription)
		{
			abilityDescription.transform.localScale = Vector3.zero;
			abilityDescription.text = _sceneModel.ShortDescription;
		}
	}

	public override IEnumerator PlayStateSequence(Action<PostBattleRewardsStates, object> callback)
	{
		base.callback = callback;
		yield return new WaitForSeconds(0.25f);
		if ((bool)abilityName)
		{
			abilityName.transform.TweenLocalScale(1f, 0.5f, EaseType.EaseInExpo);
		}
		yield return new WaitForSeconds(0.1f);
		if ((bool)abilityIcon)
		{
			abilityIcon.transform.TweenLocalScale(1f, 0.8f, EaseType.EaseInOutElastic);
		}
		yield return new WaitForSeconds(0.1f);
		if ((bool)abilityDescription)
		{
			abilityDescription.transform.TweenLocalScale(1f, 0.5f, EaseType.EaseInExpo);
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
		if ((bool)abilityName)
		{
			abilityName.transform.localScale = Vector3.one;
		}
		if ((bool)abilityDescription)
		{
			abilityDescription.transform.localScale = Vector3.one;
		}
		if ((bool)abilityIcon)
		{
			abilityIcon.transform.localScale = Vector3.one;
		}
		isFinished = true;
	}

	public override IEnumerator EndSequence(float delay)
	{
		yield return StartCoroutine(base.EndSequence(delay));
		object dataModel = null;
		PostBattleRewardsStates nextState = PostBattleRewardsStates.NONE;
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
		HOTween.Complete();
		if (callback != null)
		{
			callback(nextState, dataModel);
		}
	}
}
