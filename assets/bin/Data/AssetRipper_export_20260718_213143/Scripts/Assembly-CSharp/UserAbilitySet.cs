using System.Collections.Generic;

public class UserAbilitySet
{
	public int index;

	public List<string> abilities;

	public int Count
	{
		get
		{
			return abilities.Count;
		}
	}

	public UserAbilitySet()
	{
	}

	public UserAbilitySet(List<string> abilitiesSet, int newIndex)
	{
		abilities = abilitiesSet;
		index = newIndex;
	}

	public bool Contains(string abilityID)
	{
		foreach (string ability in abilities)
		{
			if (!string.IsNullOrEmpty(ability) && string.Equals(ability, abilityID))
			{
				return true;
			}
		}
		return false;
	}
}
