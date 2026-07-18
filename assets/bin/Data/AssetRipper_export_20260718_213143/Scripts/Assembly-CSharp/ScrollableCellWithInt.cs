using UnityEngine;

public class ScrollableCellWithInt : ScrollableCell
{
	[SerializeField]
	private tk2dTextMesh textMesh;

	public override void ConfigureCell()
	{
		if ((bool)textMesh && dataObject != null)
		{
			textMesh.text = ((UnitDataModel)dataObject).id;
		}
	}
}
