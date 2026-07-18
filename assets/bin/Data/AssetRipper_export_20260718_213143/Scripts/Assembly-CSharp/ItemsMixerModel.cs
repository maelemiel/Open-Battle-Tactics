public class ItemsMixerModel
{
	public int itemType;

	public int itemId;

	public int priceItemType;

	public int priceItemId;

	public int selectedSlot;

	public ItemsMixerModel(int itemType, int itemId, int priceItemType, int priceItemId, int selectedSlot)
	{
		this.itemType = itemType;
		this.itemId = itemId;
		this.priceItemType = priceItemType;
		this.priceItemId = priceItemId;
		this.selectedSlot = selectedSlot;
	}
}
