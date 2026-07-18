using LitJson0;

public class EmoticonAction : BattleAction
{
	public string emoticonName = string.Empty;

	public EmoticonAction()
		: base("emoticon_action")
	{
	}

	public static EmoticonAction Create(string emoticonName)
	{
		EmoticonAction emoticonAction = new EmoticonAction();
		emoticonAction.emoticonName = emoticonName;
		return emoticonAction;
	}

	public override JsonObject Serialize()
	{
		JsonObject jsonObject = base.Serialize();
		jsonObject.SetString("emoticon_name", emoticonName);
		return jsonObject;
	}

	public override void Deserialize(JsonObject json)
	{
		emoticonName = json.GetString("emoticon_name");
	}
}
