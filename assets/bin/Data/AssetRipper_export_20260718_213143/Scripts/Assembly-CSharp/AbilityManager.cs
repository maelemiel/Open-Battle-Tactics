public class AbilityManager
{
	private BattleController battleController;

	public AbilityManager(BattleController battleController)
	{
		this.battleController = battleController;
	}

	public void PlayerTryActivateAbility(AbilityState ability)
	{
		IAbilityMetadata metadata = ability.metadata;
		if (metadata.TargetSelectType != TargetType.None)
		{
			battleController.targetSelectionManager.EnterTargetingMode(ability, metadata.TargetSelectText, delegate(UnitView selectedUnit)
			{
				if ((bool)selectedUnit)
				{
					PlayerActivateAbility(ability, selectedUnit.state);
				}
			});
		}
		else
		{
			PlayerActivateAbility(ability, null);
		}
	}

	private void PlayerActivateAbility(AbilityState ability, UnitState target)
	{
		UseAbilityAction useAbilityAction = UseAbilityAction.Create(ability, target);
		BattleLogic.ApplyBattleAction(ability.team, useAbilityAction);
		battleController.matchManager.playerActions.Add(useAbilityAction);
	}

	public void DepositOneEnergy(AbilityState ability)
	{
		if (battleController.playerTeam.energy == 0)
		{
			Log.Warning("Player tried to perform action without enough energy.");
			return;
		}
		int num = ability.metadata.Cost - ability.energy;
		if (num <= 1)
		{
			PlayerTryActivateAbility(ability);
			return;
		}
		InvestEnergyAction investEnergyAction = InvestEnergyAction.Create(ability);
		BattleLogic.ApplyBattleAction(ability.team, investEnergyAction);
		battleController.matchManager.playerActions.Add(investEnergyAction);
	}
}
