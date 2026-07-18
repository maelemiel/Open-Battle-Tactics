using System;
using System.Collections;
using Holoville.HOTween;
using UnityEngine;

public class MovingObjectController : MonoBehaviour
{
	public float timeToOpen = 0.5f;

	public float overlayFinalAlpha = 0.65f;

	private Tweener currentTween;

	private Tweener alphaTween;

	[SerializeField]
	private tk2dBaseSprite overlay;

	private bool _isOpen;

	[SerializeField]
	private float openY;

	[SerializeField]
	private float closedY;

	public bool IsOpen
	{
		get
		{
			return _isOpen;
		}
		set
		{
			SetOpen(value);
			_isOpen = value;
		}
	}

	public event Action OnOpened;

	public event Action OnClosed;

	public void SetOpen(bool isOpen)
	{
		StopAllCoroutines();
		StartCoroutine(OpenCloseSequence(isOpen));
	}

	private IEnumerator OpenCloseSequence(bool isOpen)
	{
		float targetY = ((!isOpen) ? closedY : openY);
		float alphaTo = ((!isOpen) ? 0f : overlayFinalAlpha);
		if (currentTween != null)
		{
			currentTween.Kill();
		}
		currentTween = base.transform.TweenLocalYPosition(targetY, timeToOpen);
		if ((bool)overlay)
		{
			if (alphaTween != null)
			{
				alphaTween.Kill();
			}
			alphaTween = overlay.TweenAlpha(alphaTo, timeToOpen);
		}
		yield return new WaitForSeconds(timeToOpen);
		if (isOpen)
		{
			if (this.OnOpened != null)
			{
				this.OnOpened();
			}
			yield break;
		}
		base.gameObject.SetActive(false);
		if (this.OnClosed != null)
		{
			this.OnClosed();
		}
	}

	private void CloseWindow()
	{
		if (IsOpen)
		{
			IsOpen = false;
		}
		if (currentTween != null)
		{
			currentTween.Kill();
		}
	}
}
