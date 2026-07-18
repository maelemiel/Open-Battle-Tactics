using System.Collections;
using UnityEngine;

public class CoinIncreaseAnimationHandler : AbilityAnimationHandler
{
	private UnitState deadUnit;

	private int cashEffect;

	public override IEnumerator OntoFieldAnimation()
	{
		abilityState.target.unitView.ShowPassiveEffect("Reward Passive", abilityState.Name);
		yield return new WaitForSeconds(0.1f);
	}

	protected override void LoadDependencies()
	{
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.PASSIVE_EFFECT, 1);
	}

	public override IEnumerator UnitDiedAnimation(UnitState unit)
	{
		if (abilityState.target.team == battleController.playerTeam)
		{
			deadUnit = unit;
			cashEffect = unit.extraDestroyCoins;
			unit.unitView.DeathAnimationList.Add(ShowCashReward());
		}
		yield break;
	}

	private IEnumerator ShowCashReward()
	{
		abilityState.target.unitView.ShowPassiveEffect("Reward Passive", abilityState.Name);
		deadUnit.unitView.ShowCashStackEffect(cashEffect, UserInventory.ItemType.Coins);
		yield return new WaitForSeconds(0.25f);
	}
}
