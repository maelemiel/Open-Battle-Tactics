using UnityEngine;

public class PriceConfirmationPopUpController : PopupController
{
	[SerializeField]
	private PriceLabelController priceLabel;

	private ItemCollectionDataModel priceDataModel;

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
		if ((bool)downRightLabel && (bool)_rightLabel)
		{
			downRightLabel.text = _rightLabel.text;
		}
		priceDataModel = model.payload as ItemCollectionDataModel;
		UpdatePrice();
	}

	private void UpdatePrice()
	{
		priceLabel.ConfigurePriceLabel(priceDataModel);
	}

	private void OnClickYes()
	{
		if (model.rightAction != null)
		{
			model.rightAction();
		}
		Close();
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
