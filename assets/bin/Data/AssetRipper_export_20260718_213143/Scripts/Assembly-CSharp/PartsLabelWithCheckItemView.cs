using UnityEngine;

public class PartsLabelWithCheckItemView : PartsLabelItemView
{
	public tk2dSprite checkSprite;

	[SerializeField]
	private bool showIfCanAfford;

	private void Awake()
	{
		if ((bool)checkSprite)
		{
			checkSprite.gameObject.SetActive(false);
		}
	}

	protected override void SetupPriceLabel(ItemCollectionDataModel.Item priceData, bool showAvailable = false)
	{
		if (priceLabel == null)
		{
			return;
		}
		UserProfile player = UserProfile.player;
		if (player.CanAffordItem(priceData))
		{
			priceLabel.gameObject.SetActive(showIfCanAfford);
			if (checkSprite != null)
			{
				checkSprite.gameObject.SetActive(true);
			}
			if (showIfCanAfford)
			{
				base.SetupPriceLabel(priceData, showAvailable);
				priceLabel.color = Color.green;
			}
		}
		else
		{
			base.SetupPriceLabel(priceData, showAvailable);
			priceLabel.color = Color.red;
		}
	}
}
