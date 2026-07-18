using System;
using System.Collections;
using Holoville.HOTween;
using UnityEngine;

public class CashRewardsState : SceneState
{
	private const string TITLE_VICTORY = "victory!";

	private const string TITLE_DEFEAT = "defeat!";

	private const string TOTAL_WINS = " total wins";

	private const string GAME_WIN_STREAK = " game win streak!";

	private const string WIN_BONUS = "Win Bonus";

	private const string LOSE_BONUS = "Lose Bonus";

	private const string CASH_EARNED = "Cash Earned";

	private const string UNIT_DEST_BONUS = "Unit Destruction Bonus";

	private const string UNIT_SURV_BONUS = "Unit Survival Bonus";

	private const string MULTI_KILL_BONUS = "Multi Kill Bonus";

	private const string PERFECT_KILL_BONUS = "Perfect Kill Bonus";

	private const string OVER_KILL_BONUS = "Rewards Overkill Bonus";

	private const string BEST_ROLL_BONUS = "Best Roll Bonus";

	private int bonusTextIndex;

	private int _minStreakToDisplay = 3;

	[SerializeField]
	private tk2dTextMesh[] bonusTextFields;

	[SerializeField]
	private tk2dTextMesh _victory;

	[SerializeField]
	private tk2dTextMesh _totalWins;

	[SerializeField]
	private tk2dTextMesh _winStreak;

	[SerializeField]
	private tk2dSprite _rewardsSprite;

	[SerializeField]
	private tk2dTextMesh _rewards;

	[SerializeField]
	private tk2dSprite stateBackground;

	private BattleRewardsSceneModel _sceneModel;

	public override void InitSequence(object dataObject)
	{
		base.InitSequence(dataObject);
		ClearScene();
		_sceneModel = (BattleRewardsSceneModel)dataObject;
		if ((bool)_victory)
		{
			_victory.text = ((!GetBattleResultForPlayer()) ? "defeat!" : "victory!");
		}
		if ((bool)_totalWins)
		{
			_totalWins.text = GetTotalNumberOfWinsForPlayer() + " total wins";
		}
		if ((bool)stateBackground)
		{
			stateBackground.SetSprite((!GetBattleResultForPlayer()) ? PostBattleRewardsSceneController.BACKGROUND_DISABLED_SPRITENAME : PostBattleRewardsSceneController.BACKGROUND_ENABLED_SPRITENAME);
		}
	}

	public override IEnumerator PlayStateSequence(Action<PostBattleRewardsStates, object> callback)
	{
		base.callback = callback;
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
		bool battleResultForPlayer = GetBattleResultForPlayer();
		if ((bool)_victory)
		{
			_victory.text = ((!GetBattleResultForPlayer()) ? "defeat!" : "victory!");
		}
		if ((bool)_totalWins)
		{
			_totalWins.text = GetTotalNumberOfWinsForPlayer() + " total wins";
		}
		int totalCoinsForPlayer = GetTotalCoinsForPlayer();
		SetRewardsCoins(_rewards, totalCoinsForPlayer, "Cash Earned");
		_rewardsSprite.Alpha = 1f;
		if (GetWinStreakForPlayer() >= _minStreakToDisplay)
		{
			_winStreak.gameObject.SetActive(true);
			_winStreak.text = GetWinStreakForPlayer() + " game win streak!";
		}
		int num = 0;
		SetRewardsCoins(bonusTextFields[num], GetBaseRewardForPlayer(), (!battleResultForPlayer) ? "Lose Bonus" : "Win Bonus");
		if (CoinsFromUnitsDestroyed() != 0)
		{
			num++;
			SetRewardsCoins(bonusTextFields[num], CoinsFromUnitsDestroyed(), "Unit Destruction Bonus");
		}
		if (CoinsFromUnitsSurvived() != 0)
		{
			num++;
			SetRewardsCoins(bonusTextFields[num], CoinsFromUnitsSurvived(), "Unit Survival Bonus");
		}
		if (_sceneModel.playerStats.coinsFromBestRolls != 0)
		{
			num++;
			SetRewardsCoins(bonusTextFields[num], _sceneModel.playerStats.coinsFromBestRolls, "Best Roll Bonus");
		}
		if (_sceneModel.playerStats.coinsFromMultiKill != 0)
		{
			num++;
			SetRewardsCoins(bonusTextFields[num], _sceneModel.playerStats.coinsFromMultiKill, "Multi Kill Bonus");
		}
		if (_sceneModel.playerStats.coinsFromPerfectKills != 0)
		{
			num++;
			SetRewardsCoins(bonusTextFields[num], _sceneModel.playerStats.coinsFromPerfectKills, "Perfect Kill Bonus");
		}
		if (_sceneModel.playerStats.coinsFromOverKills != 0)
		{
			num++;
			SetRewardsCoins(bonusTextFields[num], _sceneModel.playerStats.coinsFromOverKills, "Rewards Overkill Bonus");
		}
		isFinished = true;
	}

