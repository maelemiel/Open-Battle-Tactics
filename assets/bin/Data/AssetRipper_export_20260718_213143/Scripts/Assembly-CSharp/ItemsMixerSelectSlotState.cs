using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using UnityEngine;

public class ItemsMixerSelectSlotState : GachaPlinkoBaseState
{
	private readonly Vector3 initialCellStateLocalPosition = new Vector3(-100f, 0f, 0f);

	[SerializeField]
	private GameObject itemMixerBoard;

	[SerializeField]
	private GameObject chipSlotsGameObject;

	[SerializeField]
	private ScrollableAreaController pricesScrollableArea;

	[SerializeField]
	private GameObject toggleButton;

	[SerializeField]
	private GameObject exitButton;

	[SerializeField]
	private tk2dTextMesh previewLabel;

	[SerializeField]
	private tk2dTextMesh slotsLabel;

	protected DragDropItem currentDragItem;

	protected DragDropItem selectedDragItem;

	protected Transform selectedTarget;

	protected int normalLayer;

	protected int dragLayer;

	protected Tweener dragEffectTween;

	private ItemCollectionDataModel.Item itemToSpend;

	private ItemsMixerPlayerSlotViewController selectedSlot;

	[SerializeField]
	private float chipScale = 0.7f;

	private ItemsMixerPlayerSlotViewController previousDropTargetSlot;

	private bool boardOnTop = true;

	public override IEnumerator StartStateSequence()
	{
		if ((bool)chipSlotsGameObject)
		{
			chipSlotsGameObject.SetActive(true);
		}
		if ((bool)toggleButton)
		{
			toggleButton.SetActive(true);
		}
		normalLayer = LayerMask.NameToLayer("UI 2D");
		dragLayer = LayerMask.NameToLayer("Drag");
		if ((bool)pricesScrollableArea)
		{
			foreach (DragDropItem cell in pricesScrollableArea.GetCellComponents<DragDropItem>(true))
			{
				InitDragDropItem(cell);
			}
		}
		else
		{
			Log.Error("[ItemsMixerSelectSlotState] PricesScrollableArea not found");
		}
		SetLabelsState(boardOnTop);
		yield break;
	}

	private bool PlayItemsMixerLogic()
	{
		bool result = false;
		UserProfile player = UserProfile.player;
		if (itemToSpend != null && player.CanAffordItem(itemToSpend))
		{
			player.RemoveItems(new ItemCollectionDataModel(itemToSpend));
			result = true;
		}
		return result;
	}

	private void PlayItemsMixerRequest()
	{
		if (base.ItemsMixer.selectedPrize != null && itemToSpend != null && selectedSlot != null && PlayItemsMixerLogic())
		{
			ItemsMixerModel itemsMixerModel = new ItemsMixerModel(base.ItemsMixer.selectedPrize.itemType, base.ItemsMixer.selectedPrize.itemId, (int)itemToSpend.itemType, itemToSpend.itemId, selectedSlot.slotIndex);
			GachaPoolsDataModel prizeGachaPoolsDataModel = base.ItemsMixer.gachaResultsPayload.prizeGachaPoolsDataModel;
			Singleton<SessionManager>.instance.BuyPrizeGacha(prizeGachaPoolsDataModel, itemsMixerModel, OnBuyItemsMixerResponse);
			base.ItemsMixer.SelectedPlayerSlot = selectedSlot;
			if ((bool)exitButton)
			{
				exitButton.SetActive(false);
			}
		}
		else
		{
			Log.Error("[ItemsMixerSelectSlotState] Cannot play ItemsMixer gacha. Not enough Inputs or currency.", base.gameObject);
			Log.Error(("SelectedPrize: " + base.ItemsMixer.selectedPrize != null) ? "True" : (("False. ItemToSpend: " + itemToSpend != null) ? "True" : (("False. SelectedSlot: " + selectedSlot == null) ? "False" : "True")));
		}
	}

	private void OnBuyItemsMixerResponse(GachaResult gachaResult)
	{
		ItemCollectionDataModel.Item item = gachaResult.itemCollection.items[0];
		if (item == null)
		{
			Log.Error("ItemsMixer item result is null", base.gameObject);
		}
		base.ItemsMixer.SetResult(item, gachaResult.slot);
	}

