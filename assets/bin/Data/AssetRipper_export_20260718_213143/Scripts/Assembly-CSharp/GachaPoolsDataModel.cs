using System;
using System.Collections.Generic;
using System.Globalization;

public class GachaPoolsDataModel : BaseLocalizationDataModel, IPurchasableDataModel, IPrizeGachaMetadata
{
	private UserPriceDataModel _cachedPrice;

	private long cachedStartTimeStamp = -1L;

	private long cachedEndTimeStamp = -1L;

	public int abTestingId;

	public int assetLinkageId;

	public int availableCountdown;

	public int boxCount;

	public int bundleGiftId;

	public string dateEnd;

	public string dateStart;

	public int eventId;

	public int freeCooldown;

	public int gachaType;

	public int orderPosition;

	public int priceId;

	public int size;

	public int stepUpDrawLimit;

	public int stepUpId;

	public int stepUpNum;

	public int unlockDivision;

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

	public string Description
	{
		get
		{
			return base.description;
		}
	}

	public float FreeCooldownTime
	{
		get
		{
			return freeCooldown;
		}
	}

	public float AvailableCountdownTime
	{
		get
		{
			return availableCountdown;
		}
	}

	public GachaTypes GachaType
	{
		get
		{
			return (GachaTypes)gachaType;
		}
	}

	public AssetLinkageDataModel AssetLinkage
	{
		get
		{
			return AssetLinkageDataModel.GetSingle(assetLinkageId);
		}
	}

	public bool IsActive
	{
		get
		{
			return NonUnitySingleton<TimeManager>.instance.IsTimestampInPast(DateStartTimeStamp) && NonUnitySingleton<TimeManager>.instance.IsTimestampInFuture(DateEndTimeStamp);
		}
	}

	public long DateStartTimeStamp
	{
		get
		{
			if (cachedStartTimeStamp == -1)
			{
				cachedStartTimeStamp = TimeManager.TimeToUnixTimeStamp(DateTime.Parse(dateStart, CultureInfo.InvariantCulture).ToUniversalTime());
			}
			return cachedStartTimeStamp;
		}
	}

	public long DateEndTimeStamp
	{
		get
		{
			if (cachedEndTimeStamp == -1)
			{
				cachedEndTimeStamp = TimeManager.TimeToUnixTimeStamp(DateTime.Parse(dateEnd, CultureInfo.InvariantCulture).ToUniversalTime());
			}
			return cachedEndTimeStamp;
		}
	}

	public static GachaPoolsDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<GachaPoolsDataModel>(id.ToString());
	}

	public static GachaPoolsDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<GachaPoolsDataModel>(id);
	}

	public static List<GachaPoolsDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<GachaPoolsDataModel>();
	}

	public UserPriceDataModel GetPrice()
	{
		if (_cachedPrice == null)
		{
			_cachedPrice = ItemPriceDataModel.GetPriceForID(priceId);
		}
		return _cachedPrice;
	}
}
