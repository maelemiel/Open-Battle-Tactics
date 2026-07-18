using UnityEngine;

public class FirstrewardEnergyLabelItemView : EnergyItemView
{
	[SerializeField]
	private GameObject[] _energies;

	protected override void SetupPriceIcon(ItemCollectionDataModel.Item itemData)
	{
		if (itemData.Unit == null)
		{
			Log.Warning(string.Concat("Unit with unit of: ", itemData.Unit, " is null"));
		}
		else if ((bool)energyNameLabel)
		{
			energyNameLabel.text = "metadata_item_name_2".Localize("Ticket");
		}
		if (itemData.amount >= 6)
		{
			for (int i = 0; i < _energies.Length; i++)
			{
				_energies[i].SetActive(true);
			}
		}
		else
		{
			for (int j = 0; j < itemData.amount; j++)
			{
				_energies[j].SetActive(true);
			}
		}
	}
}
