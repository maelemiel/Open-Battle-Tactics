using UnityEngine;

public class BuyItemPopUpController : PopupController
{
	private const string BUTTON_UNACTIVE_SPRITE_NAME = "large_button_grey";

	private const string BUTTON_ACTIVE_SPRITE_NAME = "large_button_green";

	[SerializeField]
	private PriceLabelController priceLabel;

	private UserPriceDataModel buyItemPrice;

	[SerializeField]
	private tk2dTextMesh rightDownLabel;

	[SerializeField]
	private tk2dTextMesh leftDownLabel;

	protected override void Start()
	{
		base.Start();
		buyItemPrice = model.payload as UserPriceDataModel;
		if ((bool)_rightLabel && (bool)rightDownLabel)
		{
			rightDownLabel.text = _rightLabel.text;
		}
		if ((bool)_leftLabel && (bool)leftDownLabel)
		{
			leftDownLabel.text = _leftLabel.text;
		}
		UpdatePrice();
	}

	private void UpdatePrice()
	{
		priceLabel.ConfigurePriceLabel(buyItemPrice);
	}

	private void OnClickYes()
	{
		if (!UserProfile.player.CanAfford(buyItemPrice))
		{
			PopupManager.ShowPopup(PopupDataModel.NoYes("ui_not_enough_gems_title".Localize("Not enough gems"), "ui_not_enough_gems_desc".Localize("Do you want to go buy more gems?"), delegate
			{
				TopBarController.instance.LoadShop();
			}));
		}
		else if (model.rightAction != null)
		{
			model.rightAction();
		}
		Close();
	}

	private void OnClickNo()
	{
		OnCloseButton();
	}
}
