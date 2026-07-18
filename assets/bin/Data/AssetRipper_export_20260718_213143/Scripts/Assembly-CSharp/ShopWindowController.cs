using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopWindowController : MonoBehaviour
{
	private const float OVERLAY_ALPHA = 0.75f;

	[SerializeField]
	private ScrollableAreaController shopScrollableArea;

	[SerializeField]
	private tk2dSlicedSprite overlaySprite;

	[SerializeField]
	private float openY;

	[SerializeField]
	private float closedY;

	[SerializeField]
	private float timeToOpen = 0.5f;

	private bool _isOpen;

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

	public event Action OnShopOpened;

	public event Action OnShopClosed;

	private void Awake()
	{
		base.transform.localPosition = new Vector3(base.transform.localPosition.x, closedY, base.transform.localPosition.z);
		if ((bool)overlaySprite)
		{
			overlaySprite.gameObject.SetActive(false);
		}
		if ((bool)shopScrollableArea)
		{
			shopScrollableArea.gameObject.SetActive(false);
		}
	}

	public void SetOpen(bool isOpen)
	{
		base.gameObject.SetActive(true);
		StopAllCoroutines();
		StartCoroutine(OpenCloseSequence(isOpen));
		if (isOpen)
		{
			StartCoroutine(FetchShopItems());
		}
	}

	private IEnumerator OpenCloseSequence(bool isOpen)
	{
		if ((bool)overlaySprite)
		{
			overlaySprite.gameObject.SetActive(isOpen);
			if (isOpen)
			{
				overlaySprite.TweenAlpha(0.75f, timeToOpen);
			}
		}
		Extensions.TweenLocalYPosition(newLocalYPosition: (!isOpen) ? closedY : openY, transform: base.transform, duration: timeToOpen);
		yield return new WaitForSeconds(timeToOpen);
		if (!isOpen)
		{
			if ((bool)shopScrollableArea)
			{
				shopScrollableArea.gameObject.SetActive(false);
			}
			base.gameObject.SetActive(false);
			if (this.OnShopClosed != null)
			{
				this.OnShopClosed();
			}
		}
		else if (this.OnShopOpened != null)
		{
			this.OnShopOpened();
		}
	}

	private IEnumerator FetchShopItems()
	{
		Singleton<BankService>.instance.GetASCItems(delegate(bool querySuccess, List<ShopItem> shopItems)
		{
			if (querySuccess && (bool)shopScrollableArea)
			{
				shopScrollableArea.gameObject.SetActive(true);
				shopScrollableArea.InitializeWithData(shopItems);
			}
		});
		yield break;
	}
}
