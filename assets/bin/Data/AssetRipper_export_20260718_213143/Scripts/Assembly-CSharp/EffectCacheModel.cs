using System;
using System.Collections.Generic;
using UnityEngine;

public class EffectCacheModel : MonoBehaviour
{
	[Serializable]
	public class EffectCacheItem
	{
		public EffectType type;

		public int poolCount;
	}

	public List<EffectCacheItem> effectTypes = new List<EffectCacheItem>();

	private void Start()
	{
		List<EffectType> loadedEffects = Singleton<GlobalEffectsManager>.instance.GetLoadedEffects();
		foreach (EffectCacheItem effectType in effectTypes)
		{
			Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(effectType.type, effectType.poolCount);
			loadedEffects.Remove(effectType.type);
		}
		foreach (EffectType item in loadedEffects)
		{
			Singleton<GlobalEffectsManager>.instance.UnloadEffect(item);
		}
		if (loadedEffects.Count > 0)
		{
			Resources.UnloadUnusedAssets();
		}
	}

	public EffectCacheItem GetByType(EffectType type)
	{
		foreach (EffectCacheItem effectType in effectTypes)
		{
			if (effectType.type == type)
			{
				return effectType;
			}
		}
		return null;
	}
}
