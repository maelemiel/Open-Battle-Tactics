using System;
using System.Collections.Generic;
using System.Globalization;

public class GachaAbTestingDataModel : BaseDataModel
{
	public int enable;

	public string endDate;

	public int groupsCount;

	public string startDate;

	public long DateStartTimeStamp
	{
		get
		{
			return TimeManager.TimeToUnixTimeStamp(DateTime.Parse(startDate, CultureInfo.InvariantCulture).ToUniversalTime());
		}
	}

	public long DateEndTimeStamp
	{
		get
		{
			return TimeManager.TimeToUnixTimeStamp(DateTime.Parse(endDate, CultureInfo.InvariantCulture).ToUniversalTime());
		}
	}

	public bool Enabled
	{
		get
		{
			if (enable == 0)
			{
				return false;
			}
			return NonUnitySingleton<TimeManager>.instance.IsTimestampInPast(DateStartTimeStamp) && NonUnitySingleton<TimeManager>.instance.IsTimestampInFuture(DateEndTimeStamp);
		}
	}

	public bool UserIsInABTesting
	{
		get
		{
			if (!Enabled)
			{
				return false;
			}
			int num = UserProfile.player.ABTestingGroup(this);
			List<GachaAbTestingGroupEnableDataModel> list = GachaAbTestingGroupEnableDataModel.GetAll().FindAll((GachaAbTestingGroupEnableDataModel x) => x.abTestingId.ToString() == id);
			if (list.Count == 0)
			{
				return false;
			}
			for (int num2 = 0; num2 < list.Count; num2++)
			{
				if (num == list[num2].groupNumber)
				{
					return true;
				}
			}
			return false;
		}
	}

	public static GachaAbTestingDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<GachaAbTestingDataModel>(id.ToString());
	}

	public static GachaAbTestingDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<GachaAbTestingDataModel>(id);
	}

	public static List<GachaAbTestingDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<GachaAbTestingDataModel>();
	}
}
