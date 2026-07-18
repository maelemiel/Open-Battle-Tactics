using System;
using System.Collections.Generic;
using System.Linq;

public class AbilityDataModel : BaseLocalizationDataModel, IResearchableDataModel, IAbilityMetadata
{
	public int abilityType;

	public int actionBoostType;

	public int actionBoostValueA;

	public int actionBoostValueB;

	public int actionPoint;

	public int actionType;

	public int executionOrder;

	public int handlerId;

	public int iconLinkageId;

	public int isActive;

	public int isAnnouncer;

	public int limitOnePerBattle;

	public int limitOnePerRound;

	public int numKillUnit;

	public int researchTime;

	public string selectionText;

	public int targetGroup;

	public int unlockOrder;

	public int unlockTier;

	public bool IsActive
	{
		get
		{
			return isActive > 0;
		}
	}

	public long ResearchDuration
	{
		get
		{
			return researchTime * 1000;
		}
	}

	public string ID
	{
		get
		{
			return id;
		}
	}

	public string Name
	{
		get
		{
			return string.Format(base.name, BoostValue, SecondaryBoostValue);
		}
	}

	public string ShortDescription
	{
		get
		{
			return base.description;
		}
	}

	public string TargetSelectText
	{
		get
		{
			return selectionText.Localize("Choose Target");
		}
	}

	public string Type
	{
		get
		{
			return AbilityHandlerDataModel.GetSingle(handlerId).handler;
		}
	}

	public int Cost
	{
		get
		{
			return actionPoint;
		}
	}

	public TargetType TargetSelectType
	{
		get
		{
			return (TargetType)(int)Enum.ToObject(typeof(TargetType), targetGroup);
		}
	}

	public bool LimitOnePerRound
	{
		get
		{
			return limitOnePerRound > 0;
		}
	}

	public int ExecutionOrder
	{
		get
		{
			return executionOrder;
		}
	}

	public string ButtonIconArtName
	{
		get
		{
			return Type;
		}
	}

	public int BoostValue
	{
		get
		{
			return actionBoostValueA;
		}
	}

	public float BoostValueFloat
	{
		get
		{
			return actionBoostValueA;
		}
	}

	public int SecondaryBoostValue
	{
		get
		{
			return actionBoostValueB;
		}
	}

	public float SecondaryBoostValueFloat
	{
		get
		{
			return actionBoostValueB;
		}
	}

	public bool IsUnitAbility
	{
		get
		{
			return false;
		}
	}

	public static AbilityDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<AbilityDataModel>(id.ToString());
	}

	public static AbilityDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<AbilityDataModel>(id);
	}

	public static List<AbilityDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<AbilityDataModel>();
	}

	public static string GetAbilityIDByType(string abilityType)
	{
		AbilityDataModel abilityByType = GetAbilityByType(abilityType);
		return (abilityByType == null) ? null : abilityByType.ID;
	}

	public static AbilityDataModel GetAbilityByType(string abilityType)
	{
		AbilityDataModel abilityDataModel = (from x in NonUnitySingleton<DMAccessManager>.instance.GetAll<AbilityDataModel>()
			where x.Type == abilityType
			select x).FirstOrDefault();
		if (abilityDataModel != null)
		{
			return abilityDataModel;
		}
		return null;
	}

	public static IEnumerable<AbilityDataModel> GetAbilitiesByDivision(int divisionId)
	{
		return from x in NonUnitySingleton<DMAccessManager>.instance.GetAll<AbilityDataModel>()
			where x.unlockTier == divisionId
			select x;
	}

	public UserPriceDataModel GetResearchCost(UserProfile userProfile)
	{
		return new UserPriceDataModel();
	}
}
