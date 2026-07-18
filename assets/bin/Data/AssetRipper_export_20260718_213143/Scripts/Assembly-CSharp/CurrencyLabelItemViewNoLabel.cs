public class CurrencyLabelItemViewNoLabel : CurrencyLabelItemView
{
	protected override void SetupPriceLabel(ItemCollectionDataModel.Item priceData, bool showAvailable = false)
	{
		if ((bool)priceLabel)
		{
			priceLabel.gameObject.SetActive(false);
		}
	}
}
