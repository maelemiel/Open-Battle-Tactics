using System.Collections.Generic;
using UnityEngine;

public class EffectPool
{
	private Dictionary<EffectType, List<EffectInstance>> effects = new Dictionary<EffectType, List<EffectInstance>>();

	public EffectInstance Get(EffectType type)
	{
		List<EffectInstance> list = GetList(type);
		if (list.Count > 0)
		{
			EffectInstance effectInstance = list[0];
			list.RemoveAt(0);
			effectInstance.Reset();
			effectInstance.gameObject.hideFlags = HideFlags.None;
			effectInstance.gameObject.SetActive(true);
			return effectInstance;
		}
		return null;
	}

	public void Put(EffectInstance effect)
	{
		List<EffectInstance> list = GetList(effect.type);
		if (!list.Contains(effect))
		{
			list.Add(effect);
		}
		effect.gameObject.hideFlags = HideFlags.HideInHierarchy;
		effect.gameObject.SetActive(false);
	}

	public void Purge(EffectType type)
	{
		List<EffectInstance> list = GetList(type);
		while (list.Count > 0)
		{
			EffectInstance effectInstance = list[0];
			list.RemoveAt(0);
			Object.Destroy(effectInstance.gameObject);
			effectInstance.gameObject = null;
			effectInstance.transform = null;
		}
	}

	public void PurgeAll()
	{
		foreach (KeyValuePair<EffectType, List<EffectInstance>> effect in effects)
		{
			Purge(effect.Key);
		}
	}

	public void Remove(EffectInstance effect)
	{
		List<EffectInstance> list = GetList(effect.type);
		if (list.Contains(effect))
		{
			list.Remove(effect);
		}
	}

	public int GetCount(EffectType type)
	{
		List<EffectInstance> list = GetList(type);
		return list.Count;
	}

	public bool Contains(EffectType type)
	{
		return GetCount(type) > 0;
	}

	private List<EffectInstance> GetList(EffectType type)
	{
		List<EffectInstance> list;
		if (effects.ContainsKey(type))
		{
			list = effects[type];
		}
		else
		{
			list = new List<EffectInstance>();
			effects[type] = list;
		}
		return list;
	}
}
