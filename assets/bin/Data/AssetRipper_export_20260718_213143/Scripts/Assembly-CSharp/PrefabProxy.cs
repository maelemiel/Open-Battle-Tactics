using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PrefabInflator))]
public class PrefabProxy : BaseProxy<GameObject>
{
	protected PrefabInflator inflator;

	protected GameObject prefab;

	protected Coroutine updateOperation;

	protected bool assetReady;

	public int SortingOrder;

	public bool centerAtZero;

	public GameObject defaultObject;

	private GameObject defaultObjectInstance;

	public Color objectColor = Color.white;

	protected AssetBundleDataModel currentAssetBundleModel;

	public bool AssetReady
	{
		get
		{
			return assetReady;
		}
	}

	public GameObject Prefab
	{
		get
		{
			return prefab;
		}
	}

	protected override string BaseResourcePath
	{
		get
		{
			return "Assets/Bundles/Assets/{0}/{1}";
		}
	}

	public event Action OnProxyChanged;

	protected override void Awake()
	{
		inflator = GetComponent<PrefabInflator>();
	}

	protected override void OnDestroy()
	{
		DestroyPrefabCoroutine();
		base.OnDestroy();
	}

	public void ChangeAsset(AssetLinkageDataModel assetLinkage)
	{
		if (assetLinkage == null)
		{
			Log.Error("Trying to load a null AssetLinkage");
			return;
		}
		StopAllCoroutines();
		StartCoroutine(_ChangeAssetCoroutine(assetLinkage.assetName, assetLinkage.bundleId));
	}

	public IEnumerator ChangeAssetCoroutine(AssetLinkageDataModel assetLinkage)
	{
		if (assetLinkage == null)
		{
			Log.Error("Trying to load a null AssetLinkage");
		}
		else
		{
			yield return StartCoroutine(_ChangeAssetCoroutine(assetLinkage.assetName, assetLinkage.bundleId));
		}
	}

	private IEnumerator _ChangeAssetCoroutine(AssetLinkageDataModel assetLinkage)
	{
		if (assetLinkage == null)
		{
			Log.Error("Trying to load a null AssetLinkage");
		}
		else
		{
			yield return StartCoroutine(ChangeAssetCoroutine(assetLinkage.assetName, assetLinkage.bundleId));
		}
	}

	public virtual IEnumerator ChangeAssetCoroutine(string assetName, int bundleId)
	{
		StopAllCoroutines();
		yield return StartCoroutine(_ChangeAssetCoroutine(assetName, bundleId));
	}

	public virtual void ChangeAsset(string assetName, int bundleId)
	{
		StopAllCoroutines();
		StartCoroutine(_ChangeAssetCoroutine(assetName, bundleId));
	}

	public virtual IEnumerator _ChangeAssetCoroutine(string assetName, int bundleId)
	{
		resourcePath = string.Format(BaseResourcePath, bundleId, assetName);
		assetReady = false;
		if (this.OnProxyChanged != null)
		{
			this.OnProxyChanged();
		}
		SetDefaultObject();
		base.assetName = assetName;
		base.bundleId = bundleId;
		DestroyPrefabCoroutine();
		int dotIndex = assetName.LastIndexOf('.');
		if (dotIndex >= 0)
		{
			string trimmedAssetName = assetName.Remove(dotIndex);
			GameObject localAsset = Resources.Load(trimmedAssetName) as GameObject;
			if (localAsset != null)
			{
				yield return StartCoroutine(ProcessAsset(localAsset));
				yield break;
			}
		}
		if (!string.IsNullOrEmpty(assetName) && bundleId != -1)
		{
			yield return StartCoroutine(UpdateAsset(true));
		}
	}

