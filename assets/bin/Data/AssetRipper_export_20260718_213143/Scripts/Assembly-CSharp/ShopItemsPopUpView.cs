using UnityEngine;

public class ShopItemsPopUpView : MonoBehaviour
{
	[SerializeField]
	private tk2dSprite icon;

	[SerializeField]
	private tk2dTextMesh nameLabel;

	[SerializeField]
	private tk2dTextMesh descriptionLabel;

	[SerializeField]
	private PriceLabelController priceLabel;

	public void ConfigurePopUpView(IShopItemMetadata shopItemMetadata)
	{
		nameLabel.text = shopItemMetadata.Name;
		descriptionLabel.text = shopItemMetadata.Description;
		priceLabel.ConfigurePriceLabel(shopItemMetadata.Cost.GetPrice());
		icon.SetSprite(shopItemMetadata.IconName);
	}
}
