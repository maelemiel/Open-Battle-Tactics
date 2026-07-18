using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PrefabInflator))]
public class UnitProxy : PrefabProxy
{
	public Vector3 initPosition;

	public Quaternion initRotation;

	private bool hasPositionAndRotation;

	private GameObject objectToSet;

	public bool hasToSetPrefab = true;

	public bool isStatic;

	public List<GameObject> staticPool = new List<GameObject>();

	public bool DownloadedAsset
	{
		get
		{
			return objectToSet != null;
		}
	}

	protected override string BaseResourcePath
	{
		get
		{
			return "Assets/Bundles/Units/{0}/{1}";
		}
	}

	protected override void Awake()
	{
		base.Awake();
	}

	public IEnumerator ChangeAssetCoroutine(string assetName, int bundleId, Vector3 position, Quaternion rot)
	{
		initPosition = position;
		initRotation = rot;
		hasPositionAndRotation = true;
		StartCoroutine(ChangeAssetCoroutine(assetName, bundleId));
		yield return StartCoroutine(WaitForAssetReady());
	}

	public override IEnumerator ChangeAssetCoroutine(string assetName, int bundleId)
	{
		yield return StartCoroutine(base.ChangeAssetCoroutine(assetName, bundleId));
		while (!assetReady)
		{
			yield return 0;
		}
	}

	public IEnumerator ChangeAssetCoroutine(int bundleId, bool setLayerRecursively = true)
	{
		yield return StartCoroutine(ChangeAssetCoroutine("Prefab.prefab", bundleId));
		if (setLayerRecursively)
		{
			base.Prefab.SetLayerRecursively(base.gameObject.layer);
		}
	}

	protected override IEnumerator ProcessAsset(GameObject p)
	{
		objectToSet = p;
		if (hasToSetPrefab)
		{
			SetPrefab();
		}
		yield break;
	}

	public void SetPrefab()
	{
		InstantiatePrefab();
	}

	private void InstantiatePrefab()
	{
		base.DestroyPrefabCoroutine();
		if (hasPositionAndRotation)
		{
			prefab = Object.Instantiate(objectToSet, initPosition, initRotation) as GameObject;
			inflator.Inflate(prefab);
			prefab.transform.parent = base.transform;
			if (isStatic)
			{
				CreateStaticSprites();
			}
		}
		else
		{
			StartCoroutine(base.ProcessAsset(objectToSet));
			if (isStatic)
			{
				StartCoroutine(WaitForStatic());
			}
		}
	}

	public override void ResetProxy()
	{
		base.ResetProxy();
		staticPool.Clear();
	}

	private IEnumerator WaitForStatic()
	{
		while (!assetReady)
		{
			yield return new WaitForEndOfFrame();
		}
		CreateStaticSprites();
	}

	private void CreateStaticSprites()
	{
		tk2dSprite component = prefab.GetComponent<tk2dSprite>();
		tk2dSpriteCollectionData collection = component.Collection;
		for (int i = 0; i < collection.spriteDefinitions.Length; i++)
		{
			tk2dSpriteDefinition tk2dSpriteDefinition2 = collection.spriteDefinitions[i];
			if (tk2dSpriteDefinition2 != null)
			{
				CheckAttachPoints(collection, component, tk2dSpriteDefinition2, i);
			}
		}
	}

	private void CheckAttachPoints(tk2dSpriteCollectionData spriteCollection, tk2dSprite tankSprite, tk2dSpriteDefinition spriteDef, int spriteId)
	{
		tk2dSpriteDefinition.AttachPoint[] attachPoints = spriteDef.attachPoints;
		foreach (tk2dSpriteDefinition.AttachPoint attachPoint in attachPoints)
		{
			if (attachPoint.name == "static")
			{
				CreateNewSprite(spriteCollection, tankSprite, spriteId);
			}
		}
	}

	private void CreateNewSprite(tk2dSpriteCollectionData spriteCollection, tk2dSprite tankSprite, int spriteId)
	{
		GameObject gameObject = new GameObject();
		gameObject.name = "StaticSprite";
		gameObject.transform.parent = prefab.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localScale = Vector3.one;
		gameObject.SetLayerRecursively(prefab.layer);
		tk2dSprite tk2dSprite2 = gameObject.AddComponent<tk2dSprite>();
		tk2dSprite2.SetSprite(spriteCollection, spriteId);
		tk2dSprite2.color = tankSprite.color;
		tk2dSprite2.SortingOrder = tankSprite.SortingOrder + 1;
		staticPool.Add(gameObject);
	}
}
