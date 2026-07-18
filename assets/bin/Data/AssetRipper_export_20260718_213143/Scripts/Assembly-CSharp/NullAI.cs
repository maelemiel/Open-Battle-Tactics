using System.Collections.Generic;

public class NullAI : BaseAI
{
	public override List<BattleAction> Think(ServerTeamState team)
	{
		return new List<BattleAction>();
	}
}
