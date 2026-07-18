using UnityEngine;

public class ItemsMixerPlayerSlotViewController : MonoBehaviour
{
	public int slotIndex;

	public Color slotColor = Color.white;

	public tk2dBaseSprite arrow;

	public void SetArrowColor()
	{
		if (arrow != null)
		{
			arrow.color = slotColor;
		}
	}

	public void RestoreArrowColor()
	{
		if (arrow != null)
		{
			arrow.color = Color.white;
		}
	}
}
