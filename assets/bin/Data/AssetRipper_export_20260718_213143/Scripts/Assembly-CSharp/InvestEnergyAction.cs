using LitJson0;

public class InvestEnergyAction : BattleAction
{
	public string abilityID;

	public InvestEnergyAction()
		: base("invest_ability")
	{
	}

	public static InvestEnergyAction Create(ServerAbilityState abilityUsed)
	{
		InvestEnergyAction investEnergyAction = new InvestEnergyAction();
		investEnergyAction.abilityID = abilityUsed.metadata.ID;
		return investEnergyAction;
	}

	public override JsonObject Serialize()
	{
		JsonObject jsonObject = base.Serialize();
		jsonObject.SetString("id", abilityID);
		return jsonObject;
	}

	public override void Deserialize(JsonObject json)
	{
		abilityID = json.GetString("id");
	}
}
