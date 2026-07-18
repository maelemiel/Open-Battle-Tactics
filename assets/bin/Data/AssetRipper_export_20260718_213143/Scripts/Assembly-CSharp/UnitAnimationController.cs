using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimationController : MonoBehaviour
{
	private const string MOVEMENT_EFFECT_ATTACH_POINT_NAME = "dust";

	private const string GROUND_ATTACH_POINT_NAME = "ground";

	public UnitProxy unitProxy;

	private EffectType movementEffectType = EffectType.DUST;

	public Vector3 movementEffectOffset = Vector3.zero;

	public GameObject shadow;

	public Vector3 shadowOffset = new Vector3(0f, 9f, 0f);

	[SerializeField]
	private List<UnitAnimationLayer> animationLayers;

	private tk2dSprite tankSprite;

	private EffectInstance movementEffect;

	private tk2dBaseSprite shadowSprite;

	private Vector3 originalShadowScale;

	public bool showDust = true;

	public GameObject testPrefab;

	private void Awake()
	{
		if ((bool)shadow)
		{
			shadowSprite = shadow.GetComponent<tk2dBaseSprite>();
			originalShadowScale = shadowSprite.scale;
		}
	}

	private void OnEnable()
	{
		StartCoroutine(SetupAnimation());
	}

	public IEnumerator SetupAnimation()
	{
		if (!unitProxy)
		{
			yield break;
		}
		if ((bool)testPrefab)
		{
			tankSprite = testPrefab.GetComponent<tk2dSprite>();
		}
		else
		{
			while (!unitProxy.AssetReady)
			{
				yield return null;
			}
			tankSprite = unitProxy.Prefab.GetComponent<tk2dSprite>();
		}
		if (!tankSprite)
		{
			Log.Error("Tank sprite not found", base.gameObject);
			yield break;
		}
		base.gameObject.SetLayerRecursively(tankSprite.gameObject.layer);
		if (tankSprite.GetAttachPointByName("helicopter") != null)
		{
			movementEffectType = EffectType.DUST_AIR;
		}
		SetupWheelsAnimation();
		SetupShadow();
		SetupMovementEffect();
		SetupSpriteTween();
		UpdateAnimationColour();
		unitProxy.OnProxyChanged += UpdateAnimation;
	}

	private void UpdateAnimation()
	{
		unitProxy.OnProxyChanged -= UpdateAnimation;
		DeactivateWheelsAnimation();
		if ((bool)movementEffect)
		{
			movementEffect.gameObject.SetActive(false);
		}
		if (base.gameObject.activeSelf)
		{
			StartCoroutine(SetupAnimation());
		}
	}

	private void UpdateAnimationColour()
	{
		for (int i = 0; i < animationLayers.Count && !(animationLayers[i].color == unitProxy.objectColor); i++)
		{
			animationLayers[i].color = unitProxy.objectColor;
		}
	}

	private void SetupMovementEffect()
	{
		if ((bool)tankSprite && movementEffectType != EffectType.NONE && showDust)
		{
			if (movementEffect == null)
			{
				movementEffect = GlobalEffectsManager.Create(movementEffectType, Vector3.zero, unitProxy.transform);
			}
			else
			{
				movementEffect.gameObject.SetActive(true);
			}
			tk2dSpineAnimation tk2dSpineAnimation2 = movementEffect.GetComponent<tk2dSpineAnimation>();
			if (!tk2dSpineAnimation2)
			{
				tk2dSpineAnimation2 = movementEffect.GetComponentInChildren<tk2dSpineAnimation>();
			}
			if ((bool)tk2dSpineAnimation2)
			{
				tk2dSpineAnimation2.Skeleton.SortOrder = tankSprite.SortingOrder - 1;
			}
			tk2dSpriteDefinition.AttachPoint attachPointByName = tankSprite.GetAttachPointByName("dust");
			if (attachPointByName != null)
			{
				movementEffect.gameObject.SetActive(true);
				movementEffect.transform.localPosition = new Vector3(attachPointByName.position.x * Mathf.Sign(unitProxy.transform.localScale.x), attachPointByName.position.y, attachPointByName.position.z) + movementEffectOffset;
			}
			else
			{
				movementEffect.gameObject.SetActive(false);
			}
		}
	}

	private void SetupShadow()
	{
		if ((bool)shadowSprite && (bool)tankSprite)
		{
			tk2dSpriteDefinition.AttachPoint attachPointByName = tankSprite.GetAttachPointByName("ground");
			float x = tankSprite.GetBounds().extents.x;
			shadowSprite.scale = originalShadowScale * x * 0.01f;
			shadowSprite.SortingOrder = tankSprite.SortingOrder - 1;
			if (attachPointByName != null)
			{
				shadow.transform.localPosition = new Vector3(0f, attachPointByName.position.y, attachPointByName.position.z) + shadowOffset;
			}
		}
	}

	private void SetupWheelsAnimation()
	{
		string baseSpriteName = tankSprite.GetCurrentSpriteDef().name;
		foreach (UnitAnimationLayer animationLayer in animationLayers)
		{
			animationLayer.Activate(tankSprite.Collection, baseSpriteName);
			animationLayer.SetSortingOrder(tankSprite.SortingOrder + 1);
			if (animationLayer.shouldBounce)
			{
				animationLayer.bounceTarget = tankSprite.transform;
			}
			if (animationLayer.hideOutsideBattle)
			{
				animationLayer.gameObject.SetActive(false);
			}
		}
	}

	private void DeactivateWheelsAnimation()
	{
		foreach (UnitAnimationLayer animationLayer in animationLayers)
		{
			animationLayer.gameObject.SetActive(false);
		}
	}

	private void SetupSpriteTween()
	{
		if ((bool)tankSprite)
		{
			tankSprite.gameObject.AddComponent<UnitSpriteTween>();
		}
	}
}