	protected override IEnumerator ProcessAsset(GameObject p)
	{
		DestroyPrefabCoroutine();
		if (defaultObjectInstance != null)
		{
			UnityEngine.Object.Destroy(defaultObjectInstance);
		}
		currentAssetBundleModel = assetBundleModel;
		Singleton<AssetBundleManager>.instance.RetainAssetBundle(currentAssetBundleModel);
		prefab = UnityEngine.Object.Instantiate(p) as GameObject;
		inflator.Inflate(prefab);
		base.transform.MakeChild(prefab.transform);
		prefab.SetLayerRecursively(base.transform.gameObject.layer);
		if (centerAtZero)
		{
			prefab.transform.localPosition = Vector3.zero;
		}
		assetReady = true;
		ChangeSortingOrder(SortingOrder);
		if (objectColor != Color.white)
		{
			base.gameObject.SetColor(objectColor);
		}
		yield break;
	}

	public void ChangeLayer(int layerIndex)
	{
		StartCoroutine(ChangeLayerCoroutine(layerIndex));
	}

	private IEnumerator ChangeLayerCoroutine(int layerIndex)
	{
		while (!assetReady)
		{
			yield return 0;
		}
		base.gameObject.SetLayerRecursively(layerIndex);
	}

	public void ChangeSortingOrder(int order)
	{
		StartCoroutine(_ChangeSortingOrder(order));
	}

	private IEnumerator _ChangeSortingOrder(int order)
	{
		while (!assetReady)
		{
			yield return 0;
		}
		base.gameObject.SetSortingOrder(order);
	}

	public void FitWithinBounds(Bounds bounds)
	{
		StartCoroutine(_FitWithinBounds(bounds));
	}

	private IEnumerator _FitWithinBounds(Bounds bounds)
	{
		while (!assetReady)
		{
			yield return 0;
		}
		base.gameObject.FitWithinBounds(bounds);
	}

	public virtual IEnumerator WaitForAssetReady()
	{
		while (!assetReady)
		{
			yield return 0;
		}
	}

	protected virtual void DestroyPrefabCoroutine()
	{
		if (prefab != null)
		{
			UnityEngine.Object.Destroy(Prefab);
		}
		if (currentAssetBundleModel != null)
		{
			Singleton<AssetBundleManager>.instance.ReleaseAssetBundle(currentAssetBundleModel);
			currentAssetBundleModel = null;
		}
		prefab = null;
	}

	public override void ResetProxy()
	{
		base.ResetProxy();
		DestroyPrefabCoroutine();
	}

	public void SetDefaultObject()
	{
		if (defaultObjectInstance != null)
		{
			UnityEngine.Object.Destroy(defaultObjectInstance);
		}
		if ((bool)defaultObject)
		{
			defaultObjectInstance = UnityEngine.Object.Instantiate(defaultObject) as GameObject;
			defaultObjectInstance.SetLayerRecursively(base.gameObject.layer);
			defaultObjectInstance.SetSortingOrder(SortingOrder);
			defaultObjectInstance.transform.parent = base.transform;
			defaultObjectInstance.transform.localPosition = Vector3.zero;
			defaultObjectInstance.transform.localScale = Vector3.one;
		}
	}

	public static PrefabProxy CreateFromAssetLinkage(AssetLinkageDataModel linkage, Action finishCallback = null)
	{
		GameObject gameObject = new GameObject();
		PrefabProxy prefabProxy = gameObject.AddComponent<PrefabProxy>();
		if (finishCallback != null)
		{
			SingletonManager.ExecuteAfterCoroutine(prefabProxy.ChangeAssetCoroutine(linkage), finishCallback);
		}
		else
		{
			prefabProxy.ChangeAsset(linkage);
		}
		return prefabProxy;
	}

	public static PrefabProxy CreateFromBundle(int bundleId, Action finishCallback = null)
	{
		GameObject gameObject = new GameObject();
		PrefabProxy prefabProxy = gameObject.AddComponent<PrefabProxy>();
		if (finishCallback != null)
		{
			SingletonManager.ExecuteAfterCoroutine(prefabProxy.ChangeAssetCoroutine("Prefab.prefab", bundleId), finishCallback);
		}
		else
		{
			prefabProxy.ChangeAsset("Prefab.prefab", bundleId);
		}
		return prefabProxy;
	}
}
