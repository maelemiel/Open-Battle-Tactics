using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioSequence : MonoBehaviour
{
	public List<SfxWithTime> sounds = new List<SfxWithTime>();

	private void Start()
	{
		foreach (SfxWithTime sound in sounds)
		{
			StartCoroutine(PlaySfxWithDelay(sound.sfxTrigger, sound.time));
		}
	}

	private IEnumerator PlaySfxWithDelay(AudioTrigger audioTrigger, float delay)
	{
		if (delay > 0f)
		{
			yield return new WaitForSeconds(delay);
		}
		audioTrigger.Play();
	}
}
