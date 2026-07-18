using System.Collections.Generic;
using UnityEngine;

public class RewardsAssigner : MonoBehaviour
{
	[SerializeField]
	private Vector2 offset = Vector2.zero;

	[SerializeField]
	private bool center;

	[SerializeField]
	private Vector3 labelScale = Vector3.one;

	[SerializeField]
	private MultipleTankRewards multiUnitPrefab;

	[SerializeField]
	private MultiplePartRewards multiPartsPrefab;

	private ItemCollectionDataModel itemCollectionModel;

	public bool showItemsAtZero;

	public int rowWidth = 1;

	private List<RewardLabelItemView> priceItems = new List<RewardLabelItemView>();

	public int SortingOrder = 1;

	public bool showAvailable;

	public bool hideIfCanAfford;

	public bool forceSortingOrder;

	private GameObject tempParent;

	public int ItemsCount
	{
		get
		{
			return priceItems.Count;
		}
	}

	public RewardLabelItemView GetPriceItem(UserInventory.ItemType itemtype, string id)
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

	public RewardLabelItemView GetPriceItem(int index)
	{
		if (index >= 0 && index < priceItems.Count)
		{
			return priceItems[index];
		}
		return null;
	}

	public void ConfigureRewardDisplay(ItemCollectionDataModel itemCollectionModelOrig)
	{
		itemCollectionModel = ItemCollectionDataModel.CopyCollectionDataModel(itemCollectionModelOrig);
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
		int num = 0;
		int num2 = 0;
		int num3 = Mathf.CeilToInt((float)itemCollectionModel.items.Count / (float)rowWidth);
		if (itemCollectionModel.items.Count <= 0)
		{
			return;
		}
		RewardLabelItemView rewardLabelItemView = null;
		switch (itemCollectionModel.items[0].itemType)
		{
		case UserInventory.ItemType.Unit:
			rewardLabelItemView = (Object.Instantiate(multiUnitPrefab.gameObject) as GameObject).GetComponent<MultipleTankRewards>();
			break;
		case UserInventory.ItemType.Energy:
		case UserInventory.ItemType.Coins:
		case UserInventory.ItemType.PremiumCurrency:
		case UserInventory.ItemType.Parts:
			rewardLabelItemView = (Object.Instantiate(multiPartsPrefab.gameObject) as GameObject).GetComponent<MultiplePartRewards>();
			break;
		default:
			Log.Error("## Rewards Assigner: Unsupported ItemType! " + itemCollectionModel.items[0].itemType);
			break;
		}
		if (rewardLabelItemView != null)
		{
			if (forceSortingOrder)
			{
				rewardLabelItemView.gameObject.SetSortingOrder(SortingOrder);
			}
			rewardLabelItemView.gameObject.SetLayerRecursively(base.gameObject.layer);
			rewardLabelItemView.transform.parent = tempParent.transform;
			rewardLabelItemView.transform.localScale = labelScale;
			rewardLabelItemView.SetupAllItems(itemCollectionModel, itemCollectionModel.items[0]);
			rewardLabelItemView.gameObject.SetLayerRecursively(base.gameObject.layer);
			priceItems.Add(rewardLabelItemView);
			rewardLabelItemView.transform.localPosition = Vector3.zero;
			if (num3 < rowWidth)
			{
				float y = offset.y * 0.5f;
				rewardLabelItemView.transform.localPosition += new Vector3(0f, y, 0f);
			}
		}
	}
}
