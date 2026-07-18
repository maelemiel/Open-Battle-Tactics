using System;
using UnityEngine;

[Serializable]
public class AudioClipPaths
{
	public string[] paths;

	public AudioClipPaths(string[] paths)
	{
		this.paths = paths;
	}

	public string GetAudioClipPath()
	{
		if (paths.Length == 0)
		{
			return null;
		}
		if (paths.Length == 1)
		{
			return paths[0];
		}
		return paths[UnityEngine.Random.Range(0, paths.Length)];
	}
}
