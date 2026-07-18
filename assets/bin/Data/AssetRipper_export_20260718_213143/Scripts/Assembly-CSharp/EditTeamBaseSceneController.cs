using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using UnityEngine;

public abstract class EditTeamBaseSceneController : SceneController, iSortableController
{
	public enum DropTargetType
	{
		None = 0,
		SwapTeamToTeam = 1,
		MoveTeamToTeam = 2,
		MoveTeamToList = 3,
		MoveListToTeam = 4,
		SwapListToTeam = 5
	}

	private class PreviewSlot
	{
		public int index;

		public float val;

		public float targetVal;

		public Tweener tweener;
	}

	protected const float DRAG_TIME = 0.15f;

	[SerializeField]
	protected ScrollableAreaController scrollableAreaController;

	[SerializeField]
	protected DragDropActionView dragDropActionView;

	[SerializeField]
	protected tk2dCamera sceneCamera;

	[SerializeField]
	protected EditTeamSortOptions sortOptions;

	public IList unassignedInventory;

	protected int normalLayer;

	protected int dragLayer;

	protected Tweener dragEffectTween;

	protected DragDropItem currentDragItem;

	protected DropTargetType dropTargetType;

	protected float dropTargetChangeTime;

	private PreviewSlot scrollableAreaPreview;

	private List<PreviewSlot> previewSlots = new List<PreviewSlot>();

	public ScrollableAreaController ScrollableAreaControllerInstance()
	{
		return scrollableAreaController;
	}

	public IList UnassignedInventory()
	{
		return unassignedInventory;
	}

	public override void Awake()
	{
		base.Awake();
		normalLayer = LayerMask.NameToLayer("UI 2D");
		dragLayer = LayerMask.NameToLayer("Drag");
		allowsBackButton = true;
	}

	protected virtual void Start()
	{
		SceneTransitionManager.readyToTransitionIn = true;
		Singleton<InitializationManager>.instance.ExecuteOnState(InitializationManager.State.OnlineReady, delegate
		{
			Init();
		});
	}

	protected virtual void Update()
	{
		if (dropTargetType == DropTargetType.MoveTeamToList)
		{
			int indexOfWorldPosition = scrollableAreaController.GetIndexOfWorldPosition(currentDragItem.transform.position);
			if (scrollableAreaPreview != null && indexOfWorldPosition != scrollableAreaPreview.index)
			{
				PreviewSlotOpen(scrollableAreaPreview.index, 0f);
			}
			scrollableAreaPreview = PreviewSlotOpen(indexOfWorldPosition, 1f);
		}
		else if (scrollableAreaPreview != null)
		{
			PreviewSlotOpen(scrollableAreaPreview.index, 0f);
			scrollableAreaPreview = null;
		}
		ApplyPreviewSlots();
	}

	protected virtual void Init()
	{
		InitInventory();
		scrollableAreaController.DataSource = unassignedInventory;
		InitSortOptions();
		InitTeamCells();
		foreach (DragDropItem cellComponent in scrollableAreaController.GetCellComponents<DragDropItem>())
		{
			InitDragDropItem(cellComponent);
		}
	}

	protected void InitDragDropItem(DragDropItem cell)
	{
		cell.OnDragStart += HandleOnDragStart;
		cell.OnDragEnd += HandleOnDragEnd;
		cell.OnDropTargetChanged += HandleOnDropTargetChanged;
		cell.OnTap += HandleCellClicked;
	}

	protected abstract void InitInventory();

	protected abstract void InitSortOptions();

	protected abstract void InitTeamCells();

	protected abstract void PlaySound();

	protected void ApplyDragEffect(DragDropItem dragItem, float duration = 0.4f, EaseType easeType = EaseType.EaseOutExpo)
	{
		dragEffectTween = dragItem.transform.TweenLocalScale(1f, duration, easeType);
		dragItem.gameObject.SetLayerRecursively(dragLayer);
	}

	protected void ResetDragEffect(DragDropItem dragItem, float duration = 0.4f, EaseType easeType = EaseType.EaseOutExpo)
	{
		EditTeamBaseCell component = dragItem.GetComponent<EditTeamBaseCell>();
		float scaleValue = ((!component.isTeamCell) ? ScrollableAreaControllerInstance().cellsScale.x : 0.9f);
		dragEffectTween = dragItem.transform.TweenLocalScale(scaleValue, duration, easeType);
		dragItem.gameObject.SetLayerRecursively(normalLayer);
	}

