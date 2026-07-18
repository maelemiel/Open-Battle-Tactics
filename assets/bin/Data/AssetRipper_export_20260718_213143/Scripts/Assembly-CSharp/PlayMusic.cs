using UnityEngine;

public class PlayMusic : MonoBehaviour
{
	public AudioClip music;

	private void Start()
	{
		Singleton<AudioManager>.instance.PlayMusic(music);
	}
}
