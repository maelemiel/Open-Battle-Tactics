using System.Collections;
using UnityEngine;

public class SoundProxy : BaseProxy<AudioClip>
{
	private AudioClip clip;

	public AudioClip audioClip
	{
		get
		{
			return clip;
		}
	}

	public virtual IEnumerator ChangeAssetCoroutine(string assetName, int bundleId)
	{
		base.assetName = assetName;
		base.bundleId = bundleId;
		if (clip != null)
		{
			Object.Destroy(clip);
		}
		int dotIndex = assetName.LastIndexOf('.');
		if (dotIndex >= 0)
		{
			string trimmedAssetName = assetName.Remove(dotIndex);
			AudioClip localAsset = Resources.Load(trimmedAssetName) as AudioClip;
			if (localAsset != null)
			{
				yield return StartCoroutine(ProcessAsset(localAsset));
				yield break;
			}
		}
		if (!string.IsNullOrEmpty(assetName) && bundleId != -1)
		{
			yield return StartCoroutine(UpdateAsset(true));
		}
	}

	protected override IEnumerator ProcessAsset(AudioClip clip)
	{
		this.clip = clip;
		yield break;
	}
}