	protected void HandleOnDragStart(DragDropItem dragItem)
	{
		scrollableAreaController.ScrollableArea.CancelScroll();
		currentDragItem = dragItem;
		ApplyDragEffect(dragItem);
	}

	protected void HandleOnDragEnd(DragDropItem dragItem)
	{
		StartCoroutine(DragResultAnimation(dragItem));
		currentDragItem = null;
	}

	protected void HandleOnDropTargetChanged(DragDropItem dragItem)
	{
		dropTargetChangeTime = Time.time;
		dropTargetType = ClassifyDropTarget(dragItem, dragItem.DropTarget);
		dragDropActionView.Configure(dragItem, GetDropTargetActionString(dropTargetType));
		EditTeamBaseCell component = dragItem.GetComponent<EditTeamBaseCell>();
		if (dropTargetType == DropTargetType.SwapListToTeam || dropTargetType == DropTargetType.SwapTeamToTeam)
		{
			component.Background.TweenAlpha(0.5f, 0.5f);
		}
		else
		{
			component.Background.TweenAlpha(1f, 0.5f);
		}
	}

	protected void HandleCellClicked(DragDropItem dragItem)
	{
		EditTeamBaseCell component = dragItem.GetComponent<EditTeamBaseCell>();
		if (component != null)
		{
			OnCellTapped(component);
		}
	}

	protected abstract void OnCellTapped(object data);

	public virtual void OnConfirm()
	{
		SaveToProfile();
		SceneTransitionManager.PopScene();
	}

	protected abstract void SaveToProfile();

	private PreviewSlot PreviewSlotOpen(int index, float targetValue, float duration = 0.5f)
	{
		PreviewSlot previewSlot = null;
		foreach (PreviewSlot previewSlot2 in previewSlots)
		{
			if (previewSlot2.index == index)
			{
				previewSlot = previewSlot2;
			}
		}
		if (previewSlot == null)
		{
			previewSlot = new PreviewSlot();
			previewSlot.index = index;
			previewSlots.Add(previewSlot);
		}
		if (previewSlot.targetVal == targetValue)
		{
			return previewSlot;
		}
		if (previewSlot.tweener != null)
		{
			previewSlot.tweener.Kill();
			previewSlot.tweener = null;
		}
		previewSlot.targetVal = targetValue;
		duration *= Mathf.Abs(targetValue - previewSlot.val);
		if (duration == 0f)
		{
			previewSlot.val = targetValue;
		}
		else
		{
			previewSlot.tweener = HOTween.To(previewSlot, duration, new TweenParms().NewProp("val", targetValue).Ease(EaseType.EaseOutExpo));
		}
		return previewSlot;
	}

	private void ApplyPreviewSlots()
	{
		if (previewSlots.Count == 0)
		{
			return;
		}
		for (int i = 0; i < previewSlots.Count; i++)
		{
			PreviewSlot previewSlot = previewSlots[i];
			if (previewSlot.val == 0f && (previewSlot.tweener == null || previewSlot.tweener.isComplete))
			{
				previewSlots.RemoveAt(i);
				i--;
			}
		}
		if (previewSlots.Count == 0)
		{
			return;
		}
		foreach (EditTeamBaseCell cellComponent in scrollableAreaController.GetCellComponents<EditTeamBaseCell>())
		{
			Vector3 cellLocalPosition = scrollableAreaController.GetCellLocalPosition(cellComponent.index);
			foreach (PreviewSlot previewSlot2 in previewSlots)
			{
				if (cellComponent.index >= previewSlot2.index)
				{
					cellLocalPosition.x += scrollableAreaController.cellWidth * previewSlot2.val;
				}
			}
			cellComponent.transform.localPosition = cellLocalPosition;
		}
	}

