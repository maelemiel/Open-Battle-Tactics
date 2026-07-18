using System.Collections;
using UnityEngine;

public class ShopItemController : MonoBehaviour
{
	public ShopItem shopItemData;

	[SerializeField]
	private ShopItemView shopItemView;

	[SerializeField]
	private FireworksAnimation fireworks;

	public void Init(ShopItem shopItemData)
	{
		this.shopItemData = shopItemData;
		shopItemView.ConfigureView(shopItemData);
	}

	public void OnPress()
	{
		int loadingPopupId = LoadingPopupManager.ShowLoadingPopup(0f);
		Singleton<BankService>.instance.PurchaseASC(shopItemData, delegate(bool purchaseSuccess)
		{
			LoadingPopupManager.ClearLoadingPopup(loadingPopupId);
			loadingPopupId = -1;
			if (purchaseSuccess)
			{
				Log.InfoTag("ASC purchase successful.", null, "Shop");
				Singleton<BankService>.instance.GetCurrencyBalance(delegate(int balance)
				{
					UserProfile.player.gems = balance;
				});
				OtherLevelsHelper.UsePlacement("In-app_purhcase", "Placement 5", false);
				if ((bool)fireworks)
				{
					fireworks.PlayEffect();
					AudioTrigger.CrowdCheering.Play();
				}
				string title = "ui_purchase_success".Localize("Purchase Success");
				PopupDataModel popupModel = PopupDataModel.Ok(title, shopItemData.value + " " + UserInventory.ItemType.PremiumCurrency.GetLocalizedName());
				PopupManager.ShowPopup(popupModel);
				StartCoroutine(AnimateGems(shopItemData));
			}
			else
			{
				Log.ErrorTag("ASC purchase unsuccessful.", null, "Shop");
			}
		});
	}

	private IEnumerator AnimateGems(ShopItem shopItemData)
	{
		CurrencyEffect currencyEffectGO = CurrencyEffect.Create(UserInventory.ItemType.PremiumCurrency, shopItemData.value, 50);
		currencyEffectGO.gameObject.SetLayerRecursively(base.gameObject.layer);
		currencyEffectGO.transform.parent = TopBarController.instance.transform;
		currencyEffectGO.transform.position = base.transform.position;
		yield break;
	}
}
