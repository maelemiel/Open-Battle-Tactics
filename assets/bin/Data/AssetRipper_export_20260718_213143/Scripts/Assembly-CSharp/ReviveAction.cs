using LitJson0;

public class ReviveAction : BattleAction
{
	public bool usedA;

	public ReviveAction()
		: base("revive")
	{
	}

	public static ReviveAction Create()
	{
		return new ReviveAction();
	}

	public override JsonObject Serialize()
	{
		JsonObject jsonObject = base.Serialize();
		jsonObject.SetInt("used_a", usedA ? 1 : 0);
		return jsonObject;
	}

	public override void Deserialize(JsonObject json)
	{
		usedA = json.GetInt("used_a") == 1;
	}
}
