using LitJson0;

public class UseAbilityAction : BattleAction
{
	public string abilityID;

	public string targetUnitID;

	public UseAbilityAction()
		: base("use_ability")
	{
	}

	public static UseAbilityAction Create(ServerAbilityState abilityUsed, ServerUnitState targetUnit)
	{
		UseAbilityAction useAbilityAction = new UseAbilityAction();
		useAbilityAction.abilityID = abilityUsed.metadata.ID;
		useAbilityAction.targetUnitID = BattleAction.GetUnitID(targetUnit);
		return useAbilityAction;
	}

	public override JsonObject Serialize()
	{
		JsonObject jsonObject = base.Serialize();
		jsonObject.SetString("id", abilityID);
		jsonObject.SetString("targetUnit", targetUnitID);
		return jsonObject;
	}

	public override void Deserialize(JsonObject json)
	{
		abilityID = json.GetString("id");
		targetUnitID = json.GetString("targetUnit");
	}
}
