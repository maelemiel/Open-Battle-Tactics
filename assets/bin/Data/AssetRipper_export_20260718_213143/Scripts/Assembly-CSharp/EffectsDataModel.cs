using System;
using System.Collections.Generic;

public class EffectsDataModel : BaseDataModel
{
	public int assetLinkageId;

	public string effectName;

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

	public static EffectsDataModel GetSingle(int id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<EffectsDataModel>(id.ToString());
	}

	public static EffectsDataModel GetSingle(string id)
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetSingle<EffectsDataModel>(id);
	}

	public static List<EffectsDataModel> GetAll()
	{
		return NonUnitySingleton<DMAccessManager>.instance.GetAll<EffectsDataModel>();
	}

	private static void InitCache()
	{
		if (CacheManager.effectsCache != null)
		{
			return;
		}
		CacheManager.effectsCache = new Dictionary<int, EffectsDataModel>();
		foreach (EffectsDataModel item in GetAll())
		{
			EffectType key = (EffectType)(int)Enum.Parse(typeof(EffectType), item.effectName, true);
			CacheManager.effectsCache[(int)key] = item;
		}
	}

	public static EffectsDataModel GetByType(EffectType type)
	{
		InitCache();
		if (!CacheManager.effectsCache.ContainsKey((int)type))
		{
			return null;
		}
		return CacheManager.effectsCache[(int)type];
	}
}