	private void ToggleBoardPosition(tk2dUIItem button)
	{
		if (base.ItemsMixer.CurrentState == GachaPlinkoStates.SELECT_SLOT)
		{
			float newLocalYPosition = ((!boardOnTop) ? base.ItemsMixer.finalBoardYPosition : base.ItemsMixer.initialBoardYPosition);
			itemMixerBoard.transform.TweenLocalYPosition(newLocalYPosition, 1f);
			boardOnTop = !boardOnTop;
			SetLabelsState(boardOnTop);
		}
	}

	private void SetLabelsState(bool state)
	{
		if ((bool)previewLabel)
		{
			previewLabel.gameObject.SetActive(state);
		}
		if ((bool)slotsLabel)
		{
			slotsLabel.gameObject.SetActive(!state);
		}
	}

	protected void InitDragDropItem(DragDropItem cell)
	{
		cell.UIItem.enabled = true;
		cell.OnDragStart += HandleOnDragStart;
		cell.OnDragEnd += HandleOnDragEnd;
		cell.OnDropTargetChanged += HandleOnDropTargetChanged;
		cell.OnTap += HandleCellClicked;
	}

	protected void ApplyDragEffect(DragDropItem dragItem, float duration = 0.4f, EaseType easeType = EaseType.EaseOutExpo)
	{
		dragEffectTween = dragItem.transform.TweenLocalScale(1.1f, duration, easeType);
		dragItem.gameObject.SetLayerRecursively(dragLayer);
		ItemsMixerPriceItemCell component = dragItem.GetComponent<ItemsMixerPriceItemCell>();
		if ((bool)component)
		{
			component.SetDraggingState(true, dragItem.GetScreenPos());
		}
	}

	protected void ResetDragEffect(DragDropItem dragItem, float duration = 0.4f, EaseType easeType = EaseType.EaseOutExpo, bool setScale = true)
	{
		if (setScale)
		{
			float x = pricesScrollableArea.cellsScale.x;
			dragEffectTween = dragItem.transform.TweenLocalScale(x, duration, easeType);
		}
		dragItem.gameObject.SetLayerRecursively(normalLayer);
	}

	protected void HandleOnDragStart(DragDropItem dragItem)
	{
		pricesScrollableArea.ScrollableArea.CancelScroll();
		currentDragItem = dragItem;
		ApplyDragEffect(dragItem);
	}

	protected void HandleOnDragEnd(DragDropItem dragItem)
	{
		ItemsMixerPriceItemCell component = dragItem.transform.GetComponent<ItemsMixerPriceItemCell>();
		if (dragItem.DropTarget != null)
		{
			selectedSlot = null;
			itemToSpend = null;
			selectedSlot = dragItem.DropTarget.GetComponent<ItemsMixerPlayerSlotViewController>();
			if ((bool)component)
			{
				itemToSpend = component.Item;
				component.SetDraggingState(false, Vector3.zero, true);
				component.SetContentState(false);
				component.SetChipColliderState(true);
			}
			selectedDragItem = dragItem;
			selectedTarget = dragItem.DropTarget.transform;
			SetCellToTarget(selectedDragItem, selectedTarget, true);
			GachaPoolsDataModel prizeGachaPoolsDataModel = base.ItemsMixer.gachaResultsPayload.prizeGachaPoolsDataModel;
			PopupManager.ShowPopup(PopupDataModel.PriceConfirmationPopUp(prizeGachaPoolsDataModel.GetPrice(), "ui_prizegacha_confirmation_title".Localize("Confirmation"), "ui_prizegacha_itemsmixer_confirmation_text".Localize("DO YOU WANT TO DROP N' SWAP?"), PlayItemsMixerGacha, null, RestoreCell));
			AudioTrigger.SwapAbilities.Play();
			AudioTrigger.CrowdGasp.Play();
		}
		else
		{
			component.SetChipColliderState(false);
			RestoreCell();
		}
	}

