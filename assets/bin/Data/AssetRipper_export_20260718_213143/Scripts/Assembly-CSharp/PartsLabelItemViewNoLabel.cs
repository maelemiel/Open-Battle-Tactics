public class PartsLabelItemViewNoLabel : PartsLabelItemView
{
	protected override void SetupPriceLabel(ItemCollectionDataModel.Item priceData, bool showAvailable = false)
	{
		if ((bool)priceLabel)
		{
			priceLabel.text = string.Empty;
		}
	}
}
