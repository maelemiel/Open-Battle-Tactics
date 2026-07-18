using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Holoville.HOTween;
using UnityEngine;

public class PartsRewardsState : SceneState
{
	private delegate int CashAmountMethod();

	private const float PART_ELEMENT_HEIGHT = 60f;

	private const float PARTS_FINAL_X_POS = 200f;

	private const float CASH_ELEMENT_HEIGHT = 60f;

	private const string UNIT_DEST_BONUS = "Unit Destruction Bonus";

	private const string UNIT_DEST_BONUS_LOC = "ui_postbattle_unitdestruct_bonus";

	private const string UNIT_SURV_BONUS = "Unit Survival Bonus";

	private const string UNIT_SURV_BONUS_LOC = "ui_postbattle_unitsurvival_bonus";

	private const string MULTI_KILL_BONUS = "Multi Kill Bonus";

	private const string MULTI_KILL_BONUS_LOC = "ui_postbattle_multikill_bonus";

	private const string PERFECT_KILL_BONUS = "Perfect Kill Bonus";

	private const string PERFECT_KILL_BONUS_LOC = "ui_postbattle_perfectkill_bonus";

	private const string OVER_KILL_BONUS = "Rewards Overkill Bonus";

	private const string OVER_KILL_BONUS_LOC = "ui_postbattle_overkill_bonus";

	private const string BEST_ROLL_BONUS = "Best Roll Bonus";

	private const string BEST_ROLL_BONUS_LOC = "ui_postbattle_bestroll_bonus";

	private const string PRIZE_POOL_BONUS = "Prize Pool Bonus";

	private const string PRIZE_POOL_BONUS_LOC = "ui_postbattle_prizepool_bonus";

	private const string PRIZE_POOL_BOOST_LOC = "ui_postbattle_prizepool_boost";

	[SerializeField]
	private GameObject _victoryGameObject;

	[SerializeField]
	private GameObject _defeatGameObject;

	[SerializeField]
	private GameObject cashContainer;

	[SerializeField]
	private GameObject gemContainer;

	[SerializeField]
	private tk2dSprite cashSprite;

	[SerializeField]
	private tk2dTextMesh cashLabel;

	[SerializeField]
	private tk2dTextMesh cashEarnedLabel;

	[SerializeField]
	private tk2dTextMesh partsEarnedLabel;

	[SerializeField]
	private PartsLabelItemView[] _partsViews;

	[SerializeField]
	private tk2dSpineAnimation specialEffectAnimation;

	[SerializeField]
	private tk2dSprite stateBackground;

	[SerializeField]
	private tk2dUIScrollableArea partsScrollArea;

	[SerializeField]
	private GameObject partsContainer;

	[SerializeField]
	private GameObject partsTopMask;

	[SerializeField]
	private GameObject partsBottomMask;

	[SerializeField]
	private tk2dSpineAnimation cashCloudAnimationLoop;

	[SerializeField]
	private tk2dTextMesh[] cashBonusTextFields;

	[SerializeField]
	private CurrencyLabelItemView currencyItemview;

	private BattleRewardsSceneModel _sceneModel;

	private List<CashAmountMethod> cashCallbacks;

	private List<string> cashTitles;

