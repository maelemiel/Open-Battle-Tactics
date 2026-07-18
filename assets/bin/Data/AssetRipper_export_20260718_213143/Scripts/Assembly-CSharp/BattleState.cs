public class BattleState : ServerBattleState
{
	public new TeamState teamOne
	{
		get
		{
			return base.teamOne as TeamState;
		}
		set
		{
			base.teamOne = value;
		}
	}

	public new TeamState teamTwo
	{
		get
		{
			return base.teamTwo as TeamState;
		}
		set
		{
			base.teamTwo = value;
		}
	}

	public new TeamState hostTeam
	{
		get
		{
			return base.hostTeam as TeamState;
		}
		set
		{
			base.hostTeam = value;
		}
	}

	public new TeamState actingTeam
	{
		get
		{
			return base.actingTeam as TeamState;
		}
		set
		{
			base.actingTeam = value;
		}
	}

	public new TeamState winningTeam
	{
		get
		{
			return base.winningTeam as TeamState;
		}
		set
		{
			base.winningTeam = value;
		}
	}

	public new TeamState initiativeWinner
	{
		get
		{
			return base.initiativeWinner as TeamState;
		}
		set
		{
			base.initiativeWinner = value;
		}
	}
}