	public bool GetBattleResultForPlayer()
	{
		return _sceneModel.isPlayerWinner;
	}

	private int GetTotalNumberOfWinsForPlayer()
	{
		return UserProfile.player.wins;
	}

	private int GetWinStreakForPlayer()
	{
		return UserProfile.player.winStreak;
	}

	private int GetBaseRewardForPlayer()
	{
		return _sceneModel.playerStats.baseCoins;
	}

	private int CoinsFromUnitsDestroyed()
	{
		return _sceneModel.playerStats.coinsFromUnitsDestroyed;
	}

	private int CoinsFromUnitsSurvived()
	{
		return _sceneModel.playerStats.coinsFromUnitsSurvived;
	}

	private int GetTotalCoinsForPlayer()
	{
		return GetBaseRewardForPlayer() + CoinsFromUnitsDestroyed() + CoinsFromUnitsSurvived() + _sceneModel.playerStats.coinsFromBestRolls + _sceneModel.playerStats.coinsFromMultiKill + _sceneModel.playerStats.coinsFromPerfectKills + _sceneModel.playerStats.coinsFromOverKills + _sceneModel.playerStats.coinsFromPrizePool;
	}

	private void ClearScene()
	{
		if ((bool)_winStreak)
		{
			_winStreak.gameObject.SetActive(false);
		}
		if ((bool)_rewards)
		{
			_rewards.text = string.Empty;
			_rewards.Alpha = 0f;
			_rewardsSprite.Alpha = 0f;
		}
		tk2dTextMesh[] array = bonusTextFields;
		foreach (tk2dTextMesh tk2dTextMesh2 in array)
		{
			tk2dTextMesh2.Alpha = 0f;
			tk2dTextMesh2.text = string.Empty;
		}
	}

	private IEnumerator DisplayResultsSequence(Action<PostBattleRewardsStates, object> callback)
	{
		base.callback = callback;
		yield return new WaitForSeconds(0.5f);
		FadeAlpha(_rewards, 1f, 1f);
		FadeAlpha(_rewardsSprite, 1f, 1f);
		StartCoroutine(AddRewardCoins(_rewards, GetTotalCoinsForPlayer(), 0.3f * (float)GetRewardCountForPlayer(), 0f, "Cash Earned", true, EaseType.Linear));
		int counter = 0;
		if (CoinsFromUnitsDestroyed() != 0)
		{
			counter++;
			FadeAlpha(bonusTextFields[counter], 1f, 0.5f);
			yield return StartCoroutine(AddRewardCoins(bonusTextFields[counter], CoinsFromUnitsDestroyed(), 0.5f, -30f, "Unit Destruction Bonus"));
		}
		if (CoinsFromUnitsSurvived() != 0)
		{
			counter++;
			FadeAlpha(bonusTextFields[counter], 1f, 0.5f);
			yield return StartCoroutine(AddRewardCoins(bonusTextFields[counter], CoinsFromUnitsSurvived(), 0.5f, -30f, "Unit Survival Bonus"));
		}
		if (_sceneModel.playerStats.coinsFromBestRolls != 0)
		{
			counter++;
			FadeAlpha(bonusTextFields[counter], 1f, 0.5f);
			yield return StartCoroutine(AddRewardCoins(bonusTextFields[counter], _sceneModel.playerStats.coinsFromBestRolls, 0.5f, -30f, "Best Roll Bonus"));
		}
		if (_sceneModel.playerStats.coinsFromMultiKill != 0)
		{
			counter++;
			FadeAlpha(bonusTextFields[counter], 1f, 0.5f);
			yield return StartCoroutine(AddRewardCoins(bonusTextFields[counter], _sceneModel.playerStats.coinsFromMultiKill, 0.5f, -30f, "Multi Kill Bonus"));
		}
		if (_sceneModel.playerStats.coinsFromPerfectKills != 0)
		{
			counter++;
			FadeAlpha(bonusTextFields[counter], 1f, 0.5f);
			yield return StartCoroutine(AddRewardCoins(bonusTextFields[counter], _sceneModel.playerStats.coinsFromPerfectKills, 0.5f, -30f, "Perfect Kill Bonus"));
		}
		if (_sceneModel.playerStats.coinsFromOverKills != 0)
		{
			counter++;
			FadeAlpha(bonusTextFields[counter], 1f, 0.5f);
			yield return StartCoroutine(AddRewardCoins(bonusTextFields[counter], _sceneModel.playerStats.coinsFromOverKills, 0.5f, -30f, "Rewards Overkill Bonus"));
		}
		yield return new WaitForSeconds(1.5f);
		isFinished = true;
	}

