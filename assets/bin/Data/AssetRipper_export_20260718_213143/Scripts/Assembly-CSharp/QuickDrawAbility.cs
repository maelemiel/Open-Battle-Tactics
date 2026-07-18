public class QuickDrawAbility : ServerAbilityHandler
{
	public bool activated;

	public override bool UnitFiringEvent(ServerUnitState unit)
	{
		activated = false;
		if (unit == abilityState.target)
		{
			int nextTeamRandom = BattleLogic.GetNextTeamRandom(abilityState.team, 0, 100, "QuickDraw Ability Fire");
			if (nextTeamRandom <= abilityState.BoostValue)
			{
				activated = true;
				BattleLogic.UnitAttack(unit);
				for (int i = 1; i < abilityState.SecondaryBoostValue; i++)
				{
					if (unit.battle.IsComplete)
					{
						break;
					}
					if (unit.battle.animationHandler != null)
					{
						unit.battle.animationHandler.QuickDraw(abilityState);
					}
					BattleLogic.RerollUnit(unit, false, false, true);
					int currentRoll = unit.currentRoll;
					if (unit.battle.animationHandler != null)
					{
						unit.battle.animationHandler.ReRoll(abilityState, currentRoll);
					}
					if (unit.CurrentRollType == DieFaceType.Special && unit.abilities[0].handler.UnitFiringEvent(unit) && unit.battle.animationHandler != null)
					{
						unit.battle.animationHandler.UnitFiringAbility(unit, unit.abilities[0], unit.abilities[0].handler.target);
					}
					BattleLogic.UnitAttack(unit);
					BattleLogic.DeactivateDeadUnitAbilities(unit.battle);
				}
				return true;
			}
		}
		return false;
	}
}
