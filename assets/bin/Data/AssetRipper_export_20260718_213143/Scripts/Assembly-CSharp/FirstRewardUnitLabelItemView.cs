using UnityEngine;

public class FirstRewardUnitLabelItemView : UnitLabelItemView
{
	[SerializeField]
	protected tk2dTextMesh _unitName;

	[SerializeField]
	protected tk2dTextMesh _unitAbility;

	[SerializeField]
	protected UnitInfoView _unitInfo;

	protected override void SetupPriceIcon(ItemCollectionDataModel.Item itemData)
	{
		UnitDataModel unitDataModel = itemData.Unit.UnitDataModel;
		if ((bool)_unitName)
		{
			_unitName.text = unitDataModel.name;
			_unitName.Commit();
		}
		if ((bool)_unitAbility)
		{
		}
		if ((bool)_unitInfo)
		{
			_unitInfo.ConfigureUnitView(unitDataModel, itemData.Unit.level);
		}
	}

	protected override void SetupPriceLabel(ItemCollectionDataModel.Item itemData, bool showAvailable = false)
	{
	}
}
