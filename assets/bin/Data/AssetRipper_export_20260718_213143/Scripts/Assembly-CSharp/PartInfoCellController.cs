using UnityEngine;

public class PartInfoCellController : ScrollableCell
{
	private PartsLabelItemView partsLabel;

	private PrefabProxy partsProxy;

	[SerializeField]
	private GameObject spriteOverlay;

	private void Awake()
	{
		partsLabel = GetComponent<PartsLabelItemView>();
	}

	public override void ConfigureCellData()
	{
		ItemCollectionDataModel.Item item = (ItemCollectionDataModel.Item)dataObject;
		if (item != null)
		{
			partsLabel.SetupPriceItem(item, true);
			string empty = string.Empty;
			if (UserProfile.player.parts.ContainsKey(item.itemId.ToString()))
			{
				empty = item.amount.ToString();
				spriteOverlay.SetActive(false);
			}
			else
			{
				spriteOverlay.SetActive(true);
				empty = "???";
			}
			partsLabel.Label = empty;
		}
	}
}
