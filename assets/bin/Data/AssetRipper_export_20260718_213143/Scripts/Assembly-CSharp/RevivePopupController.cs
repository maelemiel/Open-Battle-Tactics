using System;
using UnityEngine;

public class RevivePopupController : PopupController
{
	[SerializeField]
	protected tk2dTextMesh secondaryLeftLabel;

	[SerializeField]
	protected tk2dTextMesh secondaryRightLabel;

	[SerializeField]
	protected tk2dTextMesh countDownTimer;

	[SerializeField]
	protected PriceLabelController availablePriceLabel;

	[SerializeField]
	protected PriceLabelController upButtonPriceLabel;

	[SerializeField]
	protected PriceLabelController downButtonPriceLabel;

	private float reviveTime;

	protected override void Start()
	{
		base.Start();
		if (secondaryLeftLabel != null)
		{
			secondaryLeftLabel.text = (string.IsNullOrEmpty(model.leftLabel) ? secondaryLeftLabel.text : model.leftLabel);
		}
		if (secondaryRightLabel != null)
		{
			secondaryRightLabel.text = (string.IsNullOrEmpty(model.rightLabel) ? secondaryRightLabel.text : model.rightLabel);
		}
		UserPriceDataModel priceForID = ItemPriceDataModel.GetPriceForID(Constants.RaidBossRevivePriceA);
		if (UserProfile.player.CanAfford(priceForID))
		{
			upButtonPriceLabel.ConfigurePriceLabel(priceForID);
			downButtonPriceLabel.ConfigurePriceLabel(priceForID);
		}
		else
		{
			priceForID = ItemPriceDataModel.GetPriceForID(Constants.RaidBossRevivePriceB);
			upButtonPriceLabel.ConfigurePriceLabel(priceForID);
			downButtonPriceLabel.ConfigurePriceLabel(priceForID);
		}
		if (priceForID.items.Count > 0 && priceForID.items[0] != null)
		{
			UserInventory.ItemType itemType = priceForID.items[0].itemType;
			int item = UserProfile.player.inventory.GetItem(itemType, 0);
			ItemCollectionDataModel priceDataModel = new ItemCollectionDataModel(itemType, item);
			availablePriceLabel.ConfigurePriceLabel(priceDataModel);
		}
		reviveTime = Constants.RaidBossReviveTimer;
	}

	protected override void Update()
	{
		countDownTimer.text = ((int)reviveTime).ToString();
		countDownTimer.scale = Vector3.one * 2f + (0.5f + Mathf.Cos(reviveTime * (float)Math.PI * 2f)) / 2f * Vector3.one * 0.25f;
		reviveTime -= Time.deltaTime;
		if (reviveTime <= 0f)
		{
			OnCloseButton();
		}
	}

	public override void OnCloseButton()
	{
		if (model.closeButtonAction != null)
		{
			model.closeButtonAction();
		}
		Close();
	}
}
