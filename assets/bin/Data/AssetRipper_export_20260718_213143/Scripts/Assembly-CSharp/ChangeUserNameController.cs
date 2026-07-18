using System;
using UnityEngine;

public class ChangeUserNameController : MonoBehaviour
{
	private Action<ServerUtilities.BaseResponse> parentCallback;

	private Action parentCancelCallback;

	public void ChangeUserName(string userName, Action<ServerUtilities.BaseResponse> parentCallback, Action parentCancelCallback)
	{
		this.parentCallback = parentCallback;
		this.parentCancelCallback = parentCancelCallback;
		ChangeUserNameRequest(userName);
	}

	private void ChangeUserNameRequest(string userName)
	{
		UserPriceDataModel priceForID = ItemPriceDataModel.GetPriceForID(Constants.ChangeNamePriceId);
		if (UserProfile.player.CanAfford(priceForID))
		{
			ChangeUserNameTransaction(userName, UserPriceDataModel.PaymentType.Normal);
		}
		else if (priceForID.HasItemOfType(UserInventory.ItemType.Coins))
		{
			UserPriceDataModel premiumPrice = UserPriceDataModel.GetPremiumPrice(priceForID, UserProfile.player, Constants.ChangeNameGemConversion);
			PopupManager.ShowPopup(PopupDataModel.PriceConfirmationPopUp(premiumPrice, "ui_not_enough_cash_title".Localize("Not enough cash"), "ui_not_enough_cash_desc".Localize("Pay with gems?"), delegate
			{
				if (UserProfile.player.CanAfford(premiumPrice))
				{
					ChangeUserNameTransaction(userName, UserPriceDataModel.PaymentType.UsePremiumForDifference);
					Reporting.GemsToCashPopupEvent(PurchaseSource.SkinPurchase, PurchaseAction.purchase, premiumPrice);
				}
				else
				{
					NotEnoughGems();
				}
			}, delegate
			{
				Reporting.GemsToCashPopupEvent(PurchaseSource.PickNamePayment, PurchaseAction.cancelled, premiumPrice);
			}, delegate
			{
				Reporting.GemsToCashPopupEvent(PurchaseSource.PickNamePayment, PurchaseAction.cancelled, premiumPrice);
			}));
		}
		else
		{
			NotEnoughGems();
		}
	}

	private void NotEnoughGems()
	{
		PopupManager.ShowPopup(PopupDataModel.NoYes("ui_not_enough_gems_title".Localize("Not enough gems"), "ui_not_enough_gems_desc".Localize("Do you want to go buy more gems?"), delegate
		{
			PopupManager.DestroyAllPopups();
			TopBarController.instance.LoadShop();
		}, parentCancelCallback, parentCancelCallback));
	}

	private void ChangeUserNameTransaction(string userName, UserPriceDataModel.PaymentType type)
	{
		Singleton<SessionManager>.instance.ChangeUserName(userName, type, delegate(ServerUtilities.BaseResponse response)
		{
			if (parentCallback != null)
			{
				parentCallback(response);
			}
		});
	}
}
