using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Holoville.HOTween;
using UnityEngine;

public class ItemsMixerSelectItemState : GachaPlinkoBaseState
{
	[SerializeField]
	private GameObject instructionsBubble;

	[SerializeField]
	private GameObject bouncingArrow;

	private readonly Vector3 initialCellStateLocalPosition = new Vector3(-100f, 0f, 0f);

	private int itemsMixerID = -1;

	private string partFilterID;

	[SerializeField]
	private GameObject prizesScrollableAreaGameObject;

	public float initialPrizesScrollableAreaPosition = -265f;

	private ScrollableAreaController prizesScrollableArea;

	protected DragDropItem currentDragItem;

	protected DragDropItem selectedDragItem;

	protected Transform selectedTarget;

	protected int normalLayer;

	protected int dragLayer;

	protected Tweener dragEffectTween;

	public override IEnumerator StartStateSequence()
	{
		GachaRewardsSceneModel gachaRewards = base.ItemsMixer.gachaResultsPayload;
		partFilterID = base.ItemsMixer.partFilterID;
		if (gachaRewards != null && gachaRewards.prizeGachaPoolsDataModel != null)
		{
			itemsMixerID = gachaRewards.prizeGachaPoolsDataModel.eventId;
			SetupPrizesScrollableArea();
			normalLayer = LayerMask.NameToLayer("UI 2D");
			dragLayer = LayerMask.NameToLayer("Drag");
			yield return StartCoroutine(SetupPrizesSequence());
			if ((bool)instructionsBubble)
			{
				instructionsBubble.SetActive(true);
				instructionsBubble.transform.localScale = Vector3.zero;
				instructionsBubble.transform.TweenLocalScale(1f, 1f);
			}
			if ((bool)bouncingArrow)
			{
				bouncingArrow.SetActive(true);
			}
		}
	}

	private IEnumerator SetupPrizesSequence()
	{
		List<GachaPlinkoPrizesDataModel> itemMixerPrizes = GachaPlinkoPrizesDataModel.GetAllPrizes(itemsMixerID, partFilterID);
		itemMixerPrizes = itemMixerPrizes.OrderByDescending((GachaPlinkoPrizesDataModel x) => x.orderNumber).Reverse().ToList();
		itemMixerPrizes.Sort(delegate(GachaPlinkoPrizesDataModel x, GachaPlinkoPrizesDataModel y)
		{
			ItemCollectionDataModel.Item item = new ItemCollectionDataModel.Item((UserInventory.ItemType)x.itemType, x.itemId, 1);
			ItemCollectionDataModel.Item item2 = new ItemCollectionDataModel.Item((UserInventory.ItemType)y.itemType, y.itemId, 1);
			bool flag = UserProfile.player.CanAffordItem(item);
			bool flag2 = UserProfile.player.CanAffordItem(item2);
			if (!flag && flag2)
			{
				return 1;
			}
			return (flag && !flag2) ? (-1) : (x.orderNumber - y.orderNumber);
		});
		if ((bool)prizesScrollableArea)
		{
			prizesScrollableArea.InitializeWithData(itemMixerPrizes);
		}
		foreach (DragDropItem cell in prizesScrollableArea.GetCellComponents<DragDropItem>(true))
		{
			InitDragDropItem(cell);
		}
		prizesScrollableAreaGameObject.transform.TweenLocalXPosition(initialPrizesScrollableAreaPosition, 1.25f);
		yield return new WaitForSeconds(1.25f);
	}

	private void SetupPrizesScrollableArea()
	{
		if ((bool)prizesScrollableAreaGameObject)
		{
			prizesScrollableArea = prizesScrollableAreaGameObject.GetComponent<ScrollableAreaController>();
			if (!prizesScrollableArea)
			{
				Log.Error("[ItemsMixerScene] ScrollableAreaController not found", base.gameObject);
			}
		}
	}