	public override void InitSequence(object dataObject)
	{
		base.InitSequence(dataObject);
		_sceneModel = (BattleRewardsSceneModel)dataObject;
		SetBackgroundState(_sceneModel.isPlayerWinner);
		if ((bool)stateBackground)
		{
			stateBackground.SetSprite((!GetBattleResultForPlayer()) ? PostBattleRewardsSceneController.BACKGROUND_DISABLED_SPRITENAME : PostBattleRewardsSceneController.BACKGROUND_ENABLED_SPRITENAME);
		}
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
		_EnablePartsMasks();
		HOTween.Complete();
		cashCloudAnimationLoop.gameObject.SetActive(false);
		Dictionary<string, int> dictionary = PartsHistogram(_sceneModel.playerStats.partsEarned);
		if (_sceneModel.playerStats.gemsEarned > 0)
		{
		}
		List<KeyValuePair<string, int>> list = dictionary.ToList();
		list.Sort((KeyValuePair<string, int> x, KeyValuePair<string, int> y) => int.Parse(x.Key).CompareTo(int.Parse(y.Key)));
		int num = 0;
		foreach (KeyValuePair<string, int> item in list)
		{
			if (num < _partsViews.Count())
			{
				SetupPartView(_partsViews[num], item, dictionary[item.Key]);
				num++;
			}
		}
		partsScrollArea.ContentLength = (float)(num + 1) * 60f;
		partsScrollArea.Value = 1f;
		partsScrollArea.collider.enabled = true;
		DisplayCashResults();
		int totalCoinsForPlayer = GetTotalCoinsForPlayer();
		SetRewardsCoins(cashLabel, totalCoinsForPlayer, string.Empty);
		cashSprite.Alpha = 1f;
		cashEarnedLabel.Alpha = 1f;
		cashLabel.Alpha = 1f;
		partsEarnedLabel.Alpha = 1f;
		if (_sceneModel.playerStats.gemsEarned > 0)
		{
			cashContainer.transform.position = cashContainer.transform.position + Vector3.up * 60f;
			gemContainer.SetActive(true);
			ItemCollectionDataModel.Item priceData = new ItemCollectionDataModel.Item(UserInventory.ItemType.PremiumCurrency, 0, _sceneModel.playerStats.gemsEarned);
			currencyItemview.SetupPriceItem(priceData);
		}
		isFinished = true;
	}

	private void ClearScene()
	{
		if ((bool)cashLabel)
		{
			cashLabel.text = string.Empty;
			cashLabel.Alpha = 0f;
			cashSprite.Alpha = 0f;
			cashEarnedLabel.Alpha = 0f;
			partsEarnedLabel.Alpha = 0f;
			cashCloudAnimationLoop.gameObject.SetActive(false);
			tk2dTextMesh[] array = cashBonusTextFields;
			foreach (tk2dTextMesh tk2dTextMesh2 in array)
			{
				tk2dTextMesh2.Alpha = 0f;
				tk2dTextMesh2.text = string.Empty;
			}
		}
		cashTitles = new List<string>(8);
		cashCallbacks = new List<CashAmountMethod>(8);
		cashTitles.Add((!GetBattleResultForPlayer()) ? "postbattle_loss_coins".Localize("Completion Bonus") : "postbattle_victory_coins".Localize("Victory Bonus"));
		cashCallbacks.Add(GetBaseRewardForPlayer);
		cashTitles.Add("ui_postbattle_unitdestruct_bonus".Localize("Unit Destruction Bonus"));
		cashCallbacks.Add(CoinsFromUnitsDestroyed);
		cashTitles.Add("ui_postbattle_unitsurvival_bonus".Localize("Unit Survival Bonus"));
		cashCallbacks.Add(CoinsFromUnitsSurvived);
		cashTitles.Add("ui_postbattle_bestroll_bonus".Localize("Best Roll Bonus"));
		cashCallbacks.Add(CoinsFromBestRolls);
		cashTitles.Add("ui_postbattle_multikill_bonus".Localize("Multi Kill Bonus"));
		cashCallbacks.Add(CoinsFromMultiKills);
		cashTitles.Add("ui_postbattle_perfectkill_bonus".Localize("Perfect Kill Bonus"));
		cashCallbacks.Add(CoinsFromPerfectKills);
		cashTitles.Add("ui_postbattle_overkill_bonus".Localize("Rewards Overkill Bonus"));
		cashCallbacks.Add(CoinsFromOverKills);
		cashTitles.Add("ui_postbattle_prizepool_bonus".Localize("Prize Pool Bonus"));
		cashCallbacks.Add(CoinsFromPrizePool);
		cashTitles.Add("ui_postbattle_prizepool_boost".Localize("Prize Pool Bonus"));
		cashCallbacks.Add(CoinsFromBoost);
	}

