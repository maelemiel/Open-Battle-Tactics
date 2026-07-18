using System.Collections;
using System.Collections.Generic;
using Spine;
using UnityEngine;

public class RaidBossManager : MonoBehaviour
{
	private BattleController battleController;

	private int totalDamageDealt;

	private int previousDamageThreshold;

	private List<EventRaidbossDamageDropRateDataModel> achievedDrops = new List<EventRaidbossDamageDropRateDataModel>();

	[SerializeField]
	private UnitProxy bossProxy;

	public DamageScoreboard damageScoreboard;

	public DamageProgressViewController damageProgressViewController;

	private List<EventRaidbossDamageDropRateDataModel> orderedDrops = new List<EventRaidbossDamageDropRateDataModel>();

	private int completedTiers = 1;

	private int rewardTierCount;

	private EventRaidbossDamageDropRateDataModel maxDropRate;

	private IUnitMetadata bossUnitMetadata;

	public void Init(BattleController battleController)
	{
		this.battleController = battleController;
		if (battleController.SceneModel.activeEvent != null && battleController.SceneModel.activeEvent.EventType == EventDataModel.EventTypes.RAIDBOSS_EVENT && battleController.enemyTeam.type == TeamType.RaidBoss)
		{
			Startup();
		}
	}

	private void Startup()
	{
		totalDamageDealt = 0;
		battleController.BattleHooks = new RaidBossBattleHooks();
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.BOSS_MOMENT, 1);
		UnitState[] units = battleController.enemyTeam.units;
		foreach (UnitState unitState in units)
		{
			if (unitState.metadata.UnitType == UnitType.RAID_BOSS)
			{
				unitState.unitView.OnReceiveDamageEvent += OnBossReceiveDamage;
				bossUnitMetadata = unitState.metadata;
			}
		}
		if ((bool)damageProgressViewController)
		{
			damageProgressViewController.Init(new List<EventRaidbossDamageDropRateDataModel>(battleController.MatchData.opponentTeam.rewardDropRate), bossUnitMetadata.StartingHealth);
		}
		orderedDrops = new List<EventRaidbossDamageDropRateDataModel>(battleController.MatchData.opponentTeam.rewardDropRate);
		orderedDrops.Sort((EventRaidbossDamageDropRateDataModel x, EventRaidbossDamageDropRateDataModel y) => x.threshold.CompareTo(y.threshold));
		maxDropRate = ((orderedDrops.Count <= 0) ? null : orderedDrops[orderedDrops.Count - 1]);
		CheckNextRewardDamage();
	}

	private void OnBossReceiveDamage(UnitView unit, int damage)
	{
		totalDamageDealt += damage;
		CheckCompletedProgress();
		if ((bool)damageScoreboard)
		{
			damageScoreboard.SetValue(totalDamageDealt);
		}
		if ((bool)damageProgressViewController)
		{
			damageProgressViewController.SetProgress(totalDamageDealt);
		}
		CheckNextRewardDamage();
		CheckForPrizeDrop();
	}

	private void CheckCompletedProgress()
	{
		if (maxDropRate != null && totalDamageDealt > maxDropRate.threshold * completedTiers)
		{
			completedTiers++;
		}
	}

	private void CheckNextRewardDamage()
	{
		int nextRewardValue = 0;
		if (maxDropRate == null)
		{
			if (bossUnitMetadata != null)
			{
				nextRewardValue = bossUnitMetadata.StartingHealth;
			}
		}
		else
		{
			int num = maxDropRate.threshold * (completedTiers - 1);
			int num2 = totalDamageDealt % maxDropRate.threshold;
			foreach (EventRaidbossDamageDropRateDataModel orderedDrop in orderedDrops)
			{
				if (num2 < orderedDrop.threshold)
				{
					nextRewardValue = orderedDrop.threshold + num;
					break;
				}
			}
		}
		if ((bool)damageScoreboard)
		{
			damageScoreboard.SetNextRewardValue(nextRewardValue);
		}
	}

	private int GetLastCompletedProgress()
	{
		return Mathf.Max(completedTiers - 1, 1);
	}

	private void CheckForPrizeDrop()
	{
		if (maxDropRate == null)
		{
			return;
		}
		List<UnitView> allEnemyUnitViews = battleController.GetAllEnemyUnitViews();
		int num = totalDamageDealt - maxDropRate.threshold * rewardTierCount;
		UnitView unitView = null;
		for (int i = 0; i < allEnemyUnitViews.Count; i++)
		{
			unitView = allEnemyUnitViews[i];
			if (unitView.state.metadata.UnitType != UnitType.RAID_BOSS)
			{
				continue;
			}
			List<ItemCollectionDataModel> list = new List<ItemCollectionDataModel>();
			foreach (EventRaidbossDamageDropRateDataModel orderedDrop in orderedDrops)
			{
				if (num >= orderedDrop.threshold && !achievedDrops.Contains(orderedDrop))
				{
					list.Add(ItemGiftDataModel.GetGiftPackage(orderedDrop.giftid));
					achievedDrops.Add(orderedDrop);
				}
			}
			if (list.Count > 0)
			{
				unitView.ShowGiftEffect(list);
			}
			else if (totalDamageDealt > maxDropRate.threshold * GetLastCompletedProgress())
			{
				rewardTierCount++;
				achievedDrops.Clear();
			}
			break;
		}
	}

	public IEnumerator AnimateRaidBossMoment()
	{
		bool waitForAnim = true;
		battleController.hud.battleText.ShowMessage("battle_event_raid_boss_warning".Localize("WARNING RAID BOSS!"), Color.yellow, InBattleMessageType.BOSS_MOMENT);
		UnitState bossUnit = battleController.enemyTeam.units[0];
		for (int i = 0; i < battleController.enemyTeam.units.Length; i++)
		{
			if (battleController.enemyTeam.units[i].metadata.UnitType == UnitType.RAID_BOSS)
			{
				bossUnit = battleController.enemyTeam.units[i];
				break;
			}
		}
		bossProxy.ChangeAsset("Prefab.prefab", bossUnit.metadata.AssetBundleID);
		AudioTrigger.BossEntrance.Play();
		yield return new WaitForSeconds(2f);
		AudioTrigger.BossMoment.Play();
		EffectInstance effect = null;
		yield return StartCoroutine(GlobalEffectsManager.CreateCoroutine(EffectType.BOSS_MOMENT, base.transform.position, base.gameObject, delegate(EffectInstance x)
		{
			effect = x;
		}));
		effect.AutoDestroy();
		Bone bossBone = effect.SpineAnimation.Skeleton.skeleton.FindBone("Big Boss");
		bossProxy.transform.parent = effect.transform;
		if ((bool)effect.SpineAnimation)
		{
			effect.SpineAnimation.AnimationComplete += delegate
			{
				waitForAnim = false;
			};
		}
		battleController.hud.battleText.ShowMessage(bossUnit.UserUnitMetadata.Name, Color.white, InBattleMessageType.NONE, 250f);
		while (waitForAnim)
		{
			bossProxy.transform.localPosition = new Vector3(bossBone.X, bossBone.Y, 0f);
			yield return new WaitForEndOfFrame();
		}
		bossProxy.gameObject.SetActive(false);
		AudioTrigger.BattleBackground_Music.PlayMusic();
	}
}
