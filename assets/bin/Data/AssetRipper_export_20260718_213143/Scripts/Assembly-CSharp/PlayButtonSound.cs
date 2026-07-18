using UnityEngine;

public class PlayButtonSound : MonoBehaviour
{
	public AudioClip pressSound;

	private void OnClick()
	{
		Singleton<AudioManager>.instance.PlaySfx(pressSound);
	}
}
