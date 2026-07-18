using System.Collections.Generic;
using UnityEngine;

public class PriceLabelController : MonoBehaviour
{
	[SerializeField]
	private EnergyItemView energyLabelPrefab;

	[SerializeField]
	private UnitLabelItemView unitLabelPrefab;

	[SerializeField]
	private PartsLabelItemView partsLabelPrefab;

	[SerializeField]
	private CurrencyLabelItemView currencyLabelPrefab;

	[SerializeField]
	private Vector2 offset = Vector2.zero;

	[SerializeField]
	private bool center;

	[SerializeField]
	private Vector3 labelScale = Vector3.one;

	public bool showItemsAtZero;

	public int rowWidth = 1;

	private List<PriceLabelItemView> priceItems = new List<PriceLabelItemView>();

	public int SortingOrder = 1;

	public bool showAvailable;

	public bool hideIfCanAfford;

	public bool setSortingOrder = true;

	private GameObject tempParent;

	public int ItemsCount
	{
		get
		{
			return priceItems.Count;
		}
	}

	public PriceLabelItemView GetPriceItem(UserInventory.ItemType itemtype, string id)
	{
		for (int i = 0; i < priceItems.Count; i++)
		{
			if (priceItems[i].item.itemType == itemtype && priceItems[i].item.itemId == int.Parse(id))
			{
				return priceItems[i];
			}
		}
		return null;
	}

	public PriceLabelItemView GetPriceItem(int index)
	{
		if (index >= 0 && index < priceItems.Count)
		{
			return priceItems[index];
		}
		return null;
	}

	public void ConfigurePriceLabel(ItemCollectionDataModel priceDataModel)
	{
		if ((bool)tempParent)
		{
			Object.Destroy(tempParent);
		}
		priceItems.Clear();
		tempParent = new GameObject();
		tempParent.transform.parent = base.transform;
		tempParent.transform.position = base.transform.position;
		tempParent.transform.rotation = base.transform.rotation;
		tempParent.transform.localScale = base.transform.localScale;
		if (hideIfCanAfford && UserProfile.player.CanAfford(priceDataModel.items))
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		int num3 = Mathf.CeilToInt((float)priceDataModel.items.Count / (float)rowWidth);
		for (int i = 0; i < priceDataModel.items.Count; i++)
		{
			num = Mathf.FloorToInt(i / rowWidth);
			num2 = i % rowWidth;
			PriceLabelItemView priceLabelItemView = null;
			switch (priceDataModel.items[i].itemType)
			{
			case UserInventory.ItemType.Parts:
				if (priceDataModel.items[i].amount <= 0 && !showItemsAtZero)
				{
					continue;
				}
				priceLabelItemView = (Object.Instantiate(partsLabelPrefab.gameObject) as GameObject).GetComponent<PartsLabelItemView>();
				break;
			case UserInventory.ItemType.Unit:
				priceLabelItemView = (Object.Instantiate(unitLabelPrefab.gameObject) as GameObject).GetComponent<UnitLabelItemView>();
				break;
			case UserInventory.ItemType.Energy:
				if (priceDataModel.items[i].amount <= 0 && !showItemsAtZero)
				{
					continue;
				}
				priceLabelItemView = (Object.Instantiate(energyLabelPrefab.gameObject) as GameObject).GetComponent<EnergyItemView>();
				break;
			default:
				priceLabelItemView = (Object.Instantiate(currencyLabelPrefab.gameObject) as GameObject).GetComponent<CurrencyLabelItemView>();
				break;
			}
			if (setSortingOrder)
			{
				priceLabelItemView.gameObject.SetSortingOrder(SortingOrder);
			}
			priceLabelItemView.gameObject.SetLayerRecursively(base.gameObject.layer);
			PrefabProxy componentInChildren = priceLabelItemView.GetComponentInChildren<PrefabProxy>();
			if ((bool)componentInChildren)
			{
				componentInChildren.SortingOrder = SortingOrder;
			}
			if (priceLabelItemView != null)
			{
				priceLabelItemView.transform.parent = tempParent.transform;
				priceLabelItemView.transform.localScale = labelScale;
				priceLabelItemView.SetupPriceItem(priceDataModel.items[i], showAvailable);
				priceLabelItemView.gameObject.SetLayerRecursively(base.gameObject.layer);
				priceItems.Add(priceLabelItemView);
				priceLabelItemView.transform.localPosition = new Vector2(offset.x * (float)num2, offset.y * (float)num);
				if (num3 < rowWidth)
				{
					float y = offset.y * 0.5f;
					priceLabelItemView.transform.localPosition += new Vector3(0f, y, 0f);
				}
				if (i > 0 && i < rowWidth && center)
				{
					Vector2 vector = new Vector2(offset.x, 0f);
					tempParent.transform.Translate(vector * -0.5f);
				}
			}
		}
	}
}