	private IEnumerator DisplayResultsSequence(Action<PostBattleRewardsStates, object> callback)
	{
		base.callback = callback;
		partsTopMask.SetActive(false);
		partsBottomMask.SetActive(false);
		partsScrollArea.collider.enabled = false;
		yield return new WaitForSeconds(0.5f);
		FadeAlpha(partsEarnedLabel, 1f, 1f);
		_EnableTopMask();
		Dictionary<string, int> partAmountLookup = PartsHistogram(_sceneModel.playerStats.partsEarned);
		List<KeyValuePair<string, int>> modelData = partAmountLookup.ToList();
		modelData.Sort((KeyValuePair<string, int> x, KeyValuePair<string, int> y) => int.Parse(x.Key).CompareTo(int.Parse(y.Key)));
		int i = 0;
		foreach (KeyValuePair<string, int> p in modelData)
		{
			if (i < _partsViews.Count())
			{
				partsScrollArea.ContentLength = (float)(i + 2) * 60f;
				partsScrollArea.Value = 1f;
				yield return StartCoroutine(_SetupPartView(_partsViews[i], p, partAmountLookup[p.Key]));
			}
			i++;
		}
		yield return new WaitForSeconds(1f);
		yield return new WaitForSeconds(1f);
		_EnableBottomMask();
		partsScrollArea.collider.enabled = true;
		cashCloudAnimationLoop.gameObject.SetActive(true);
		yield return StartCoroutine(DisplayCashResultsSequence());
		int totalCoins = GetTotalCoinsForPlayer();
		cashCloudAnimationLoop.gameObject.SetActive(false);
		SetRewardsCoins(cashLabel, totalCoins, string.Empty);
		Vector3 cashLabelInitialScale = cashLabel.transform.localScale;
		Vector3 cashSpriteInitialScale = cashLabel.transform.localScale;
		SimpleTween.Start(0f, 2f, 0.4f, EaseType.EaseInExpo, delegate(float val)
		{
			cashLabel.transform.localScale = cashLabelInitialScale * (3f - val);
			cashSprite.transform.localScale = cashSpriteInitialScale * (3f - val);
			cashLabel.Alpha = val * 0.5f;
			cashSprite.Alpha = val * 0.5f;
		});
		yield return new WaitForSeconds(0.5f);
		AudioTrigger.CoinsEarned.Play();
		yield return new WaitForSeconds(0.3f);
		if (_sceneModel.playerStats.gemsEarned > 0)
		{
			cashContainer.transform.position = cashContainer.transform.position + Vector3.up * 60f;
			gemContainer.SetActive(true);
			ItemCollectionDataModel.Item item = new ItemCollectionDataModel.Item(UserInventory.ItemType.PremiumCurrency, 0, _sceneModel.playerStats.gemsEarned);
			currencyItemview.SetupPriceItem(item);
			Vector3 initialScale = currencyItemview.transform.localScale;
			SimpleTween.Start(0f, 2f, 0.4f, EaseType.EaseInExpo, delegate(float val)
			{
				currencyItemview.transform.localScale = initialScale * (3f - val);
			});
			yield return new WaitForSeconds(0.75f);
		}
		isFinished = true;
	}

	private void _EnablePartsMasks()
	{
		_EnableTopMask();
		_EnableBottomMask();
	}

	private void _EnableTopMask()
	{
		if ((bool)partsTopMask)
		{
			partsTopMask.SetActive(true);
		}
	}

	private void _EnableBottomMask()
	{
		if ((bool)partsBottomMask)
		{
			partsBottomMask.SetActive(true);
		}
	}

	private void SetupPartView(PartsLabelItemView partView, KeyValuePair<string, int> partInfo, int amount)
	{
		ItemCollectionDataModel.Item item = new ItemCollectionDataModel.Item(UserInventory.ItemType.Parts, int.Parse(partInfo.Key), amount);
		partView.SetupPriceItem(item, true);
		partView.Label = item.Part.Name + "  " + partInfo.Value;
		partView.transform.localPosition = new Vector3(-5f - (float)partView.Label.Length * 5f, partView.transform.localPosition.y, partView.transform.localPosition.z);
	}

