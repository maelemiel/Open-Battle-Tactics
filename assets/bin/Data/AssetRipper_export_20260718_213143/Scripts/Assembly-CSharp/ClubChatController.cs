using UnityEngine;

public class ClubChatController : MonoBehaviour
{
	[SerializeField]
	private ClubDataViewController dataForm;

	private void OnEnable()
	{
		if ((bool)dataForm)
		{
			dataForm.gameObject.SetActive(false);
		}
	}
}
