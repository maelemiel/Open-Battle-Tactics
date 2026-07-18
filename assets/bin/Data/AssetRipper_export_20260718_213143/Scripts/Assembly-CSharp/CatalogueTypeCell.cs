using UnityEngine;

public class CatalogueTypeCell : ScrollableCell
{
	private UnitTypeDataModel unitTypeDataModel;

	[SerializeField]
	private tk2dTextMesh unitTypeName;

	[SerializeField]
	private tk2dTextMesh unitTypeName_2;

	public UnitTypeDataModel UnitType
	{
		get
		{
			return unitTypeDataModel;
		}
	}

	private string UnitTypeName
	{
		set
		{
			if ((bool)unitTypeName)
			{
				unitTypeName.text = value;
			}
			if ((bool)unitTypeName_2)
			{
				unitTypeName_2.text = value;
			}
		}
	}

	public override void ConfigureCellData()
	{
		if (dataObject != null)
		{
			unitTypeDataModel = (UnitTypeDataModel)dataObject;
			UnitTypeName = unitTypeDataModel.keyName.Localize(unitTypeDataModel.keyName);
		}
	}

	private void OnTouch()
	{
		if (unitTypeDataModel != null)
		{
			Debug.LogWarning("UnitTypeDataModel: " + unitTypeDataModel.id + ". Name: " + unitTypeDataModel.keyName.Localize(unitTypeDataModel.keyName));
		}
	}
}
