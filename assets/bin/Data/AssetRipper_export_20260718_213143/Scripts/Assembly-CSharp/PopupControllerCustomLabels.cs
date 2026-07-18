using UnityEngine;

public class PopupControllerCustomLabels : PopupController
{
	[SerializeField]
	protected tk2dTextMesh secondaryLeftLabel;

	[SerializeField]
	protected tk2dTextMesh secondaryRightLabel;

	protected override void Start()
	{
		base.Start();
		if (secondaryLeftLabel != null)
		{
			secondaryLeftLabel.text = (string.IsNullOrEmpty(model.leftLabel) ? secondaryLeftLabel.text : model.leftLabel);
		}
		if (secondaryRightLabel != null)
		{
			secondaryRightLabel.text = (string.IsNullOrEmpty(model.rightLabel) ? secondaryRightLabel.text : model.rightLabel);
		}
	}

	public override void OnCloseButton()
	{
		if (model.closeButtonAction != null)
		{
			model.closeButtonAction();
		}
		Close();
	}
}
