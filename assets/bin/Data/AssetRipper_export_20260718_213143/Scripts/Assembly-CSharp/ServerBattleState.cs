public class ServerBattleState
{
	public int randomSeed;

	public int currentRound;

	public int cashPrizePool;

	public ServerTeamState teamOne;

	public ServerTeamState teamTwo;

	public ServerTeamState hostTeam;

	public ServerTeamState actingTeam;

	public ServerTeamState winningTeam;

	public ServerTeamState initiativeWinner;

	public IRandomProvider randomProvider;

	public IBattleConstantsProvider constantsProvider;

	public IBattleAnimationHandler animationHandler;

	public ServerTeamState[] bothTeams;

	public bool IsComplete
	{
		get
		{
			return teamOne.IsDead || teamTwo.IsDead || teamOne.forfeited || teamTwo.forfeited;
		}
	}

	public bool IsPvPMatch
	{
		get
		{
			return teamOne.type == TeamType.Player && teamTwo.type == TeamType.Player;
		}
	}

	public ServerBattleState Clone()
	{
		ServerBattleState serverBattleState = new ServerBattleState();
		serverBattleState.randomSeed = randomSeed;
		serverBattleState.currentRound = currentRound;
		serverBattleState.cashPrizePool = cashPrizePool;
		serverBattleState.teamOne = teamOne.Clone();
		serverBattleState.teamTwo = teamTwo.Clone();
		serverBattleState.hostTeam = _GetTeamReferenceInClone(serverBattleState, hostTeam);
		serverBattleState.actingTeam = _GetTeamReferenceInClone(serverBattleState, actingTeam);
		serverBattleState.winningTeam = _GetTeamReferenceInClone(serverBattleState, winningTeam);
		serverBattleState.initiativeWinner = _GetTeamReferenceInClone(serverBattleState, initiativeWinner);
		serverBattleState.constantsProvider = constantsProvider;
		serverBattleState.randomProvider = randomProvider.Clone();
		BattleLogic.SetupConvenienceData(serverBattleState);
		return serverBattleState;
	}

	private ServerTeamState _GetTeamReferenceInClone(ServerBattleState clonedBattle, ServerTeamState teamRef)
	{
		if (teamRef == null)
		{
			return null;
		}
		return (teamRef != teamOne) ? clonedBattle.teamTwo : clonedBattle.teamOne;
	}

	public int GetConstantInt(string name, int defaultValue)
	{
		if (constantsProvider == null || !constantsProvider.Contains(name))
		{
			return defaultValue;
		}
		return constantsProvider.GetInt(name);
	}
}
