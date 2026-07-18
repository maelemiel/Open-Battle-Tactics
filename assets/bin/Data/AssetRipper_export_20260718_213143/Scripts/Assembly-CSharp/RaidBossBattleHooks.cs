using System.Collections;
using Holoville.HOTween;
using UnityEngine;

public class RaidBossBattleHooks : BattleHooks
{
	private bool waitingOnBossDeath;

	public override IEnumerator PreIntroEnemyRollIn()
	{
		while (battleController.eventLogoController.IsAnimating)
		{
			yield return new WaitForEndOfFrame();
		}
		yield return battleController.StartCoroutine(battleController.raidBossManager.AnimateRaidBossMoment());
	}

	public override IEnumerator OnEnterDecisionPhase()
	{
		if ((bool)battleController.raidBossManager)
		{
			battleController.raidBossManager.damageScoreboard.Show();
		}
		yield break;
	}

	public override IEnumerator OutroAnimation()
	{
		if ((bool)battleController.raidBossManager)
		{
			battleController.raidBossManager.damageScoreboard.Hide();
		}
		float timer = 0f;
		while (waitingOnBossDeath && timer <= 2f)
		{
			yield return new WaitForEndOfFrame();
			timer += Time.deltaTime;
		}
	}

	public override bool CanRevive()
	{
		if (battleController.battleState.teamOne.IsDead && !battleController.battleState.teamOne.forfeited && !battleController.battleState.teamTwo.IsDead)
		{
			EventDataModel activeEvent = UserProfile.player.GetActiveEvent();
			if (activeEvent != null && activeEvent.EventType == EventDataModel.EventTypes.RAIDBOSS_EVENT && battleController.battleState.teamTwo.type == TeamType.RaidBoss)
			{
				UserPriceDataModel priceForID = ItemPriceDataModel.GetPriceForID(Constants.RaidBossRevivePriceA);
				UserPriceDataModel priceForID2 = ItemPriceDataModel.GetPriceForID(Constants.RaidBossRevivePriceB);
				return UserProfile.player.CanAfford(priceForID) || UserProfile.player.CanAfford(priceForID2);
			}
		}
		return false;
	}

	public override IEnumerator Revive()
	{
		bool waitingForResponse = true;
		bool revive = false;
		PopupManager.ShowPopup(PopupDataModel.Revive(delegate
		{
			waitingForResponse = false;
			revive = true;
		}, delegate
		{
			waitingForResponse = false;
		}));
		while (waitingForResponse)
		{
			yield return new WaitForEndOfFrame();
		}
		if (revive)
		{
			UserPriceDataModel priceA = ItemPriceDataModel.GetPriceForID(Constants.RaidBossRevivePriceA);
			UserPriceDataModel priceB = ItemPriceDataModel.GetPriceForID(Constants.RaidBossRevivePriceB);
			ReviveAction action = new ReviveAction();
			if (UserProfile.player.CanAfford(priceA))
			{
				action.usedA = true;
				UserProfile.player.RemoveItems(priceA);
			}
			else
			{
				action.usedA = false;
				UserProfile.player.RemoveItems(priceB);
			}
			battleController.StartCoroutine(battleController.resolutionPhase.TeamRevive(action));
		}
		else
		{
			battleController.phaseManager.SwitchPhase(Phase.OUTRO);
		}
	}

	public override bool OverrideUnitDeath(UnitState unit)
	{
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.BOSS_EXPLOSION, 1);
		return unit.metadata.UnitType == UnitType.RAID_BOSS;
	}

	public override IEnumerator OnUnitDeath(UnitView unitView)
	{
		waitingOnBossDeath = true;
		if (unitView.isEnemy)
		{
			unitView.HealthUI.gameObject.SetActive(false);
			if (unitView.state.droppedParts != null)
			{
				unitView.DropPartsAnimation();
				battleController.animationHandler.AddImmediateSequence(unitView.WaitSequence(0.4f));
			}
			if (battleController.MatchHandler.IsEventPointsMatch && UserProfile.player.divisionInt >= Constants.MinTierEventContent)
			{
				unitView.ShowCashStackEffect(unitView.state.metadata.DestroyEventPoints, UserInventory.ItemType.EventPoint);
			}
			else if (battleController.MatchHandler.IsRaidBossEventActive && unitView.state.metadata.UnitType == UnitType.RAID_BOSS)
			{
				unitView.ShowCashStackEffect(unitView.state.metadata.DestroyEventPoints, UserInventory.ItemType.RaidBossEventPoint);
			}
			if (unitView.state.gemsDropped > 0)
			{
				unitView.ShowGemEffect(unitView.state.gemsDropped);
			}
			unitView.ShowCashEffect(BattleLogic.GetUnitDestroyCoinReward(unitView.state, unitView.state.team));
			AudioTrigger.CrowdCheering.Play();
		}
		battleController.animationHandler.AddImmediateSequence(unitView.WaitSequence(7f));
		yield return new WaitForSeconds(3f);
		EffectInstance effect = null;
		yield return unitView.StartCoroutine(GlobalEffectsManager.CreateCoroutine(EffectType.BOSS_EXPLOSION, unitView.transform.position, null, delegate(EffectInstance x)
		{
			effect = x;
		}));
		AudioTrigger.BigBossExplosion.Play();
		effect.SetLayer(LayerMask.NameToLayer("BattleFieldEnemy"));
		effect.AutoDestroy();
		unitView.ShakeMyField(10f, 0.2f);
		yield return new WaitForSeconds(1.5f);
		AudioTrigger.TankKilled.Play();
		unitView.ShakeMyField(30f, 0.2f);
		Handheld.Vibrate();
		battleController.RemoveUnit(unitView);
		unitView.StartCoroutine(unitView.DeactivateIn(0.25f));
		EffectInstance wreckEffect = GlobalEffectsManager.Create(EffectType.WRECK_BOSS, unitView.transform.position, battleController.enemyField.gameObject);
		wreckEffect.transform.TweenLocalXPosition(wreckEffect.transform.localPosition.x + 1000f, 2f, EaseType.Linear);
		yield return new WaitForSeconds(2f);
		waitingOnBossDeath = false;
	}
}
