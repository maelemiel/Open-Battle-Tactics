using System.Collections;
using UnityEngine;

public abstract class BaseProxy<T> : MonoBehaviour where T : class
{
	protected string assetName;

	protected int bundleId = -1;

	protected AssetBundle assetBundle;

	protected AssetBundleDataModel assetBundleModel;

	protected string resourcePath;

	public float startTime;

	public string AssetName
	{
		get
		{
			return assetName;
		}
	}

	protected virtual string BaseResourcePath
	{
		get
		{
			return string.Empty;
		}
	}

	protected virtual void Awake()
	{
	}

	protected virtual void OnDestroy()
	{
	}

	protected virtual IEnumerator UpdateAsset(bool retry)
	{
		bool isDone = false;
		string error = null;
		startTime = Time.realtimeSinceStartup;
		int dotIndex = assetName.LastIndexOf('.');
		if (dotIndex >= 0)
		{
			string trimmedAssetName = assetName.Remove(dotIndex);
			T localAsset = Resources.Load(trimmedAssetName) as T;
			if (localAsset != null)
			{
				yield return StartCoroutine(ProcessAsset(localAsset));
				yield break;
			}
		}
		string text;
		bool flag;
		Singleton<AssetBundleManager>.instance.GetAssetBundle(bundleId, delegate(string err, AssetBundle ab, AssetBundleDataModel m)
		{
			assetBundle = ab;
			assetBundleModel = m;
			text = err;
			flag = true;
		});
		while (!isDone)
		{
			yield return 0;
		}
		if (error != null || assetBundle == null)
		{
			Log.Warning("BaseProxy: {0} could not be downloaded from {1} reason: {2}", assetName, bundleId, error);
			if (retry)
			{
				yield return StartCoroutine(UpdateAsset(false));
			}
			yield break;
		}
		T asset = (T)null;
		if ((resourcePath != null && AppConfig.offlineMode) || AppConfig.useLocalAssetBundles)
		{
			Log.Info("Using local AssetBundle from path: '" + resourcePath + "'");
			asset = Resources.LoadAssetAtPath<GameObject>(resourcePath) as T;
		}
		else if (Time.realtimeSinceStartup - startTime > 0.008f)
		{
			AssetBundleRequest req = assetBundle.LoadAsync(assetName, typeof(T));
			yield return req;
			try
			{
				asset = req.asset as T;
			}
			catch
			{
			}
		}
		else
		{
			asset = assetBundle.Load(assetName, typeof(T)) as T;
		}
		if (asset == null)
		{
			if (retry)
			{
				yield return StartCoroutine(UpdateAsset(false));
				yield break;
			}
			Log.Error("BaseProxy: {0} not found on asset bundle id {1}", assetName, bundleId);
		}
		else
		{
			yield return StartCoroutine(ProcessAsset(asset));
		}
	}

	protected abstract IEnumerator ProcessAsset(T asset);

	public virtual void ResetProxy()
	{
		StopAllCoroutines();
	}
}
