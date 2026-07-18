using System.Collections;
using UnityEngine;

public class ShopItemView : MonoBehaviour
{
	[SerializeField]
	private tk2dSprite icon;

	[SerializeField]
	private tk2dTextMesh shopItemName;

	[SerializeField]
	private tk2dTextMesh shopItemDescription;

	[SerializeField]
	private PriceLabelController priceLabel;

	[SerializeField]
	private tk2dTextMesh shopItemQuantity;

	[SerializeField]
	private tk2dTextMesh shopItemPrice;

	[SerializeField]
	private StreamingThumbnail shopItemImage;

	[SerializeField]
	private PrefabProxy shopImageProxy;

	public void ConfigureView(ShopItem shopItemData)
	{
		string text = shopItemData.sku;
		if (text.Split('_').Length > 2)
		{
			text = text.Substring(0, text.IndexOf('_') + 2);
		}
		Log.DebugTag("Shop item details: " + shopItemData.ToString() + " item: " + text, null, "Shop");
		shopItemName.text = (text + "_title").Localize();
		shopItemDescription.text = (text + "_desc").Localize();
		shopItemQuantity.text = shopItemData.value.ToString();
		shopItemPrice.text = shopItemData.displayPrice;
		StartCoroutine(SetItemImage(text + ".prefab"));
	}

	private IEnumerator SetItemImage(string assetLinkage)
	{
		yield return StartCoroutine(shopImageProxy.ChangeAssetCoroutine(assetLinkage, 5000));
		while (!shopImageProxy.AssetReady)
		{
			yield return 0;
		}
	}
}
