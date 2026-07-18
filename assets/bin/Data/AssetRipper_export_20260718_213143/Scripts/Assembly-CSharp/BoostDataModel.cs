using System;
using System.Collections.Generic;

public class BoostDataModel : BaseLocalizationDataModel
{
	private UserPriceDataModel cachedPrice;

	public string boostType;

	public int cashMultiplier;

	public int durationMinutes;

	public string eventType;

	public int gemMultiplier;

	public int isActive;

	public string keyMultiplier1;

	public string keyMultiplier2;

	public string keyShortDescription;

	public int multiplier1;

	public int multiplier2;

	public int partsMultiplier;

	public int priceId;

	public int tierMultiplier;

	public UserPriceDataModel Price
	{
		get
		{
			return (cachedPrice == null) ? (cachedPrice = ItemPriceDataModel.GetPriceForID(priceId)) : cachedPrice;
		}
	}

	public long DurationMillis
	{
		get
		{
			return durationMinutes * 60 * 1000;
		}
	}

	public string Description
	{
		get
		{
			string empty = string.Empty;
			return string.Format(arg1: (durationMinutes < 60) ? durationMinutes.ToString() : (durationMinutes / 60).ToString(), format: base.description, arg0: multiplier1.ToString());
		}
	}

	public string ShortDescription
	{
		get
		{
			return Singleton<LocalizationManager>.instance.Get(keyShortDescription);
		}
	}

	public string Multiplier1String
	{
		get
		{
			string format = Singleton<LocalizationManager>.instance.Get(keyMultiplier1);
			return string.Format(format, multiplier1);
		}
	}

	public string Multiplier2String
	{
		get
		{
			string format = Singleton<LocalizationManager>.instance.Get(keyMultiplier2);
			return string.Format(format, multiplier2);
		}
	}

	public BoostType Type
	{
		get
		{
			return (BoostType)(int)Enum.Parse(typeof(BoostType), boostType, true);
		}
	}

	public float Multiplier1
	{
		get
		{
			return (float)multiplier1 / 100f;
		}
	}

	public float Multiplier2
	{
		get
		{
			return (float)multiplier2 / 100f;
		}
	}

	public float PartsMultiplier
	{
		get
		{
			return (float)partsMultiplier / 100f;
		}
	}

	public float CashMultiplier
	{
		get
		{
			return (float)cashMultiplier / 100f;
		}
	}

	public float TierMultiplier
	{
		get
		{
			return (float)tierMultiplier / 100f;
		}
	}

	public float GemsMultiplier
	{
		get
		{
			return (float)gemMultiplier / 100f;
		}
	}

	public static BoostDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<BoostDataModel>(id.ToString());
	}

	public static BoostDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<BoostDataModel>(id);
	}

	public static List<BoostDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<BoostDataModel>();
	}

	public static BoostDataModel GetBoostByType(BoostType boostType)
	{
		return GetAll().Find((BoostDataModel x) => x.isActive == 1 && x.boostType == boostType.ToString());
	}
}
