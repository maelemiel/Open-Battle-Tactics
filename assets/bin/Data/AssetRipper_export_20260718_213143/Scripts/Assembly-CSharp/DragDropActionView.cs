using UnityEngine;

public class DragDropActionView : MonoBehaviour
{
	[SerializeField]
	private tk2dTextMesh textfield;

	private DragDropItem dragItem;

	public void Configure(DragDropItem dragItem, string actionText)
	{
		this.dragItem = dragItem;
		textfield.text = actionText;
	}

	private void LateUpdate()
	{
		if (dragItem != null)
		{
			base.transform.position = dragItem.transform.position;
		}
	}
}
