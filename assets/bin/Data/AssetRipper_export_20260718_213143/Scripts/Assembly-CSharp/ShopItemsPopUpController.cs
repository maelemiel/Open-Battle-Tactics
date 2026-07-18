using System;
using UnityEngine;

public class ShopItemsPopUpController : MonoBehaviour
{
	[SerializeField]
	private ShopItemsPopUpView popUpView;

	private IShopItemMetadata currentShopItem;

	private Action<IShopItemMetadata> confirmAction;

	private Action cancelAction;

	public void ConfigureAndShowPopUp(IShopItemMetadata shopItemMetadata, Action<IShopItemMetadata> confirmAction, Action cancelAction)
	{
		currentShopItem = shopItemMetadata;
		this.confirmAction = confirmAction;
		this.cancelAction = cancelAction;
		popUpView.ConfigurePopUpView(shopItemMetadata);
		base.gameObject.SetActive(true);
	}

	public void HidePopUp()
	{
		base.gameObject.SetActive(false);
	}

	private void ConfirmButtonPressed()
	{
		if (confirmAction != null)
		{
			confirmAction(currentShopItem);
		}
	}

	private void CancelButtonPressed()
	{
		if (cancelAction != null)
		{
			cancelAction();
		}
	}
}
