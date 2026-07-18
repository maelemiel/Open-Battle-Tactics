using System.Collections.Generic;

public class UnitState : ServerUnitState
{
	public UnitView unitView;

	public new List<AbilityState> abilities
	{
		get
		{
			return base.abilities.ConvertAll<AbilityState>(AbilityStateConverter);
		}
	}

	public new TeamState team
	{
		get
		{
			return base.team as TeamState;
		}
		set
		{
			base.team = value;
		}
	}

	public string AbilityName
	{
		get
		{
			if (metadata.GetAbilitiesCount() > 0)
			{
				return UnitSpecialDataModel.GetName(metadata.GetAbilityMetaData(0), abilities[0].BoostValue, abilities[0].SecondaryBoostValue);
			}
			return string.Empty;
		}
	}

	public string Ability2Name
	{
		get
		{
			if (metadata.GetAbilitiesCount() > 1)
			{
				return UnitSpecialDataModel.GetName(metadata.GetAbilityMetaData(1), abilities[1].BoostValue, abilities[1].SecondaryBoostValue);
			}
			return string.Empty;
		}
	}

	public UserUnit UserUnitMetadata
	{
		get
		{
			return metadata as UserUnit;
		}
	}

	private static AbilityState AbilityStateConverter(ServerAbilityState state)
	{
		return state as AbilityState;
	}

	public void ForceCurrentRoll(int val)
	{
		currentRoll = val;
		roundStartRoll = val;
	}
}
