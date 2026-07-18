using System;

[Serializable]
public class AudioTriggerSoundReferences
{
	public AudioTrigger audioTrigger;

	public AudioClipReferences audioClipReferences;

	public AudioTriggerSoundReferences(AudioTrigger audioTrigger, AudioClipReferences audioClipReferences)
	{
		this.audioTrigger = audioTrigger;
		this.audioClipReferences = audioClipReferences;
	}
}