	protected IEnumerator DragResultAnimation(DragDropItem dragItem)
	{
		if (dragEffectTween != null)
		{
			dragEffectTween.Kill();
		}
		DragDropItem targetItem = ((!(dragItem.DropTarget != null)) ? null : dragItem.DropTarget.GetComponent<DragDropItem>());
		bool targetLocked = targetItem != null && targetItem.Locked;
		EditTeamBaseCell dragCell = dragItem.GetComponent<EditTeamBaseCell>();
		EditTeamBaseCell targetCell = ((!(targetItem != null)) ? null : targetItem.GetComponent<EditTeamBaseCell>());
		DropTargetType dropType = ClassifyDropTarget(dragItem, dragItem.DropTarget);
		if (targetLocked)
		{
			DragTargetLocked(targetItem);
			dropType = DropTargetType.None;
		}
		switch (dropType)
		{
		case DropTargetType.None:
			yield return StartCoroutine(SnapCellBack(dragCell));
			break;
		case DropTargetType.SwapListToTeam:
			PlaySound();
			yield return StartCoroutine(SwapCellsFromList(dragCell, targetCell));
			break;
		case DropTargetType.SwapTeamToTeam:
		case DropTargetType.MoveTeamToTeam:
			PlaySound();
			yield return StartCoroutine(SwapCells(dragCell, targetCell));
			break;
		case DropTargetType.MoveListToTeam:
			PlaySound();
			yield return StartCoroutine(MoveCellListToTeam(dragCell, targetCell));
			break;
		case DropTargetType.MoveTeamToList:
			yield return StartCoroutine(MoveCellTeamToList(dragCell));
			break;
		}
	}

	protected virtual void DragTargetLocked(DragDropItem targetItem)
	{
	}

	protected IEnumerator SnapCellBack(EditTeamBaseCell dragCell)
	{
		ResetDragEffect(dragCell.DragItem);
		dragCell.DragItem.SnapToOriginalPosition();
		yield return new WaitForSeconds(0.2f);
	}

	protected IEnumerator SwapCellsFromList(EditTeamBaseCell dragCell, EditTeamBaseCell targetCell)
	{
		if (!dragCell.isTeamCell)
		{
			unassignedInventory[dragCell.index] = targetCell.DataObject;
		}
		if (!targetCell.isTeamCell)
		{
			unassignedInventory[targetCell.index] = dragCell.DataObject;
		}
		SwapCellProperties(dragCell, targetCell);
		StartCoroutine(MoveCellBackToList(dragCell));
		targetCell.transform.TweenPosition(targetCell.OriginWorldPosition, 0.15f, EaseType.EaseInOutExpo);
		ResetDragEffect(dragCell.DragItem);
		yield return new WaitForSeconds(0.15f);
		ResetDragEffect(targetCell.DragItem);
		yield return new WaitForSeconds(0.4f);
	}

	protected IEnumerator SwapCells(EditTeamBaseCell dragCell, EditTeamBaseCell targetCell)
	{
		if (!dragCell.isTeamCell)
		{
			unassignedInventory[dragCell.index] = targetCell.DataObject;
		}
		if (!targetCell.isTeamCell)
		{
			unassignedInventory[targetCell.index] = dragCell.DataObject;
		}
		SwapCellProperties(dragCell, targetCell);
		dragCell.transform.TweenPosition(dragCell.OriginWorldPosition, 0.15f, EaseType.EaseInOutExpo);
		targetCell.transform.TweenPosition(targetCell.OriginWorldPosition, 0.15f, EaseType.EaseInOutExpo);
		ResetDragEffect(dragCell.DragItem);
		yield return new WaitForSeconds(0.15f);
		ResetDragEffect(targetCell.DragItem);
		yield return new WaitForSeconds(0.4f);
	}

	protected void SwapCellProperties(EditTeamBaseCell cellOne, EditTeamBaseCell cellTwo)
	{
		Vector3 localScale = cellOne.transform.localScale;
		Vector3 position = cellOne.transform.position;
		int layer = cellOne.gameObject.layer;
		object dataObject = cellOne.DataObject;
		Vector3 localScale2 = cellTwo.transform.localScale;
		Vector3 position2 = cellTwo.transform.position;
		int layer2 = cellTwo.gameObject.layer;
		object dataObject2 = cellTwo.DataObject;
		cellOne.ResetWithData(dataObject2);
		cellOne.transform.position = position2;
		cellOne.transform.localScale = localScale2;
		cellOne.gameObject.SetLayerRecursively(layer2);
		cellTwo.ResetWithData(dataObject);
		cellTwo.transform.position = position;
		cellTwo.transform.localScale = localScale;
		cellTwo.gameObject.SetLayerRecursively(layer);
	}

