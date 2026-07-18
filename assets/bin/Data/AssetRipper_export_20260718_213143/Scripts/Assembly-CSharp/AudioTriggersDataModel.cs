using System;
using System.Collections.Generic;

public class AudioTriggersDataModel : BaseDataModel
{
	public int assetLinkageId;

	public string audioTriggerName;

	public AssetLinkageDataModel AssetLinkage
	{
		get
		{
			if (assetLinkageId == 0)
			{
				return null;
			}
			return AssetLinkageDataModel.GetSingle(assetLinkageId);
		}
	}

	public static AudioTriggersDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<AudioTriggersDataModel>(id.ToString());
	}

	public static AudioTriggersDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<AudioTriggersDataModel>(id);
	}

	public static List<AudioTriggersDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<AudioTriggersDataModel>();
	}

	private static void InitCache()
	{
		if (CacheManager.audioTriggersCache != null)
		{
			return;
		}
		CacheManager.audioTriggersCache = new Dictionary<int, AudioTriggersDataModel>();
		foreach (AudioTriggersDataModel item in GetAll())
		{
			AudioTrigger key = (AudioTrigger)(int)Enum.Parse(typeof(AudioTrigger), item.audioTriggerName, true);
			CacheManager.audioTriggersCache.Add((int)key, item);
		}
	}

	public static AudioTriggersDataModel GetByType(AudioTrigger type)
	{
		InitCache();
		if (!CacheManager.audioTriggersCache.ContainsKey((int)type))
		{
			return null;
		}
		return CacheManager.audioTriggersCache[(int)type];
	}
}
