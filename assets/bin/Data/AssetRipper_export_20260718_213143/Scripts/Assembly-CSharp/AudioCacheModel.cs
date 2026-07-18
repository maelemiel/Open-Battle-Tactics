using System.Collections.Generic;
using UnityEngine;

public class AudioCacheModel : MonoBehaviour
{
	public List<AudioTrigger> audioTriggers = new List<AudioTrigger>();

	private void Awake()
	{
		if (!Singleton<AudioCacheManager>.instance.initialized)
		{
			Singleton<AudioCacheManager>.instance.Init();
		}
		Singleton<AudioCacheManager>.instance.RegisterAudioClips(audioTriggers);
	}
}
