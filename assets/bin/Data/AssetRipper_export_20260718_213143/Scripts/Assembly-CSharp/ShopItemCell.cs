using UnityEngine;

public class ShopItemCell : ScrollableCell
{
	[SerializeField]
	private ShopItemController shopItemController;

	public override void ConfigureCell()
	{
		if (dataObject != null)
		{
			ConfigureCellData();
		}
	}

	public override void ConfigureCellData()
	{
		if (dataObject != null)
		{
			ShopItem shopItemData = (ShopItem)dataObject;
			shopItemController.Init(shopItemData);
		}
	}
}
