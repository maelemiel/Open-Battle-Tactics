using System.Collections;
using UnityEngine;

public class SkipCooldownPopUpController : PopupController
{
	private const string BUTTON_UNACTIVE_SPRITE_NAME = "large_button_grey";

	private const string BUTTON_ACTIVE_SPRITE_NAME = "large_button_green";

	[SerializeField]
	private PriceLabelController skipPriceLabel;

	[SerializeField]
	private GameObject objectContainer;

	private UserPriceDataModel skipPrice;

	private UserTeam userTeam;

	private bool topbarWasShowing;

	[SerializeField]
	private tk2dTextMesh downRightLabel;

	protected override void Start()
	{
		base.Start();
		topbarWasShowing = TopBarController.instance.Visible;
		if (!topbarWasShowing)
		{
			TopBarController.instance.ShowHomeButton = false;
			TopBarController.instance.Visible = true;
		}
		if (model.viewObject != null)
		{
			objectContainer.transform.MakeChild(model.viewObject.transform);
			objectContainer.FitWithinBounds(objectContainer.collider.bounds);
			PrefabProxy component = model.viewObject.GetComponent<PrefabProxy>();
			if (component != null)
			{
				component.FitWithinBounds(objectContainer.collider.bounds);
			}
		}
		userTeam = model.payload as UserTeam;
		if ((bool)downRightLabel && (bool)_rightLabel)
		{
			downRightLabel.text = _rightLabel.text;
		}
		UpdatePrice();
		StartCoroutine(UpdateLoop());
	}

	private IEnumerator UpdateLoop()
	{
		while (userTeam.IsOnCooldown)
		{
			UpdatePrice();
			yield return new WaitForSeconds(1f);
		}
		OnClickYes();
	}

	private void UpdatePrice()
	{
		skipPrice = userTeam.GetPriceToSkipRepair();
		skipPriceLabel.ConfigurePriceLabel(skipPrice);
	}

	private void OnClickYes()
	{
		if (userTeam.IsOnCooldown)
		{
			int teamIndex = userTeam.index;
			long serverTime = TimeManager.ServerTime;
			if (!UserProfile.player.CanAfford(skipPrice))
			{
				UserPriceDataModel premiumPrice = UserPriceDataModel.GetPremiumPrice(skipPrice, UserProfile.player, Constants.CooldownGemToCashExchangeRate);
				PopupManager.ShowPopup(PopupDataModel.PriceConfirmationPopUp(premiumPrice, "ui_not_enough_cash_title".Localize("Not enough cash"), "ui_not_enough_cash_desc".Localize("Pay with gems?"), delegate
				{
					if (UserProfile.player.CanAfford(premiumPrice))
					{
						PreformTransaction(teamIndex, serverTime, UserPriceDataModel.PaymentType.UsePremiumForDifference);
						Reporting.GemsToCashPopupEvent(PurchaseSource.SkipCooldownPopup, PurchaseAction.purchase, skipPrice);
						Reporting.CurrencyTransactionEvent(PurchaseSource.SkipCooldownPopup, skipPrice);
					}
					else
					{
						PopupManager.ShowPopup(PopupDataModel.NoYes("ui_not_enough_gems_title".Localize("Not enough gems"), "ui_not_enough_gems_desc".Localize("Do you want to go buy more gems?"), delegate
						{
							PopupManager.DestroyAllPopups();
							TopBarController.instance.LoadShop();
						}));
					}
				}, delegate
				{
				}, delegate
				{
					Reporting.GemsToCashPopupEvent(PurchaseSource.SkipCooldownPopup, PurchaseAction.cancelled, premiumPrice);
				}));
				return;
			}
			PreformTransaction(teamIndex, serverTime, UserPriceDataModel.PaymentType.Normal);
			Reporting.CurrencyTransactionEvent(PurchaseSource.SkipCooldownPopup, skipPrice);
		}
		Close();
	}

	private void PreformTransaction(int teamIndex, long serverTime, UserPriceDataModel.PaymentType paymentType)
	{
		Singleton<SessionManager>.instance.SkipRepairTime(teamIndex, serverTime, paymentType);
		if (model.rightAction != null)
		{
			model.rightAction();
		}
	}

	private void OnClickNo()
	{
		OnCloseButton();
	}

	public override void Close()
	{
		base.Close();
		TopBarController.instance.Visible = topbarWasShowing;
	}
}
