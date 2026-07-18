using UnityEngine;

public class HelpDescriptionEntryCell : ScrollableCell
{
	[SerializeField]
	private tk2dTextMesh title;

	[SerializeField]
	private tk2dTextMesh description;

	public override void ConfigureCellData()
	{
		if (base.DataObject != null)
		{
			HelpRegistersDataModel helpRegistersDataModel = base.DataObject as HelpRegistersDataModel;
			title.text = LocalizationManager.GetString(helpRegistersDataModel.titleKey, helpRegistersDataModel.name);
			title.Commit();
			description.text = LocalizationManager.GetString(helpRegistersDataModel.descriptionKey);
			description.Commit();
		}
		base.ConfigureCellData();
	}
}
