using System.Collections.Generic;

public class EventUnitBoostDataModel : BaseDataModel
{
	public int ability1BoostA;

	public int ability1BoostB;

	public int ability1Override;

	public int ability2BoostA;

	public int ability2BoostB;

	public int ability2Override;

	public int bonusPointsBoost;

	public int dieBoostAcidStrike;

	public int dieBoostArmourPiercing;

	public int dieBoostDamage;

	public int dieBoostInitiative;

	public int eventId;

	public int hpBoost;

	public int level;

	public int unitId;

	public static EventUnitBoostDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<EventUnitBoostDataModel>(id.ToString());
	}

	public static EventUnitBoostDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<EventUnitBoostDataModel>(id);
	}

	public static List<EventUnitBoostDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<EventUnitBoostDataModel>();
	}

	public static EventUnitBoostDataModel FindUnitBoost(string unitid, int level, string eventid)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingleByQuery<EventUnitBoostDataModel>(" WHERE unit_id = " + unitid + " AND level = " + level + " AND event_id = " + eventid);
	}
}