	protected IEnumerator MoveCellTeamToList(EditTeamBaseCell dragCell)
	{
		ResetDragEffect(dragCell.DragItem);
		int targetIndex = scrollableAreaController.GetIndexOfWorldPosition(dragCell.transform.position);
		scrollableAreaController.ScrollableArea.enabled = false;
		previewSlots.Clear();
		unassignedInventory.Insert(targetIndex, dragCell.DataObject);
		scrollableAreaController.OnDataChanged();
		EditTeamBaseCell targetCell = null;
		foreach (EditTeamBaseCell cell in scrollableAreaController.GetCellComponents<EditTeamBaseCell>())
		{
			if (cell.DataObject == dragCell.DataObject)
			{
				targetCell = cell;
				break;
			}
		}
		if (targetCell != null)
		{
			targetCell.transform.localScale = dragCell.transform.localScale;
			targetCell.transform.position = dragCell.transform.position;
			targetCell.transform.TweenPosition(targetCell.OriginWorldPosition, 0.4f);
			ResetDragEffect(targetCell.DragItem);
		}
		dragCell.ResetWithData(null);
		yield return new WaitForSeconds(0.3f);
		scrollableAreaController.ScrollableArea.enabled = true;
	}

	protected IEnumerator MoveCellListToTeam(EditTeamBaseCell dragCell, EditTeamBaseCell targetCell)
	{
		SwapCellProperties(dragCell, targetCell);
		dragCell.transform.position = dragCell.OriginWorldPosition;
		dragCell.transform.localScale = ScrollableAreaControllerInstance().cellsScale;
		unassignedInventory.RemoveAt(dragCell.index);
		scrollableAreaController.OnDataChanged();
		targetCell.transform.TweenPosition(targetCell.OriginWorldPosition, 0.075f, EaseType.EaseInOutExpo);
		yield return new WaitForSeconds(0.075f);
		ResetDragEffect(targetCell.DragItem, 0.5f);
		yield return new WaitForSeconds(0.5f);
	}

	private IEnumerator MoveCellBackToList(EditTeamBaseCell dragCell)
	{
		Vector3 startPosition = dragCell.transform.position;
		AnimationCurve easeInCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
		for (float timer = 0f; timer < 0.15f; timer += Time.deltaTime)
		{
			dragCell.transform.position = Vector3.Lerp(startPosition, scrollableAreaController.GetCellWorldPosition(dragCell.index), easeInCurve.Evaluate(timer / 0.15f));
			yield return new WaitForEndOfFrame();
		}
		dragCell.transform.position = scrollableAreaController.GetCellWorldPosition(dragCell.index);
	}

	protected Vector3 WorldPositionAtListIndex(int index)
	{
		return scrollableAreaController.GetCellWorldPosition(index);
	}

	protected Vector3 KeepWithinScreen(Vector3 worldPos)
	{
		Vector3 position = sceneCamera.camera.WorldToScreenPoint(worldPos);
		if (position.x <= -200f)
		{
			position.x = -200f;
		}
		if (position.x >= sceneCamera.ScreenExtents.width + 200f)
		{
			position.x = sceneCamera.ScreenExtents.width + 200f;
		}
		return sceneCamera.camera.ScreenToWorldPoint(position);
	}

	protected DropTargetType ClassifyDropTarget(DragDropItem dragItem, DragDropTarget dropTarget)
	{
		if (dropTarget == null)
		{
			return DropTargetType.None;
		}
		EditTeamBaseCell component = dragItem.GetComponent<EditTeamBaseCell>();
		EditTeamBaseCell component2 = dropTarget.GetComponent<EditTeamBaseCell>();
		if (dropTarget.gameObject == scrollableAreaController.gameObject && component.isTeamCell)
		{
			return DropTargetType.MoveTeamToList;
		}
		if (component2 == null)
		{
			return DropTargetType.None;
		}
		bool isTeamCell = component.isTeamCell;
		bool isTeamCell2 = component2.isTeamCell;
		if (isTeamCell && isTeamCell2)
		{
			if (component2.DataObject != null)
			{
				return DropTargetType.SwapTeamToTeam;
			}
			return DropTargetType.MoveTeamToTeam;
		}
		if (isTeamCell && !isTeamCell2)
		{
			return DropTargetType.MoveTeamToList;
		}
		if (!isTeamCell && isTeamCell2)
		{
			if (component2.DataObject != null)
			{
				return DropTargetType.SwapListToTeam;
			}
			return DropTargetType.MoveListToTeam;
		}
		return DropTargetType.None;
	}

	protected string GetDropTargetActionString(DropTargetType dropType)
	{
		switch (dropType)
		{
		case DropTargetType.None:
			return string.Empty;
		case DropTargetType.SwapTeamToTeam:
		case DropTargetType.SwapListToTeam:
			return "ui_editteam_swap".Localize("Swap");
		case DropTargetType.MoveTeamToTeam:
		case DropTargetType.MoveTeamToList:
		case DropTargetType.MoveListToTeam:
			return "ui_editteam_move".Localize("Move");
		default:
			return string.Empty;
		}
	}
}
