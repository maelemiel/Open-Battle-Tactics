using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioCacheManager : Singleton<AudioCacheManager>
{
	private const string AUDIO_TRIGGER_PATHS_ASSET = "Data/AudioTriggerClips_Paths";

	public const string AB_FIRING_PREFIX = "AudioABEffectFiring_";

	public const string AB_HIT_PREFIX = "AudioABEffectFiring_";

	private Dictionary<AudioTrigger, List<AudioClipReferences>> loadedAudioClips = new Dictionary<AudioTrigger, List<AudioClipReferences>>();

	private AudioTriggerPathsAsset pathsAsset;

	public bool initialized;

	public void Init()
	{
		pathsAsset = Resources.Load("Data/AudioTriggerClips_Paths") as AudioTriggerPathsAsset;
		if (pathsAsset == null)
		{
			Log.Error("Audio trigger paths .asset not found");
		}
		else
		{
			pathsAsset.Init();
			Log.Debug("AudioTriggerPathsAsset asset successfully loaded. Referenced AudioTriggers: " + pathsAsset.audioClipPaths.Count);
		}
		initialized = true;
	}

	public void RegisterSingleAudioClip(AudioTrigger newAudioTrigger)
	{
		if (loadedAudioClips.ContainsKey(newAudioTrigger))
		{
			Log.Debug(string.Concat("AudioTrigger already loaded in this scene. [", newAudioTrigger, "]"));
			return;
		}
		AudioClipReferences audioClipReferences = LoadAudioTriggerSounds(pathsAsset.GetPathsForAudioTrigger(newAudioTrigger));
		if (audioClipReferences != null)
		{
			loadedAudioClips[newAudioTrigger] = new List<AudioClipReferences> { audioClipReferences };
		}
	}

	public void RegisterSingleAudioClipFromAB(AudioTrigger newAudioTrigger)
	{
		if (loadedAudioClips.ContainsKey(newAudioTrigger))
		{
			Log.Debug(string.Concat("AudioTrigger already loaded in this scene. [", newAudioTrigger, "]"));
			return;
		}
		GetAudioClipFromAssetBundle(newAudioTrigger, delegate(AudioClipReferences references)
		{
			if (references != null)
			{
				loadedAudioClips[newAudioTrigger] = new List<AudioClipReferences> { references };
			}
		});
	}

	public void RegisterAudioClips(List<AudioTrigger> newAudioTriggers)
	{
		Dictionary<AudioTrigger, List<AudioClipReferences>> dictionary = new Dictionary<AudioTrigger, List<AudioClipReferences>>();
		newAudioTriggers.Add(AudioTrigger.Fireworks);
		foreach (AudioTrigger newAudioTrigger in newAudioTriggers)
		{
			if (loadedAudioClips.ContainsKey(newAudioTrigger))
			{
				if (dictionary.ContainsKey(newAudioTrigger))
				{
					Log.Warning(string.Concat("AudioTrigger already loaded in this scene. [", newAudioTrigger, "]"));
				}
				dictionary[newAudioTrigger] = loadedAudioClips[newAudioTrigger];
				continue;
			}
			AudioClipReferences audioClipReferences = LoadAudioTriggerSounds(pathsAsset.GetPathsForAudioTrigger(newAudioTrigger));
			if (audioClipReferences != null)
			{
				dictionary[newAudioTrigger] = new List<AudioClipReferences> { audioClipReferences };
			}
		}
		loadedAudioClips.Clear();
		loadedAudioClips = dictionary;
		Resources.UnloadUnusedAssets();
	}

	public AudioClipReferences LoadAudioTriggerSounds(AudioClipPaths soundPaths)
	{
		if (soundPaths == null)
		{
			return null;
		}
		List<AudioClip> list = new List<AudioClip>();
		for (int i = 0; i < soundPaths.paths.Length; i++)
		{
			list.Add(LoadAudioClip(soundPaths.paths[i]));
		}
		return new AudioClipReferences(list);
	}

	private AudioClip LoadAudioClip(string path)
	{
		AudioClip audioClip = Resources.Load(path) as AudioClip;
		if (audioClip == null)
		{
			Log.Warning("AudioClip not found. Path: " + path);
		}
		return audioClip;
	}

	public List<AudioClipReferences> GetAudioClipDefinitions(AudioTrigger audioClipID)
	{
		List<AudioClipReferences> result = null;
		if (loadedAudioClips.ContainsKey(audioClipID))
		{
			result = loadedAudioClips[audioClipID];
		}
		else
		{
			Log.Warning("[AudioCacheManager] AudioClipDefinitions for ID: " + audioClipID.ToString() + " not found");
		}
		return result;
	}

	private void GetAudioClipFromAssetBundle(AudioTrigger type, Action<AudioClipReferences> callback)
	{
		AudioTriggersDataModel audioTrigger = AudioTriggersDataModel.GetByType(type);
		Singleton<AssetBundleManager>.instance.GetAssetBundle(audioTrigger.AssetLinkage.bundleId, delegate(string err, AssetBundle ab, AssetBundleDataModel m)
		{
			Log.Warning("Asset bundle ready");
			Singleton<AssetBundleManager>.instance.RetainAssetBundle(m);
			AudioClip item = (AudioClip)ab.Load(audioTrigger.AssetLinkage.assetName, typeof(AudioClip));
			AudioClipReferences obj = new AudioClipReferences(new List<AudioClip> { item });
			if (callback != null)
			{
				callback(obj);
			}
		});
	}
}