	protected void InitDragDropItem(DragDropItem cell)
	{
		cell.OnDragStart += HandleOnDragStart;
		cell.OnDragEnd += HandleOnDragEnd;
		cell.OnDropTargetChanged += HandleOnDropTargetChanged;
		cell.OnTap += HandleCellClicked;
	}

	protected void ApplyDragEffect(DragDropItem dragItem, float duration = 0.4f, EaseType easeType = EaseType.EaseOutExpo)
	{
		dragEffectTween = dragItem.transform.TweenLocalScale(1.1f, duration, easeType);
		dragItem.gameObject.SetLayerRecursively(dragLayer);
		ItemsMixerRewardItemCell component = dragItem.GetComponent<ItemsMixerRewardItemCell>();
		if ((bool)component)
		{
			component.SetDraggingState(true, dragItem.GetScreenPos());
		}
	}

	protected void ResetDragEffect(DragDropItem dragItem, float duration = 0.4f, EaseType easeType = EaseType.EaseOutExpo)
	{
		float x = prizesScrollableArea.cellsScale.x;
		dragEffectTween = dragItem.transform.TweenLocalScale(x, duration, easeType);
		dragItem.gameObject.SetLayerRecursively(normalLayer);
	}

	protected void HandleOnDragStart(DragDropItem dragItem)
	{
		prizesScrollableArea.ScrollableArea.CancelScroll();
		currentDragItem = dragItem;
		ApplyDragEffect(dragItem);
	}

	protected void HandleOnDragEnd(DragDropItem dragItem)
	{
		ItemsMixerRewardItemCell component = dragItem.GetComponent<ItemsMixerRewardItemCell>();
		if (dragItem.DropTarget != null)
		{
			if ((bool)component)
			{
				base.ItemsMixer.selectedPrize = component.ItemPrizeDataModel;
				component.SetDraggingState(false, Vector3.zero, true);
				component.SetChipState(false);
			}
			selectedDragItem = dragItem;
			selectedTarget = dragItem.DropTarget.transform;
			base.ItemsMixer.playerRewardCell = dragItem.gameObject;
			selectedTarget.collider.enabled = false;
			ConfigureTargetCell();
			AudioTrigger.SwapAbilities.Play();
		}
		else
		{
			RestoreCell();
		}
	}

	private void ConfigureTargetCell()
	{
		SetCellToTarget(selectedDragItem, selectedTarget);
		selectedDragItem.collider.enabled = false;
		selectedDragItem.transform.parent = selectedTarget;
		selectedDragItem = null;
		selectedTarget = null;
		if ((bool)bouncingArrow)
		{
			bouncingArrow.SetActive(false);
		}
		base.ItemsMixer.SetState(GachaPlinkoStates.TRANSITION_TO_SLOTS);
	}

	private void RestoreCell()
	{
		if (!(currentDragItem == null))
		{
			ItemsMixerRewardItemCell component = currentDragItem.GetComponent<ItemsMixerRewardItemCell>();
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
		ItemsMixerRewardItemCell component = dragItem.GetComponent<ItemsMixerRewardItemCell>();
		if (dragItem.DropTarget != null)
		{
			component.SetChipColor(Color.yellow);
			AudioTrigger.UIButtonSoft.Play();
		}
		else if ((bool)component)
		{
			component.SetChipColor(Color.white);
		}
	}

	protected void HandleCellClicked(DragDropItem dragItem)
	{
		ItemsMixerRewardItemCell component = dragItem.GetComponent<ItemsMixerRewardItemCell>();
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

	protected void SetCellToTarget(DragDropItem dragItem, Transform target)
	{
		ResetDragEffect(dragItem);
		if (target != null)
		{
			dragItem.SnapToPosition(target.position + Vector3.back);
		}
		ItemsMixerRewardItemCell component = dragItem.GetComponent<ItemsMixerRewardItemCell>();
		if ((bool)component)
		{
			component.SetContentState(false);
		}
	}

	protected void SnapCellBack(DragDropItem dragItem)
	{
		ResetDragEffect(dragItem);
		dragItem.SnapToOriginalPosition();
	}
}
