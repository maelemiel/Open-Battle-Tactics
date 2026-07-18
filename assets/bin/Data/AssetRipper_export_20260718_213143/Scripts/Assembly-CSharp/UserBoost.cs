using UnityEngine;

public class UserBoost
{
	private BoostDataModel dataModel;

	public int metaID;

	public long expireTime;

	public float CountdownProgress
	{
		get
		{
			return Mathf.Clamp01((float)(expireTime - TimeManager.ServerTime) / (float)dataModel.DurationMillis);
		}
	}

	public BoostType Type
	{
		get
		{
			return dataModel.Type;
		}
	}

	public bool IsActive
	{
		get
		{
			return expireTime == 0L || NonUnitySingleton<TimeManager>.instance.IsTimestampInFuture(expireTime);
		}
	}

	public UserBoost(int metaID, long expireTime)
	{
		this.metaID = metaID;
		this.expireTime = expireTime;
		dataModel = BoostDataModel.GetSingle(metaID);
	}

	public int GetRemainingTime(long currentServerTime = -1)
	{
		if (currentServerTime == -1)
		{
			currentServerTime = TimeManager.ServerTime;
		}
		return (int)NonUnitySingleton<TimeManager>.instance.GetTimeDelta(expireTime);
	}
}
