using LitJson0;

public class MatchParserUtils
{
	public static IUnitMetadata GetUnitMetadata(JsonObject unitJson)
	{
		return UserUnit.FromJSON(unitJson);
	}

	public static IAbilityMetadata GetAbilityMetadata(string abilityID)
	{
		return AbilityDataModel.GetSingle(abilityID);
	}

	public static IDivisionMetadata GetDivisionMetadata(string divisionID)
	{
		return ProgressionDivisionDataModel.GetSingle(divisionID);
	}
}
