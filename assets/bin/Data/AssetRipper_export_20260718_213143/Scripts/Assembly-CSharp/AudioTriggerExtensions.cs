using System;
using System.Collections.Generic;
using UnityEngine;

public static class AudioTriggerExtensions
{
	public static void PlayMusic(this AudioTrigger audioName)
	{
		List<AudioClipReferences> audioClipReferences = GetAudioClipReferences(audioName);
		if (audioClipReferences != null && audioClipReferences.Count > 0)
		{
			if (audioClipReferences.Count > 1)
			{
				Log.Warning("AudioClipReferences with ID: " + audioName.ToString() + " has more than one audioClip");
			}
			Singleton<AudioManager>.instance.PlayMusic(audioClipReferences[0].GetAudioClip());
		}
	}

	public static float GetMusicLength(this AudioTrigger audioName)
	{
		List<AudioClipReferences> audioClipReferences = GetAudioClipReferences(audioName);
		if (audioClipReferences == null)
		{
			return 0f;
		}
		if (audioClipReferences.Count > 0)
		{
			if (audioClipReferences.Count > 1)
			{
				Log.Warning("AudioClipReferences with ID: " + audioName.ToString() + " has more than one audioClip");
			}
			return audioClipReferences[0].GetAudioClip().length;
		}
		return 0f;
	}

	public static AudioManager.Sfx Play(this AudioTrigger audioName, Action finishedCallback = null)
	{
		List<AudioClipReferences> audioClipReferences = GetAudioClipReferences(audioName);
		if (audioClipReferences == null)
		{
			return null;
		}
		AudioClip audioClip = null;
		AudioManager.Sfx result = null;
		if (audioClipReferences.Count > 0 && audioClipReferences.Count > 1)
		{
			Log.Warning("AudioClipReferences with ID: " + audioName.ToString() + " has more than one audioClip. Only the last one will be returned");
		}
		for (int i = 0; i < audioClipReferences.Count; i++)
		{
			audioClip = audioClipReferences[i].GetAudioClip();
			if ((bool)audioClip)
			{
				result = Singleton<AudioManager>.instance.PlaySfx(audioClip, finishedCallback);
			}
		}
		return result;
	}

	public static AudioManager.RepeatingSfx PlayRepeating(this AudioTrigger audioName)
	{
		List<AudioClipReferences> audioClipReferences = GetAudioClipReferences(audioName);
		if (audioClipReferences == null)
		{
			return null;
		}
		AudioManager.RepeatingSfx result = null;
		if (audioClipReferences.Count > 0)
		{
			if (audioClipReferences.Count > 1)
			{
				Log.Warning("AudioClipReferences with ID: " + audioName.ToString() + " has more than one audioClip");
			}
			result = Singleton<AudioManager>.instance.PlayRepeatingSfx(audioClipReferences[0].GetAudioClip(), 0f);
		}
		return result;
	}

	private static List<AudioClipReferences> GetAudioClipReferences(AudioTrigger audioName)
	{
		List<AudioClipReferences> audioClipDefinitions = Singleton<AudioCacheManager>.instance.GetAudioClipDefinitions(audioName);
		if (audioClipDefinitions == null)
		{
			return null;
		}
		if (audioClipDefinitions == null)
		{
			Log.Warning("AudioClipReferences with ID: " + audioName.ToString() + " not found");
			return null;
		}
		return audioClipDefinitions;
	}
}
