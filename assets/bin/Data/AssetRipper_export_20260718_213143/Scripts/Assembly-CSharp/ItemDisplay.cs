using UnityEngine;

public class ItemDisplay : MonoBehaviour
{
	public tk2dTextMesh itemCount;

	public PrefabProxy prefabProxy;

	public tk2dSprite displaySprite;

	public int itemID;

	private void Awake()
	{
		if (itemCount == null)
		{
			itemCount = GetComponentInChildren<tk2dTextMesh>();
		}
		if (prefabProxy == null)
		{
			prefabProxy = GetComponentInChildren<UnitProxy>();
		}
	}

	public void SetupItem(ItemCollectionDataModel.Item itemData, int count)
	{
		if ((bool)itemCount)
		{
			itemCount.text = count.ToString("N0");
		}
		displaySprite.gameObject.SetActive(false);
		switch (itemData.itemType)
		{
		case UserInventory.ItemType.Parts:
			if ((bool)prefabProxy)
			{
				StartCoroutine(prefabProxy.ChangeAssetCoroutine(itemData.Part.AssetLinkage));
			}
			break;
		case UserInventory.ItemType.Energy:
		case UserInventory.ItemType.Coins:
		case UserInventory.ItemType.PremiumCurrency:
			displaySprite.gameObject.SetActive(true);
			displaySprite.SetSprite(itemData.itemType.GetIconName());
			break;
		case (UserInventory.ItemType)3:
			break;
		}
	}
}
