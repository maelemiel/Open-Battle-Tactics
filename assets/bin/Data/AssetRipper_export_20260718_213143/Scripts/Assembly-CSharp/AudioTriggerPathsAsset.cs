using System.Collections.Generic;
using UnityEngine;

public class AudioTriggerPathsAsset : ScriptableObject
{
	public List<AudioTriggerSoundPaths> audioClipPaths = new List<AudioTriggerSoundPaths>();

	private Dictionary<AudioTrigger, AudioClipPaths> audioClipPathsDictionary = new Dictionary<AudioTrigger, AudioClipPaths>();

	public void Init()
	{
		audioClipPathsDictionary.Clear();
		foreach (AudioTriggerSoundPaths audioClipPath in audioClipPaths)
		{
			if (audioClipPathsDictionary.ContainsKey(audioClipPath.audioTrigger))
			{
				Log.Warning("Item with key: " + audioClipPath.audioTrigger.ToString() + " already exists in the dictionary. Skipping");
			}
			else
			{
				audioClipPathsDictionary.Add(audioClipPath.audioTrigger, audioClipPath.audioClipPaths);
			}
		}
	}

	public AudioClipPaths GetPathsForAudioTrigger(AudioTrigger audioTrigger)
	{
		if (!audioClipPathsDictionary.ContainsKey(audioTrigger))
		{
			Log.Warning("[AudioTriggerPaths Asset Database] The given AudioTrigger key: [" + audioTrigger.ToString() + "] was not present in the paths dictionary.  You might need to include it");
			return null;
		}
		return audioClipPathsDictionary[audioTrigger];
	}
}
