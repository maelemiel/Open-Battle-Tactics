using System;
using System.Collections;
using UnityEngine;

public class GenericGachaBoxController : MonoBehaviour
{
	private const int SORTING_ORDER_TANK_SPRITE = 1;

	[SerializeField]
	private GameObject crateBox;

	[SerializeField]
	private tk2dSpineAnimation[] fireworkAnimations;

	[SerializeField]
	private tk2dSpineAnimation confettiAnimation;

	[SerializeField]
	private tk2dSpineAnimation revealAnimation;

	[HideInInspector]
	public GachaSceneController controller;

	public tk2dUIItem uiButton;

	private Action OnOpenComplete;

	public void Open(Action onOpenComplete = null)
	{
		OnOpenComplete = onOpenComplete;
		OpenSequence();
	}

	private void OpenSequence()
	{
		uiButton.enabled = false;
		ShowBoxOpenAnimation();
		StartCoroutine(ShowFireworksAnimation());
	}

	private void ShowBoxOpenAnimation()
	{
		AudioTrigger.CrateBreak.Play();
		AudioTrigger.CrowdCheering.Play();
		AudioTrigger.GachaSuperRareRevealed.Play();
		if ((bool)crateBox)
		{
			UnityEngine.Object.Destroy(crateBox.gameObject);
		}
		ActivateAndAutodestroyAnimation(confettiAnimation);
		ActivateAndAutodestroyAnimation(revealAnimation);
	}

	private IEnumerator ShowFireworksAnimation()
	{
		AudioTrigger.Fireworks.Play();
		for (int i = 0; i < fireworkAnimations.Length; i++)
		{
			ActivateAndAutodestroyAnimation(fireworkAnimations[i]);
			yield return new WaitForSeconds(0.5f);
		}
		if (OnOpenComplete != null)
		{
			OnOpenComplete();
		}
	}

	private void ActivateAndAutodestroyAnimation(tk2dSpineAnimation spineAnimation)
	{
		spineAnimation.gameObject.SetActive(true);
		AutodestroySpineAnimation.Autodestroy(spineAnimation);
	}
}
