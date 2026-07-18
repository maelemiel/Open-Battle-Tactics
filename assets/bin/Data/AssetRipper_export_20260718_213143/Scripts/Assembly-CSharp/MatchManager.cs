using System;
using System.Collections.Generic;

public class MatchManager
{
	private Dictionary<MatchData.Type, Func<BaseMatchHandler>> registeredMatchTypes = new Dictionary<MatchData.Type, Func<BaseMatchHandler>>();

	private BattleController battleController;

	private MatchData matchData;

	private BaseMatchHandler matchHandler;

	public List<BattleAction> playerActions = new List<BattleAction>();

	public List<BattleAction> enemyActions = new List<BattleAction>();

	public MatchData MatchData
	{
		get
		{
			return matchData;
		}
		set
		{
			matchData = value;
		}
	}

	public BaseMatchHandler MatchHandler
	{
		get
		{
			return matchHandler;
		}
	}

	public void Register(MatchData.Type matchType, Func<BaseMatchHandler> creationFunction)
	{
		registeredMatchTypes[matchType] = creationFunction;
	}

	public void Init(BattleController battleController)
	{
		this.battleController = battleController;
		matchData = new MatchData();
		matchHandler = CreateMatchHandler();
	}

	private BaseMatchHandler CreateMatchHandler()
	{
		Func<BaseMatchHandler> value;
		if (!registeredMatchTypes.TryGetValue(battleController.MatchType, out value))
		{
			throw new Exception("Match type does not have a handler!");
		}
		BaseMatchHandler baseMatchHandler = value();
		baseMatchHandler.Init(battleController, this);
		return baseMatchHandler;
	}
}
