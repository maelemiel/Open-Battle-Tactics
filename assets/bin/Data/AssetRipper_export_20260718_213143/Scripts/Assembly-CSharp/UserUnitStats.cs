using LitJson0;

public class UserUnitStats
{
	public string metaId;

	public int built;

	public UserUnitStats(string metaId, int built)
	{
		this.metaId = metaId;
		this.built = built;
	}

	public void AddBuilt()
	{
		built++;
	}

	public static UserUnitStats FromJSON(JsonObject json)
	{
		string text = json.GetString("meta_id");
		int num = json.GetInt("built");
		return new UserUnitStats(text, num);
	}
}
