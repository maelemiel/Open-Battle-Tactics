using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrantGiftEffect
{
	public static void Create(MonoBehaviour rootObject, ItemCollectionDataModel package, Vector3 startPoint, Vector3 endPoint, int layer, Transform[] rewardPositions)
	{
		rootObject.StartCoroutine(CreateEffect(rootObject, package.items, rootObject.transform, rewardPositions, layer));
	}

	private static IEnumerator CreateEffect(MonoBehaviour rootObject, List<ItemCollectionDataModel.Item> items, Transform parent, Transform[] locations, int layer)
	{
		PartFoundEffect partsEffect = null;
		CurrencyEffect currencyEffect = null;
		for (int i = 0; i < items.Count; i++)
		{
			Vector3 location = locations[i % locations.Length].position;
			switch (items[i].itemType)
			{
			case UserInventory.ItemType.Parts:
			{
				partsEffect = GlobalEffectsManager.Create(EffectType.PART_DROP, location, parent).SetLayer(layer).GetComponent<PartFoundEffect>();
				partsEffect.rowWidth = 6;
				partsEffect.SortingOrder = 53;
				List<UnitPartTypesDataModel> partsResult = new List<UnitPartTypesDataModel>();
				UnitPartTypesDataModel partDataModel = UnitPartTypesDataModel.GetSingle(items[i].itemId);
				if (partDataModel != null)
				{
					for (int c = 0; c < items[i].amount; c++)
					{
						partsResult.Add(partDataModel);
					}
					if ((bool)partsEffect)
					{
						partsEffect.PlayAnimation(partsResult, null);
					}
					yield return new WaitForEndOfFrame();
				}
				break;
			}
			case UserInventory.ItemType.Energy:
			case UserInventory.ItemType.Coins:
			case UserInventory.ItemType.PremiumCurrency:
				if (items[i].itemType == UserInventory.ItemType.Coins)
				{
					EffectInstance rewardAnimEffect = GlobalEffectsManager.Create(EffectType.REWARD, location).AutoDestroy();
					rewardAnimEffect.gameObject.SetLayerRecursively(rootObject.gameObject.layer);
					rewardAnimEffect.SpineAnimation.Skeleton.SortOrder = 7;
				}
				currencyEffect = CurrencyEffect.Create(items[i].itemType, items[i].amount);
				currencyEffect.gameObject.SetLayerRecursively(layer);
				currencyEffect.transform.SetParent(parent);
				currencyEffect.transform.position = location;
				currencyEffect.SortingOrder = 53;
				yield return new WaitForEndOfFrame();
				break;
			}
		}
		while ((partsEffect != null && partsEffect.animating) || (currencyEffect != null && currencyEffect.animating))
		{
			yield return new WaitForEndOfFrame();
		}
	}
}
