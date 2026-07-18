using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GlobalEffectsManager : Singleton<GlobalEffectsManager>
{
	private const string PATH_FILE = "EffectPaths";

	public static bool LOG_ACTIVITY;

	public static bool WARN_ON_CACHE_MISS = true;

	public static bool WARN_ON_POOL_CREATE = true;

	private EffectPathData pathData;

	private EffectPool effectPool;

	private Dictionary<EffectType, GameObject> loadedEffects;

	private Dictionary<EffectType, Action> effectsCurrentlyLoading = new Dictionary<EffectType, Action>();

	private Dictionary<EffectType, AssetBundleDataModel> loadedBundles = new Dictionary<EffectType, AssetBundleDataModel>();

	public EffectPool Pool
	{
		get
		{
			return effectPool;
		}
	}

	private void Awake()
	{
		effectPool = new EffectPool();
		loadedEffects = new Dictionary<EffectType, GameObject>();
		pathData = Resources.Load("EffectPaths") as EffectPathData;
		if (pathData == null)
		{
			Log.Error("Effect paths .asset not found");
		}
	}

	public List<EffectType> GetLoadedEffects()
	{
		return loadedEffects.Keys.ToList();
	}

	public static IEnumerator WaitUntilEffectsLoaded()
	{
		while (Singleton<GlobalEffectsManager>.instance.effectsCurrentlyLoading.Count > 0)
		{
			yield return 0;
		}
	}

	public static IEnumerator CreateCoroutine(EffectType type, Vector3 worldPosition, GameObject parent, Action<EffectInstance> successCallback)
	{
		bool loaded = false;
		Singleton<GlobalEffectsManager>.instance.EnsureEffectIsLoaded(type, delegate
		{
			loaded = true;
		});
		while (!loaded)
		{
			yield return 0;
		}
		EffectInstance effect = Create(type, worldPosition, parent);
		if (successCallback != null)
		{
			successCallback(effect);
		}
	}

	public static EffectInstance Create(EffectType type, Vector3 worldPosition, GameObject parent)
	{
		return Singleton<GlobalEffectsManager>.instance._Create(type, worldPosition, (!parent) ? null : parent.transform);
	}

	public static EffectInstance Create(EffectType type, Vector3 worldPosition, Transform parent = null)
	{
		return Singleton<GlobalEffectsManager>.instance._Create(type, worldPosition, parent);
	}

	private EffectInstance _Create(EffectType type, Vector3 worldPosition, Transform parent = null)
	{
		if (WARN_ON_CACHE_MISS && !loadedEffects.ContainsKey(type))
		{
			Log.Warning("Effect Cache Miss. Loading effect: " + type);
		}
		if (!loadedEffects.ContainsKey(type) && EffectsDataModel.GetByType(type) != null)
		{
			Log.Error(string.Concat("Effect type ", type, " has an asset bundle, but was used without being loaded first."));
			return null;
		}
		EnsureEffectIsLoaded(type);
		EffectInstance effectInstance;
		if (effectPool.Contains(type))
		{
			if (LOG_ACTIVITY)
			{
				Log.Info("FX::Get(pool) " + type);
			}
			effectInstance = effectPool.Get(type);
		}
		else
		{
			if (WARN_ON_POOL_CREATE)
			{
				Log.Warning("EffectPool is empty. Instantiating new effect: " + type);
			}
			if (LOG_ACTIVITY)
			{
				Log.Info("FX::Get(createNew) " + type);
			}
			effectInstance = _CreateNew(type);
		}
		effectInstance.transform.position = worldPosition;
		if (parent != null && effectInstance.transform.parent != parent)
		{
			Vector3 localScale = effectInstance.transform.localScale;
			effectInstance.transform.parent = parent;
			effectInstance.transform.localScale = localScale;
			effectInstance.gameObject.SetLayerRecursively(parent.gameObject.layer);
		}
		return effectInstance;
	}

	private EffectInstance _CreateNew(EffectType type)
	{
		GameObject obj = UnityEngine.Object.Instantiate(loadedEffects[type], Vector3.zero, Quaternion.identity) as GameObject;
		return EffectInstance.Create(type, obj);
	}

	public static void Return(GameObject obj)
	{
		EffectInstance component = obj.GetComponent<EffectInstance>();
		if (component != null)
		{
			Singleton<GlobalEffectsManager>.instance._Return(component);
			return;
		}
		Log.Warning("An effect was returned that did not originate from the pool. Destroying, just in case...");
		UnityEngine.Object.Destroy(component);
	}

	public static void Return(EffectInstance effect)
	{
		Singleton<GlobalEffectsManager>.instance._Return(effect);
	}

	private void _Return(EffectInstance effect)
	{
		if (LOG_ACTIVITY)
		{
			Log.Info("FX::Return " + effect.type);
		}
		effectPool.Put(effect);
	}

	public void LoadEffect(EffectType type, Action callback)
	{
		if (LOG_ACTIVITY)
		{
			Log.Info("FX::LoadEffect " + type);
		}
		EffectsDataModel byType = EffectsDataModel.GetByType(type);
		if (byType == null || byType.AssetLinkage == null)
		{
			GameObject gameObject = Resources.Load<GameObject>(pathData.GetPath(type));
			if (gameObject != null)
			{
				loadedEffects[type] = gameObject;
			}
			else
			{
				Log.Error(string.Concat("Error loading effect type '", type, "' - effect was null"));
			}
			if (callback != null)
			{
				callback();
			}
		}
		else if (!effectsCurrentlyLoading.ContainsKey(type))
		{
			effectsCurrentlyLoading[type] = callback;
			LoadAssetBundle(type, delegate(GameObject asset)
			{
				loadedEffects[type] = asset;
				effectsCurrentlyLoading[type]();
				effectsCurrentlyLoading.Remove(type);
			});
		}
		else
		{
			Dictionary<EffectType, Action> dictionary2;
			Dictionary<EffectType, Action> dictionary = (dictionary2 = effectsCurrentlyLoading);
			EffectType key2;
			EffectType key = (key2 = type);
			Action a = dictionary2[key2];
			dictionary[key] = (Action)Delegate.Combine(a, callback);
		}
	}

	public void UnloadEffect(EffectType type)
	{
		if (LOG_ACTIVITY)
		{
			Log.Info("FX::UnloadEffect " + type);
		}
		effectPool.Purge(type);
		loadedEffects.Remove(type);
		effectsCurrentlyLoading.Remove(type);
		if (loadedBundles.ContainsKey(type))
		{
			UnloadAssetBundle(type);
		}
	}

	public static void UnloadAll()
	{
		List<EffectType> list = new List<EffectType>();
		foreach (KeyValuePair<EffectType, GameObject> loadedEffect in Singleton<GlobalEffectsManager>.instance.loadedEffects)
		{
			list.Add(loadedEffect.Key);
		}
		foreach (EffectType item in list)
		{
			Singleton<GlobalEffectsManager>.instance.UnloadEffect(item);
		}
		Singleton<GlobalEffectsManager>.instance.effectPool.PurgeAll();
	}

	public void EnsureEffectIsLoaded(EffectType type, Action callback = null)
	{
		if (!loadedEffects.ContainsKey(type))
		{
			LoadEffect(type, callback);
		}
		else if (callback != null)
		{
			callback();
		}
	}

	public void EnsurePoolCapacity(EffectType type, int count)
	{
		EnsureEffectIsLoaded(type, delegate
		{
			while (effectPool.GetCount(type) < count)
			{
				_Return(_CreateNew(type));
			}
		});
	}

	public void AppendPoolCapacity(EffectType type, int count)
	{
		EnsureEffectIsLoaded(type, delegate
		{
			EnsurePoolCapacity(type, effectPool.GetCount(type) + count);
		});
	}

	private void LoadAssetBundle(EffectType type, Action<GameObject> callback = null)
	{
		EffectsDataModel effect = EffectsDataModel.GetByType(type);
		if (LOG_ACTIVITY)
		{
			Log.Info("FX::LoadEffect Load AssetBundle for type " + type);
		}
		Singleton<AssetBundleManager>.instance.GetAssetBundle(effect.AssetLinkage.bundleId, delegate(string err, AssetBundle ab, AssetBundleDataModel m)
		{
			if (LOG_ACTIVITY)
			{
				Log.Info("FX::LoadEffect Completed AssetBundle Load for type " + type);
			}
			loadedBundles[type] = m;
			Singleton<AssetBundleManager>.instance.RetainAssetBundle(m);
			GameObject obj = (GameObject)ab.Load(effect.AssetLinkage.assetName, typeof(GameObject));
			if (callback != null)
			{
				callback(obj);
			}
		});
	}

	private void UnloadAssetBundle(EffectType type)
	{
		Singleton<AssetBundleManager>.instance.ReleaseAssetBundle(loadedBundles[type]);
		loadedBundles.Remove(type);
		if (LOG_ACTIVITY)
		{
			Log.Info("FX::UnloadEffect Unloading AssetBundle " + type);
		}
	}
}
