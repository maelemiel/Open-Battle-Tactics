using LitJson0;

public class BattleAction
{
	public const string USE_ABILITY = "use_ability";

	public const string INVEST_ABILITY = "invest_ability";

	public const string EMOTICON_ACTION = "emoticon_action";

	public const string FORFEIT = "forfeit";

	public const string REVIVE = "revive";

	protected string type;

	public BattleAction(string type)
	{
		this.type = type;
	}

	public virtual JsonObject Serialize()
	{
		JsonObject jsonObject = new JsonObject();
		jsonObject.SetString("type", type);
		return jsonObject;
	}

	public virtual void Deserialize(JsonObject json)
	{
	}

	public static BattleAction DeserializeAction(JsonObject json)
	{
		BattleAction battleAction = null;
		switch (json.GetString("type"))
		{
		case "use_ability":
			battleAction = new UseAbilityAction();
			break;
		case "invest_ability":
			battleAction = new InvestEnergyAction();
			break;
		case "emoticon_action":
			battleAction = new EmoticonAction();
			break;
		case "forfeit":
			battleAction = new ForfeitAction();
			break;
		case "revive":
			battleAction = new ReviveAction();
			break;
		}
		if (battleAction != null)
		{
			battleAction.Deserialize(json);
		}
		return battleAction;
	}

	public static ServerUnitState GetUnitByID(string id, ServerBattleState battle)
	{
		if (string.IsNullOrEmpty(id))
		{
			return null;
		}
		return ((id[0] != '0') ? battle.hostTeam.otherTeam : battle.hostTeam).units[int.Parse(id[1].ToString())];
	}

	public static string GetUnitID(ServerUnitState unit)
	{
		if (unit == null)
		{
			return string.Empty;
		}
		return ((unit.team != unit.team.battle.hostTeam) ? "1" : "0") + unit.index;
	}
}
