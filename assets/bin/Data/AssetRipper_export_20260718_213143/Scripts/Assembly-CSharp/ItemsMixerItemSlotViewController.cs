using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using UnityEngine;

public class ItemsMixerItemSlotViewController : MonoBehaviour
{
	[SerializeField]
	private UnitProxy unitProxy;

	[SerializeField]
	private GameObject itemLabelGameObject;

	private ItemCollectionDataModel.Item itemDataModel;

	private Vector3 unitProxyInitialScale;

	private Vector3 priceLabelInitialScale;

	public float unitProxyScaleTime = 0.7f;

	public float unitProxyRepositionTime = 1f;

	public float priceLabelScaleTime = 1f;

	public Vector3 unitExplosionEffectScale = new Vector3(3f, 3f, 3f);

	public bool isSpecial;

	private PriceLabelController itemLabel;

	private bool initialized;

	private UnitDataModel unitDataModel;

	private tk2dUIItem uiItem;

	private void Awake()
	{
		if ((bool)itemLabelGameObject)
		{
			itemLabel = itemLabelGameObject.GetComponent<PriceLabelController>();
		}
		else
		{
			Log.Error("[ItemsMixerSlotViewController] ItemLabelGameObject not found");
		}
		uiItem = GetComponent<tk2dUIItem>();
		if ((bool)uiItem)
		{
			uiItem.enabled = false;
		}
	}

	public IEnumerator Init(ItemCollectionDataModel.Item item)
	{
		if ((bool)unitProxy)
		{
			itemDataModel = item;
			unitProxyInitialScale = unitProxy.gameObject.transform.localScale;
			unitProxy.gameObject.transform.localScale = Vector3.zero;
			priceLabelInitialScale = itemLabelGameObject.transform.localScale;
			unitDataModel = GetUnitDataModelByItemId(item.itemId.ToString());
			unitProxy.ChangeAsset("Prefab.prefab", unitDataModel.Levels[0].assetBundleId);
			while (!unitProxy.AssetReady)
			{
				yield return 0;
			}
			initialized = true;
		}
	}

	public IEnumerator ConfigureView(ItemCollectionDataModel.Item item)
	{
		if (!initialized)
		{
			Log.Error("ItemsMixerSlotViewController not initialized");
			yield break;
		}
		if ((bool)unitProxy && unitProxy.AssetReady)
		{
			unitProxy.transform.TweenLocalScale(unitProxyInitialScale.x, unitProxyScaleTime, EaseType.EaseOutBack);
			EffectInstance explosionEffect = GlobalEffectsManager.Create((!isSpecial) ? EffectType.DICE_EFFECT : EffectType.SPECIAL_ROLL_EFFECT, unitProxy.transform.position, base.gameObject);
			explosionEffect.transform.localScale = unitExplosionEffectScale;
			explosionEffect.AutoDestroy();
			if (isSpecial)
			{
				AudioTrigger.SpecialResult.Play();
			}
			else
			{
				AudioTrigger.HighDamageResult.Play();
			}
			yield return new WaitForSeconds(unitProxyScaleTime);
			if ((bool)itemLabelGameObject)
			{
				unitProxy.transform.TweenPosition(itemLabelGameObject.transform.position, unitProxyRepositionTime);
			}
			unitProxy.transform.TweenLocalScale(0f, unitProxyRepositionTime);
		}
		if ((bool)itemLabel)
		{
			itemLabel.ConfigurePriceLabel(new ItemCollectionDataModel(item));
			itemLabelGameObject.transform.localScale = Vector3.zero;
			itemLabelGameObject.transform.TweenLocalScale(priceLabelInitialScale.x, priceLabelScaleTime);
			EffectInstance rewardEffect = GlobalEffectsManager.Create(EffectType.DICE_EFFECT_FIRST_STRIKE, itemLabel.transform.position, base.gameObject);
			rewardEffect.AutoDestroy();
			AudioTrigger.HighFirstStrikeResult.Play();
		}
		if ((bool)uiItem)
		{
			uiItem.enabled = true;
		}
	}

	private UnitDataModel GetUnitDataModelByItemId(string partId)
	{
		if (string.IsNullOrEmpty(partId))
		{
			return null;
		}
		if (itemDataModel.itemType == UserInventory.ItemType.Parts)
		{
			List<UnitPartsDataModel> multiByQuery = NonUnitySingleton<DMAccessManager>.instance.GetMultiByQuery<UnitPartsDataModel>(" WHERE part_type = " + partId);
			if (multiByQuery.Count > 0 && multiByQuery[0] != null)
			{
				return UnitDataModel.GetSingle(multiByQuery[0].unitId);
			}
		}
		return null;
	}

	public void ShowUnitPopUp()
	{
		if (unitDataModel != null)
		{
			PopupManager.ShowPopup(PopupDataModel.InspectUnitPopUp(unitDataModel, null));
		}
	}
}