	private void PlayItemsMixerGacha()
	{
		GachaPoolsDataModel prizeGachaPoolsDataModel = base.ItemsMixer.gachaResultsPayload.prizeGachaPoolsDataModel;
		if (!UserProfile.player.CanAfford(prizeGachaPoolsDataModel.GetPrice()))
		{
			PopupManager.ShowPopup(PopupDataModel.NoYes("ui_not_enough_gems_title".Localize("Not enough gems"), "ui_not_enough_gems_desc".Localize("Do you want to go buy more gems?"), delegate
			{
				PopupManager.DestroyAllPopups();
				TopBarController.instance.LoadShop();
				RestoreCell();
			}, RestoreCell, RestoreCell));
			return;
		}
		SetCellToTarget(selectedDragItem, selectedTarget, true);
		selectedDragItem.collider.enabled = false;
		selectedDragItem.transform.parent = selectedTarget;
		selectedDragItem.transform.localScale = Vector3.one * chipScale;
		base.ItemsMixer.playerSlotChip = selectedDragItem.gameObject;
		selectedDragItem = null;
		selectedTarget = null;
		PlayItemsMixerRequest();
		if ((bool)toggleButton)
		{
			toggleButton.SetActive(false);
		}
		base.ItemsMixer.SetState(GachaPlinkoStates.IN_PROGRESS);
	}

	private void RestoreCell()
	{
		if (!(currentDragItem == null))
		{
			ItemsMixerPriceItemCell component = currentDragItem.transform.GetComponent<ItemsMixerPriceItemCell>();
			if ((bool)component)
			{
				component.SetDraggingState(false, initialCellStateLocalPosition, true);
			}
			SnapCellBack(currentDragItem);
			currentDragItem = null;
		}
	}

	protected void HandleOnDropTargetChanged(DragDropItem dragItem)
	{
		if ((bool)previousDropTargetSlot)
		{
			previousDropTargetSlot.RestoreArrowColor();
			previousDropTargetSlot = null;
		}
		ItemsMixerPriceItemCell component = dragItem.transform.GetComponent<ItemsMixerPriceItemCell>();
		if (dragItem.DropTarget != null)
		{
			Log.DebugTag("Drag item  " + dragItem.DropTarget, dragItem.DropTarget, "DropNSwap");
			ItemsMixerPlayerSlotViewController component2 = dragItem.DropTarget.GetComponent<ItemsMixerPlayerSlotViewController>();
			if (component == null)
			{
				Log.DebugTag("Drag Cell null", null, "DropNSwap");
			}
			if (component2 == null)
			{
				Log.DebugTag("Drop Target Slot null", null, "DropNSwap");
			}
			component.SetChipColor(component2.slotColor);
			component2.SetArrowColor();
			previousDropTargetSlot = component2;
			AudioTrigger.UIButtonSoft.Play();
		}
		else
		{
			component.SetChipColor(Color.white);
		}
	}

	protected void HandleCellClicked(DragDropItem dragItem)
	{
		ItemsMixerPriceItemCell component = dragItem.transform.GetComponent<ItemsMixerPriceItemCell>();
		if (component.Item.itemType == UserInventory.ItemType.Parts)
		{
			List<UnitPartsDataModel> multiByQuery = NonUnitySingleton<DMAccessManager>.instance.GetMultiByQuery<UnitPartsDataModel>(" WHERE part_type = " + component.Item.itemId);
			if (multiByQuery.Count > 0 && multiByQuery[0] != null)
			{
				PopupManager.ShowPopup(PopupDataModel.InspectUnitPopUp(UnitDataModel.GetSingle(multiByQuery[0].unitId), null));
				AudioTrigger.UIButtonSoft.Play();
			}
		}
	}

	protected void SetCellToTarget(DragDropItem dragItem, Transform target, bool immediate = false)
	{
		ResetDragEffect(dragItem, 0.4f, EaseType.EaseOutExpo, false);
		if (target != null)
		{
			if (immediate)
			{
				dragItem.transform.position = target.position;
			}
			else
			{
				dragItem.SnapToPosition(target.position + Vector3.back);
			}
		}
		ItemsMixerPriceItemCell component = dragItem.transform.GetComponent<ItemsMixerPriceItemCell>();
		if ((bool)component)
		{
			component.SetContentState(false);
		}
	}

	protected void SnapCellBack(DragDropItem dragItem)
	{
		if (!(dragItem == null))
		{
			ResetDragEffect(dragItem);
			dragItem.SnapToOriginalPosition();
		}
	}
}
