using System.Collections.Generic;

public class RandomAI : BaseAI
{
	public override List<BattleAction> Think(ServerTeamState team)
	{
		team = CloneState(team);
		List<BattleAction> list = new List<BattleAction>();
		while (team.energy > 0)
		{
			ServerAbilityState ability = team.abilities[BattleLogic.GetNextTeamRandom(team, 0, team.abilities.Length, "RandomAI-Think")];
			ServerUnitState randomTargetForAbility = BaseAI.GetRandomTargetForAbility(ability);
			InvestEnergyToActivateAbility(ability, randomTargetForAbility, list);
		}
		return list;
	}
}
