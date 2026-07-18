using System.Collections.Generic;
using LitJson0;

public class OpponentDataUtils
{
	public static List<IAbilityMetadata> GetAbilityListFromStrings(IEnumerable<string> strings)
	{
		List<IAbilityMetadata> list = new List<IAbilityMetadata>();
		foreach (string @string in strings)
		{
			if (!string.IsNullOrEmpty(@string) && !(@string == "0"))
			{
				IAbilityMetadata abilityMetadata = GetAbilityMetadata(@string, false);
				if (abilityMetadata != null)
				{
					list.Add(abilityMetadata);
				}
			}
		}
		return list;
	}

	public static IAbilityMetadata GetAbilityMetadata(string id, bool unitAbility)
	{
		if (unitAbility)
		{
			return NonUnitySingleton<DMAccessManager>.instance.GetSingle<UnitSpecialDataModel>(id);
		}
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<AbilityDataModel>(id);
	}

	public static OpponentData FillOpponentUnits(OpponentData opponent, List<JsonObject> userUnits)
	{
		opponent.units = new List<IUnitMetadata>();
		foreach (JsonObject userUnit in userUnits)
		{
			if (userUnit != null)
			{
				opponent.units.Add(UserUnit.FromJSON(userUnit));
			}
		}
		return opponent;
	}
}
