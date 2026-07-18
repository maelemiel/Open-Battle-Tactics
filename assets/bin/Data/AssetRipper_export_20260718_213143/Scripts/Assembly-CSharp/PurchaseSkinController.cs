using System;
using UnityEngine;

public class PurchaseSkinController : MonoBehaviour
{
	private Action parentCallback;

	private Action parentCancelCallback;

	public void PurchaseSkin(UnitLevelProgressionDataModel skin, Action parentCallback, Action parentCancelCallback)
	{
		this.parentCallback = parentCallback;
		this.parentCancelCallback = parentCancelCallback;
		PurchaseSkinRequest(skin);
	}

	private void PurchaseSkinRequest(UnitLevelProgressionDataModel skin)
	{
		UserPriceDataModel priceForID = ItemPriceDataModel.GetPriceForID(skin.priceId);
		if (priceForID.HasItemOfType(UserInventory.ItemType.PremiumCurrency))
		{
			PopupManager.ShowPopup(PopupDataModel.CancelOk("ui_purchase_skin_gems_title".Localize("Purchase Skin?"), string.Format("ui_purchase_skin_gems_desc".Localize("Purchase skin for {0} gems?"), priceForID.items[0].amount), delegate
			{
				PurchaseSkin(skin);
			}, delegate
			{
				if (parentCancelCallback != null)
				{
					parentCancelCallback();
				}
			}, delegate
			{
				if (parentCancelCallback != null)
				{
					parentCancelCallback();
				}
			}));
			return;
		}
		PopupManager.ShowPopup(PopupDataModel.CancelOk("ui_purchase_skin_cash_title".Localize("Purchase Skin?"), string.Format("ui_purchase_skin_cash_desc".Localize("Purchase skin for {0} cash?"), priceForID.items[0].amount), delegate
		{
			PurchaseSkin(skin);
		}, delegate
		{
			if (parentCancelCallback != null)
			{
				parentCancelCallback();
			}
		}, delegate
		{
			if (parentCancelCallback != null)
			{
				parentCancelCallback();
			}
		}));
	}

	private void PurchaseSkin(UnitLevelProgressionDataModel skin)
	{
		UserPriceDataModel priceForID = ItemPriceDataModel.GetPriceForID(skin.priceId);
		if (UserProfile.player.CanAfford(priceForID))
		{
			PurchaseSkinTransaction(skin, UserPriceDataModel.PaymentType.Normal);
		}
		else if (priceForID.HasItemOfType(UserInventory.ItemType.Coins))
		{
			UserPriceDataModel premiumPrice = UserPriceDataModel.GetPremiumPrice(priceForID, UserProfile.player, Constants.SkinGemToCashConversion);
			PopupManager.ShowPopup(PopupDataModel.PriceConfirmationPopUp(premiumPrice, "ui_not_enough_cash_title".Localize("Not enough cash"), "ui_not_enough_cash_desc".Localize("Pay with gems?"), delegate
			{
				if (UserProfile.player.CanAfford(premiumPrice))
				{
					PurchaseSkinTransaction(skin, UserPriceDataModel.PaymentType.UsePremiumForDifference);
					Reporting.GemsToCashPopupEvent(PurchaseSource.SkinPurchase, PurchaseAction.purchase, premiumPrice);
				}
				else
				{
					NotEnoughGems();
				}
			}, delegate
			{
				Reporting.GemsToCashPopupEvent(PurchaseSource.SkinPurchase, PurchaseAction.cancelled, premiumPrice);
			}, delegate
			{
				Reporting.GemsToCashPopupEvent(PurchaseSource.SkinPurchase, PurchaseAction.cancelled, premiumPrice);
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

	private void PurchaseSkinTransaction(UnitLevelProgressionDataModel skin, UserPriceDataModel.PaymentType type)
	{
		Singleton<SessionManager>.instance.PurchaseSkin(skin, type, null);
		SkinPurchasedSuccessfully(skin);
	}

	private void SkinPurchasedSuccessfully(UnitLevelProgressionDataModel skin)
	{
		if (parentCallback != null)
		{
			parentCallback();
		}
	}
}
