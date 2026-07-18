using UnityEngine;

public class ToggleFromMetadata : MonoBehaviour
{
	private void OnEnable()
	{
		base.gameObject.SetActive(Constants.ShowSpecialHomeContent);
	}
}