	public override IEnumerator EndSequence(float delay)
	{
		yield return StartCoroutine(base.EndSequence(delay));
		PostBattleRewardsStates nextState = PostBattleRewardsStates.NONE;
		object dataModel = null;
		if (UserProfile.player.notifications.Count == 0)
		{
			nextState = PostBattleRewardsStates.REWARDS_PARTS;
		}
		else
		{
			UserNotification userNotification = UserNotification.ExecuteNotifications();
			if (userNotification is UserNotification.PartsReceived)
			{
				UserNotification.PartsReceived partsReceivedNotification = userNotification as UserNotification.PartsReceived;
				dataModel = partsReceivedNotification.rewards;
				nextState = PostBattleRewardsStates.REWARDS_PARTS;
			}
			else if (userNotification is UserNotification.DivisionProgress)
			{
				UserNotification.DivisionProgress divisionProgressNotification = userNotification as UserNotification.DivisionProgress;
				dataModel = divisionProgressNotification.sceneModel;
				nextState = PostBattleRewardsStates.REWARDS_DIVISION_PROGRESS;
			}
			else if (userNotification is UserNotification.PromoSeriesProgress)
			{
				UserNotification.PromoSeriesProgress promoProgressNotification = userNotification as UserNotification.PromoSeriesProgress;
				dataModel = promoProgressNotification.sceneModel;
				nextState = PostBattleRewardsStates.REWARDS_PROMOTION_PROGRESS;
			}
		}
		if (callback != null)
		{
			callback(nextState, dataModel);
		}
	}

	private IEnumerator AddRewardCoins(tk2dTextMesh textField, int amount, float time, float initialOffset, string reason, bool animateAmount = false, EaseType easeType = EaseType.EaseOutExpo)
	{
		if (bonusTextIndex < bonusTextFields.Length)
		{
			textField.text = "+" + amount + " " + reason;
			Vector3 originalPos = textField.transform.localPosition;
			Vector3 newPos = new Vector3(originalPos.x + initialOffset, originalPos.y, originalPos.z);
			bool animateAmount2 = default(bool);
			int amount2 = default(int);
			string reason2 = default(string);
			float initialOffset2 = default(float);
			SimpleTween.Start(0f, 1f, time, easeType, delegate(float val)
			{
				tk2dTextMesh bonusField;
				if (animateAmount2)
				{
					bonusField.text = "+" + (int)((float)amount2 * val) + " " + reason2;
				}
				if (initialOffset2 != 0f)
				{
					bonusField.transform.localPosition = Vector3.Lerp(newPos, originalPos, val);
				}
			});
		}
		else
		{
			Log.Warning("Not enough textfields to show reward: " + reason);
		}
		yield return new WaitForSeconds(0.25f);
	}

	private void FadeAlpha(tk2dSprite sprite, float finalAlphaValue, float time, EaseType easeType = EaseType.EaseOutExpo)
	{
		SimpleTween.Start(0f, finalAlphaValue, time, easeType, delegate(float val)
		{
			sprite.Alpha = val;
		});
	}

	private void FadeAlpha(tk2dTextMesh textMesh, float finalAlphaValue, float time, EaseType easeType = EaseType.EaseOutExpo)
	{
		SimpleTween.Start(0f, finalAlphaValue, time, easeType, delegate(float val)
		{
			textMesh.Alpha = val;
		});
	}

	private void SetRewardsCoins(tk2dTextMesh textMesh, int amount, string reason)
	{
		textMesh.Alpha = 1f;
		textMesh.text = "+" + amount + " " + reason;
	}

	private int GetRewardCountForPlayer()
	{
		int num = 0;
		if (CoinsFromUnitsDestroyed() != 0)
		{
			num++;
		}
		if (CoinsFromUnitsSurvived() != 0)
		{
			num++;
		}
		if (_sceneModel.playerStats.coinsFromBestRolls != 0)
		{
			num++;
		}
		if (_sceneModel.playerStats.coinsFromMultiKill != 0)
		{
			num++;
		}
		if (_sceneModel.playerStats.coinsFromPerfectKills != 0)
		{
			num++;
		}
		if (_sceneModel.playerStats.coinsFromOverKills != 0)
		{
			num++;
		}
		return num;
	}
}
