using System.Collections;
using UnityEngine;

public class FireworksAnimation : MonoBehaviour
{
	[SerializeField]
	private tk2dSpineAnimation[] fireworkAnimations;

	public float timeBetweenFireworks = 0.5f;

	public bool playOnStart;

	private void Start()
	{
		if (playOnStart)
		{
			PlayEffect();
			return;
		}
		for (int i = 0; i < fireworkAnimations.Length; i++)
		{
			fireworkAnimations[i].gameObject.SetActive(false);
		}
	}

	public void PlayEffect()
	{
		AudioTrigger.Fireworks.Play();
		StartCoroutine(ShowFireworksAnimation());
	}

	private IEnumerator ShowFireworksAnimation()
	{
		for (int i = 0; i < fireworkAnimations.Length; i++)
		{
			fireworkAnimations[i].gameObject.SetActive(true);
			fireworkAnimations[i].Reset();
			yield return new WaitForSeconds(timeBetweenFireworks);
		}
	}
}
