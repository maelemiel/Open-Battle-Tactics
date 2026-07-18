using System;

[Serializable]
public class AudioTriggerSoundPaths
{
	public AudioTrigger audioTrigger;

	public AudioClipPaths audioClipPaths;

	public AudioTriggerSoundPaths(AudioTrigger audioTrigger, AudioClipPaths audioClipPaths)
	{
		this.audioTrigger = audioTrigger;
		this.audioClipPaths = audioClipPaths;
	}
}
