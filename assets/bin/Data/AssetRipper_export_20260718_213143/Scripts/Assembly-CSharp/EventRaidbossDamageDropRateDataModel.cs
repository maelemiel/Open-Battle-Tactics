using System.Collections.Generic;

public class EventRaidbossDamageDropRateDataModel : BaseDataModel
{
	public int boxtype;

	public int giftid;

	public int teamid;

	public int threshold;

	private static Dictionary<int, List<EventRaidbossDamageDropRateDataModel>> cachedDropRates = new Dictionary<int, List<EventRaidbossDamageDropRateDataModel>>();

	public ItemGiftDataModel Gift
	{
		get
		{
			return ItemGiftDataModel.GetSingle(giftid);
		}
	}

	public static EventRaidbossDamageDropRateDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<EventRaidbossDamageDropRateDataModel>(id.ToString());
	}

	public static EventRaidbossDamageDropRateDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<EventRaidbossDamageDropRateDataModel>(id);
	}

	public static List<EventRaidbossDamageDropRateDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<EventRaidbossDamageDropRateDataModel>();
	}

	public static List<EventRaidbossDamageDropRateDataModel> GetTeamDamageDropRates(int unitid)
	{
		if (cachedDropRates.ContainsKey(unitid))
		{
			return cachedDropRates[unitid];
		}
		List<EventRaidbossDamageDropRateDataModel> list = GetAll().FindAll((EventRaidbossDamageDropRateDataModel model) => model.teamid == unitid);
		if (list == null)
		{
			list = new List<EventRaidbossDamageDropRateDataModel>();
		}
		cachedDropRates.Add(unitid, list);
		return list;
	}
}
