using System.Collections.Generic;

public class UnitSpecialDataModel : BaseLocalizationDataModel, IAbilityMetadata
{
	public int actionBoostType;

	public string assetBundleId;

	public int executionOrder;

	public int handlerId;

	public int relatedActionId;

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
			return base.name;
		}
	}

	public string Type
	{
		get
		{
			if (handlerId == 0)
			{
				return null;
			}
			UnitSpecialHandlerDataModel single = UnitSpecialHandlerDataModel.GetSingle(handlerId);
			if (single != null)
			{
				return single.handler;
			}
			return null;
		}
	}

	public string ShortDescription
	{
		get
		{
			return base.description;
		}
	}

	public int ExecutionOrder
	{
		get
		{
			return executionOrder;
		}
	}

	public bool IsUnitAbility
	{
		get
		{
			return true;
		}
	}

	public int Cost
	{
		get
		{
			return 0;
		}
	}

	public TargetType TargetSelectType
	{
		get
		{
			return TargetType.None;
		}
	}

	public string TargetSelectText
	{
		get
		{
			return null;
		}
	}

	public string ButtonIconArtName
	{
		get
		{
			return null;
		}
	}

	public int BoostValue
	{
		get
		{
			return 0;
		}
	}

	public float BoostValueFloat
	{
		get
		{
			return 0f;
		}
	}

	public int SecondaryBoostValue
	{
		get
		{
			return 0;
		}
	}

	public float SecondaryBoostValueFloat
	{
		get
		{
			return 0f;
		}
	}

	public bool LimitOnePerRound
	{
		get
		{
			return false;
		}
	}

	public float SpecificBosstMultiplier
	{
		get
		{
			return 1f;
		}
	}

	public static UnitSpecialDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<UnitSpecialDataModel>(id.ToString());
	}

	public static UnitSpecialDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<UnitSpecialDataModel>(id);
	}

	public static List<UnitSpecialDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<UnitSpecialDataModel>();
	}

	public static string GetName(IAbilityMetadata specialDataModel, int valueA, int valueB)
	{
		switch (specialDataModel.Type)
		{
		case "boost_dice":
		{
			DieFaceType faceType3 = GetFaceType(valueB.ToString());
			return string.Format(specialDataModel.Name, faceType3.GetDieFaceName(), valueA);
		}
		case "boost_dice_special":
		{
			string faceType = valueB.ToString().Remove(1) + "0000";
			DieFaceType faceType2 = GetFaceType(faceType);
			string text2 = valueB.ToString().Remove(0, 1);
			UnitSpecialDataModel single2 = GetSingle(text2);
			return string.Format(specialDataModel.Name, faceType2.GetDieFaceName(), string.Format(single2.Name, string.Empty, string.Empty).Trim(), valueA);
		}
		case "boost_special":
		case "boost_special_percentage":
		{
			string text = valueB.ToString();
			UnitSpecialDataModel single = GetSingle(text);
			return string.Format(specialDataModel.Name, string.Format(single.Name, string.Empty, string.Empty).Trim(), valueA);
		}
		default:
			return string.Format(specialDataModel.Name, valueA, valueB);
		}
	}

	public static string GetDescription(IAbilityMetadata specialDataModel, int valueA, int valueB)
	{
		switch (specialDataModel.Type)
		{
		case "boost_dice":
		{
			DieFaceType faceType3 = GetFaceType(valueB.ToString());
			return string.Format(specialDataModel.ShortDescription, faceType3.GetDieFaceName(), valueA);
		}
		case "boost_dice_special":
		{
			string faceType = valueB.ToString().Remove(1) + "0000";
			DieFaceType faceType2 = GetFaceType(faceType);
			string text2 = valueB.ToString().Remove(0, 1);
			UnitSpecialDataModel single2 = GetSingle(text2);
			return string.Format(specialDataModel.ShortDescription, faceType2.GetDieFaceName(), string.Format(single2.ShortDescription, string.Empty, string.Empty).Trim(), valueA);
		}
		case "boost_special":
		case "boost_special_percentage":
		{
			string text = valueB.ToString();
			UnitSpecialDataModel single = GetSingle(text);
			return string.Format(specialDataModel.ShortDescription, string.Format(single.ShortDescription, string.Empty, string.Empty).Trim(), valueA);
		}
		default:
			return string.Format(specialDataModel.ShortDescription, valueA, valueB);
		}
	}

	private static DieFaceType GetFaceType(string faceType)
	{
		if (!UnitLevelProgressionDataModel.faceTypeMap.ContainsKey(faceType))
		{
			Log.Error("BoostDiceAbility: Could not find Dice face type: " + faceType);
			return DieFaceType.None;
		}
		return UnitLevelProgressionDataModel.faceTypeMap[faceType];
	}
}
