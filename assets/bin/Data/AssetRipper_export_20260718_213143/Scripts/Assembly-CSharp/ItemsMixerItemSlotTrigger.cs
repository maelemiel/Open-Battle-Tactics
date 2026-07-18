using UnityEngine;

public class ItemsMixerItemSlotTrigger : MonoBehaviour
{
	public int itemSlotIndex;

	public ItemsMixerBoardController sceneController;

	public void OnTriggerEnter2D(Collider2D other)
	{
		sceneController.EndTrajectory(itemSlotIndex);
	}
}
