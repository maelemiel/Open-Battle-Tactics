using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonalPartsSceneController : MonoBehaviour
{
	private const float OVERLAY_ALPHA = 0.75f;

	[SerializeField]
	protected ScrollableAreaController scrollableAreaController;

	[SerializeField]
	private tk2dSlicedSprite overlaySprite;

	[SerializeField]
	private float timeToOpen = 0.5f;

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

	public event Action OnPersonalPartsOpened;

	public event Action OnPersonalPartsClosed;

	private void Start()
	{
		InitList();
		scrollableAreaController.collider.enabled = false;
		if ((bool)overlaySprite)
		{
			overlaySprite.gameObject.SetActive(false);
		}
	}

	private void InitList()
	{
		List<ItemCollectionDataModel.Item> list = new List<ItemCollectionDataModel.Item>();
		List<UnitPartTypesDataModel> list2 = UnitPartTypesDataModel.GetAll().FindAll((UnitPartTypesDataModel x) => !x.IsHidden);
		list2.Sort((UnitPartTypesDataModel p1, UnitPartTypesDataModel p2) => p1.OrderBy.CompareTo(p2.OrderBy));
		foreach (UnitPartTypesDataModel item in list2)
		{
			list.Add(new ItemCollectionDataModel.Item(UserInventory.ItemType.Parts, int.Parse(item.id), UserProfile.player.inventory.GetParts(item.id)));
		}
		scrollableAreaController.InitializeWithData(list);
		scrollableAreaController.FixScrollLimits();
	}

	private void CloseWindow()
	{
		if (IsOpen)
		{
			IsOpen = false;
		}
	}

	private void GetPartsButtonClicked()
	{
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ShopItemsSuppliesScene);
		CloseWindow();
	}

	public void SetOpen(bool isOpen)
	{
		StopAllCoroutines();
		StartCoroutine(OpenCloseSequence(isOpen));
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
		if (isOpen)
		{
			if (this.OnPersonalPartsOpened != null)
			{
				this.OnPersonalPartsOpened();
			}
			scrollableAreaController.collider.enabled = true;
		}
		else
		{
			base.gameObject.SetActive(false);
			if (this.OnPersonalPartsClosed != null)
			{
				this.OnPersonalPartsClosed();
			}
		}
	}
}
