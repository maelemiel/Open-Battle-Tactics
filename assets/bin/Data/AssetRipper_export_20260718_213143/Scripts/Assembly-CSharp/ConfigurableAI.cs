using System.Collections.Generic;

public class ConfigurableAI : BaseAI
{
	private List<AIInstruction> instructionList = new List<AIInstruction>();

	public void AddInstruction(string abilityType)
	{
		AddTargetedInstruction(abilityType, null);
	}

	public void AddTargetedInstruction(string abilityType, ServerUnitState target)
	{
		AIInstruction aIInstruction = new AIInstruction();
		aIInstruction.abilityType = abilityType;
		aIInstruction.targetID = BattleAction.GetUnitID(target);
		instructionList.Add(aIInstruction);
	}

	public void AddActionInstruction(BattleAction action)
	{
		AIInstruction aIInstruction = new AIInstruction();
		aIInstruction.customAction = action;
		instructionList.Add(aIInstruction);
	}

	public override List<BattleAction> Think(ServerTeamState team)
	{
		team = CloneState(team);
		List<BattleAction> list = new List<BattleAction>();
		foreach (AIInstruction instruction in instructionList)
		{
			if (instruction.customAction != null)
			{
				list.Add(instruction.customAction);
				continue;
			}
			ServerAbilityState abilityByType = team.GetAbilityByType(instruction.abilityType);
			if (abilityByType != null)
			{
				ServerUnitState serverUnitState = BattleAction.GetUnitByID(instruction.targetID, team.battle);
				if (serverUnitState == null || serverUnitState.IsDead)
				{
					serverUnitState = BaseAI.GetRandomTargetForAbility(abilityByType);
				}
				InvestEnergyToActivateAbility(abilityByType, serverUnitState, list);
			}
		}
		return list;
	}
}
