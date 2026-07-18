using System;
using System.Linq;

public class BattleOutputHandler : IBattleAnimationHandler
{
	private Action<string> logFunction;

	public BattleOutputHandler(Action<string> logFunction = null)
	{
		this.logFunction = logFunction;
		if (logFunction == null)
		{
			this.logFunction = delegate(string str)
			{
				Console.WriteLine(str);
			};
		}
	}

	private void print(params object[] strs)
	{
		logFunction(string.Join(string.Empty, strs.Select((object x) => x.ToString()).ToArray()));
	}

	public void RoundStart(int roundIndex)
	{
		print("=============================================================");
		print("============== Round ", roundIndex.ToString(), " ======================================");
		print("=============================================================");
	}

	public void RoundComplete(int roundIndex)
	{
		print("Round ", roundIndex, " Complete");
	}

	public void QuickDraw(ServerAbilityState ability)
	{
		print("Quick Draw");
	}

	public void PreInitiativeAbility(ServerAbilityState ability)
	{
		print("PreInitiativeEvent Ability Activates! ", GetTeamName(ability.team), " ", ability.metadata.Name);
	}

	public void PreInitiativeResultsAbility(ServerAbilityState ability, int intelBoostTeam, int intelBoostOtherTeam)
	{
		print("PreInitiativeResults Ability Activates! ", GetTeamName(ability.team), " intelBoostTeam " + intelBoostTeam + " intelBoostOtherTeam " + intelBoostOtherTeam);
	}

	public void InitiativeResults(ServerTeamState winningTeam, int hostTeamInitiative, int guestTeamInitiative)
	{
		bool flag = winningTeam == winningTeam.battle.hostTeam;
		int num = ((!flag) ? guestTeamInitiative : hostTeamInitiative);
		int num2 = ((!flag) ? hostTeamInitiative : guestTeamInitiative);
		print(GetTeamName(winningTeam), " Wins Initiative with " + num, " vs ", num2);
		print("Random seed after simulating Initiative: " + winningTeam.battle.randomSeed);
	}

	public void PostInitiativeAbility(ServerAbilityState ability)
	{
		print("PostInitiativeEvent Ability Activates! ", GetTeamName(ability.team), " ", ability.metadata.Name);
	}

	public void TeamBeginAttack(ServerTeamState team)
	{
		print("TeamBeginAttack ", GetTeamName(team));
	}

	public void TeamBeginAttackAbility(ServerTeamState team, ServerAbilityState ability)
	{
		print("TeamBeginAttackAbility Ability Activates! ", GetTeamName(ability.team), " ", ability.metadata.Name);
	}

	public void PassiveFiringAnimation(ServerUnitState unit, ServerAbilityState ability)
	{
		print("PassiveFiringEvent Ability Activates! ", GetTeamName(ability.team), " ", ability.metadata.Name);
	}

	public void UnitFiringAbility(ServerUnitState unit, ServerAbilityState ability, ServerUnitState target)
	{
		print("UnitFiringEvent Ability Activates! ", GetTeamName(ability.team), " ", ability.metadata.Name);
	}

	public void UnitAttack(ServerUnitState unit, ServerUnitState target, int damage, DamageType damageType)
	{
		print("Unit " + GetUnitName(unit), " attacks Unit ", GetUnitName(target), " for ", damage, " damage of type ", damageType.ToString());
	}

	public void UnitReceivedDamageAbility(ServerUnitState unit, int damage, DamageType dmgType, ServerAbilityState ability)
	{
		print("Unit " + GetUnitName(unit), " was damaged for " + damage + " of type " + dmgType);
	}

	public void UnitDiedAbility(ServerUnitState unit, ServerTeamState team, ServerAbilityState ability)
	{
		print("Unit " + GetUnitName(unit), " DIED at the hands of ", GetTeamName(team));
	}

	public void TeamEndAttack(ServerTeamState team)
	{
		print("TeamEndAttack ", GetTeamName(team));
	}

	public void PreFireAbility(ServerAbilityState ability, ServerUnitState unit, ServerUnitState target)
	{
		print("Pre Fire Will be Activated on ", GetTeamName(ability.team), ": ", ability.metadata.Name);
	}

	public void PreActivateAbility(ServerAbilityState ability, ServerUnitState target)
	{
		print("Ability Will be Activated on ", GetTeamName(ability.team), ": ", ability.metadata.Name, "  on target: ", GetUnitName(target));
	}

	public void ActivateAbility(ServerAbilityState ability, ServerUnitState target)
	{
		print("Ability Activated on ", GetTeamName(ability.team), ": ", ability.metadata.Name, "  on target: ", GetUnitName(target));
	}

	public void DeactivateAbility(ServerAbilityState ability)
	{
		print("Ability Deactivated on ", GetTeamName(ability.team), ": ", ability.metadata.Name);
	}

	public void ReRoll(ServerAbilityState ability, int index)
	{
		print("Ability Rerolled ", GetTeamName(ability.team), " ", GetUnitName(ability.target));
	}

	public void JamAbility(ServerAbilityState jammedAbility, ServerAbilityState jammerAbility)
	{
		print("Ability Jammed on ", GetTeamName(jammedAbility.team), ": ", jammedAbility.metadata.Name, " by ", jammerAbility.metadata.Name);
	}

	public void InvestAbilityEnergy(ServerAbilityState ability)
	{
		print(GetTeamName(ability.team), " Invests 1 point of energy into " + ability.metadata.Name);
	}

	public void FinishBattle(ServerTeamState winningTeam)
	{
		string text = ((winningTeam != null) ? GetTeamName(winningTeam) : "NOBODY");
		print("Battle Complete.  ", text, " Wins");
	}

	public void TeamForfeit(ServerTeamState team)
	{
		print("Team has forfeited: ", GetTeamName(team));
	}

	public void TeamBestRoll(ServerTeamState team)
	{
		print("Team has best roll: ", GetTeamName(team));
	}

	public void TeamWorstRoll(ServerTeamState team)
	{
		print("Team has worst roll: ", GetTeamName(team));
	}

	public void UnitWorstRolls(ServerUnitState unit)
	{
		print("Unit had worst rolls!", GetUnitName(unit));
	}

	public void UnitAcidDamage(ServerUnitState unit, int acidIncrease)
	{
		print("Unit has taken Acid damage", GetUnitName(unit));
	}

	public void ApplyDamagePerRound()
	{
		print("Applying damage per round");
	}

	public static string GetTeamName(ServerTeamState team)
	{
		return (team.battle.teamOne != team) ? "TeamTwo" : "TeamOne";
	}

	public static string GetUnitName(ServerUnitState unit)
	{
		return BattleAction.GetUnitID(unit);
	}

	public void EndOfRoundWithAbilities(ServerTeamState team, ServerAbilityState ability)
	{
		print("End of Round Ability with team: ", GetTeamName(team), " And ability " + ability.metadata.Name);
	}
}
