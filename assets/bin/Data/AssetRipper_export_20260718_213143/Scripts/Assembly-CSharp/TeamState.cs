public class TeamState : ServerTeamState
{
	public BattleField battleField;

	public new UnitState[] units
	{
		get
		{
			return base.units as UnitState[];
		}
		set
		{
			base.units = value;
		}
	}

	public new AbilityState[] abilities
	{
		get
		{
			return base.abilities as AbilityState[];
		}
		set
		{
			base.abilities = value;
		}
	}

	public new TeamState otherTeam
	{
		get
		{
			return base.otherTeam as TeamState;
		}
		set
		{
			base.otherTeam = value;
		}
	}

	public bool IsPlayer { get; set; }

	public bool IsEnemy
	{
		get
		{
			return !IsPlayer;
		}
	}

	public new AbilityState GetAbilityByType(string type)
	{
		return base.GetAbilityByType(type) as AbilityState;
	}
}
