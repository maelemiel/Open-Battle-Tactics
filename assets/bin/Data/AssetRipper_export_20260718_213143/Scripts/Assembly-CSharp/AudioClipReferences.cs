using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AudioClipReferences
{
	public List<AudioClip> audioClips;

	public AudioClipReferences(List<AudioClip> audioClips)
	{
		this.audioClips = audioClips;
	}

	public AudioClip GetAudioClip()
	{
		if (audioClips.Count == 0)
		{
			return null;
		}
		if (audioClips.Count == 1)
		{
			return audioClips[0];
		}
		return audioClips[UnityEngine.Random.Range(0, audioClips.Count)];
	}
}