	private IEnumerator _SetupPartView(PartsLabelItemView partView, KeyValuePair<string, int> partInfo, int amount)
	{
		SetupPartView(partView, partInfo, amount);
		Vector3 initialScale = partView.transform.localScale;
		PartsLabelItemView partView2 = default(PartsLabelItemView);
		SimpleTween.Start(0f, 2f, 0.4f, EaseType.EaseInExpo, delegate(float val)
		{
			partView2.transform.localScale = initialScale * (3f - val);
		});
		yield return new WaitForSeconds(0.4f);
		specialEffectAnimation.gameObject.SetActive(true);
		specialEffectAnimation.gameObject.transform.position = partView.prefabProxy.gameObject.transform.position;
		specialEffectAnimation.Reset();
		AudioTrigger.LowDamageResult.Play();
		yield return new WaitForSeconds(0.25f);
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

	public bool GetBattleResultForPlayer()
	{
		return _sceneModel.isPlayerWinner;
	}

	private Dictionary<string, int> PartsHistogram(List<IPartMetadata> parts)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		foreach (IPartMetadata item in _sceneModel.playerStats.partsEarned)
		{
			string iD = item.ID;
			if (!dictionary.ContainsKey(iD))
			{
				dictionary[iD] = 0;
			}
			Dictionary<string, int> dictionary3;
			Dictionary<string, int> dictionary2 = (dictionary3 = dictionary);
			string key2;
			string key = (key2 = iD);
			int num = dictionary3[key2];
			dictionary2[key] = num + 1;
		}
		return dictionary;
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

	private int GetTotalCoinsForPlayer()
	{
		return GetBaseRewardForPlayer() + CoinsFromUnitsDestroyed() + CoinsFromUnitsSurvived() + CoinsFromBestRolls() + CoinsFromMultiKills() + CoinsFromPerfectKills() + CoinsFromOverKills() + CoinsFromPrizePool() + CoinsFromBoost();
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

	private int CoinsFromBestRolls()
	{
		return _sceneModel.playerStats.coinsFromBestRolls;
	}

	private int CoinsFromMultiKills()
	{
		return _sceneModel.playerStats.coinsFromMultiKill;
	}

	private int CoinsFromPerfectKills()
	{
		return _sceneModel.playerStats.coinsFromPerfectKills;
	}

	private int CoinsFromOverKills()
	{
		return _sceneModel.playerStats.coinsFromOverKills;
	}

	private int CoinsFromPrizePool()
	{
		return _sceneModel.playerStats.coinsFromPrizePool;
	}

	private int CoinsFromBoost()
	{
		return _sceneModel.playerStats.coinsFromBoost;
	}

	private IEnumerator DisplayRewardCoins(tk2dTextMesh textField, int amount, string reason)
	{
		_DisplayRewardCoins(textField, amount, reason);
		Vector3 initialScale = textField.transform.localScale;
		textField.Alpha = 1f;
		tk2dTextMesh textField2 = default(tk2dTextMesh);
		SimpleTween.Start(0f, 1f, 0.4f, EaseType.EaseInExpo, delegate(float val)
		{
			textField2.transform.localScale = initialScale * (2f - val);
		});
		yield return new WaitForSeconds(0.35f);
		SimpleTween.Start(0f, 1f, 0.4f, EaseType.EaseInExpo, delegate(float val)
		{
			textField2.Alpha = 1f - val;
		});
		yield return new WaitForSeconds(0.15f);
	}

	private void _DisplayRewardCoins(tk2dTextMesh textField, int amount, string reason)
	{
		textField.text = reason + " +" + amount;
		textField.Alpha = 0f;
	}

	private IEnumerator DisplayCashResultsSequence()
	{
		yield return new WaitForSeconds(0.5f);
		int counter = 0;
		for (int i = 0; i < cashCallbacks.Count; i++)
		{
			int cashFromCallback = cashCallbacks[i]();
			if (cashFromCallback != 0)
			{
				AudioTrigger.CoinsEarned.Play();
				yield return StartCoroutine(DisplayRewardCoins(cashBonusTextFields[counter], cashFromCallback, cashTitles[i]));
				counter++;
			}
		}
		yield return new WaitForSeconds(0.75f);
		isFinished = true;
	}

	private void DisplayCashResults()
	{
		int num = 0;
		for (int i = 0; i < cashCallbacks.Count; i++)
		{
			int num2 = cashCallbacks[i]();
			if (num2 != 0)
			{
				_DisplayRewardCoins(cashBonusTextFields[num], num2, cashTitles[i]);
				num++;
			}
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
