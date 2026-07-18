using LitJson0;

public class ForfeitAction : BattleAction
{
	public ForfeitAction()
		: base("forfeit")
	{
	}

	public static ForfeitAction Create()
	{
		return new ForfeitAction();
	}

	public override JsonObject Serialize()
	{
		return base.Serialize();
	}

	public override void Deserialize(JsonObject json)
	{
	}
}
