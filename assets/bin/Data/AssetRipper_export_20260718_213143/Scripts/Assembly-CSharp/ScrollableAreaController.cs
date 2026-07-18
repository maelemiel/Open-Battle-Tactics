using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(tk2dUIScrollableArea))]
public class ScrollableAreaController : MonoBehaviour
{
	public int NUMBER_OF_COLUMNS = 2;

	public float cellWidth = 220f;

	public float cellHeight = 200f;

	public ScrollableCell cellPrefab;

	public Transform listContentParent;

	public Vector2 padding = Vector2.zero;

	public float contentBottomPadding;

	private tk2dUIScrollableArea scrollableArea;

	private Transform containterCachedTransform;

	private float xOffset;

	private float yOffset;

	private IList allCellsData;

	private LinkedList<GameObject> localCellsPool = new LinkedList<GameObject>();

	private LinkedList<GameObject> cellsInUse = new LinkedList<GameObject>();

	private int previousInitialIndex;

	private int initialIndex;

	private int visibleCellsTotalCount;

	private int visibleCellsRowCount;

	public bool clampCellsWithinRange = true;

	public Vector3 cellsScale = Vector3.one;

	public List<float> adjustedSize = new List<float>();

	public List<float> adjustedPlacement = new List<float>();

	public Transform _leftAnchor;

	public Transform _rightAnchor;

	[SerializeField]
	private bool containsAnchoredCellElements;

	public tk2dUIScrollableArea ScrollableArea
	{
		get
		{
			return scrollableArea;
		}
	}

	public IList DataSource
	{
		get
		{
			return allCellsData;
		}
		set
		{
			if (allCellsData == value)
			{
				OnDataChanged();
			}
			else
			{
				InitializeWithData(value);
			}
		}
	}

	public ScrollAxis Axis
	{
		get
		{
			return (ScrollAxis)scrollableArea.scrollAxes;
		}
	}

	public LinkedList<GameObject> CellsInUse
	{
		get
		{
			return cellsInUse;
		}
	}

	public int InitialIndex
	{
		get
		{
			return initialIndex;
		}
	}

	public event Action OnDataChange;

	[ContextMenu("Reset Area")]
	private void ResetData()
	{
		OnDataChanged();
	}

	public void Awake()
	{
		if (_leftAnchor != null && _rightAnchor != null)
		{
			containsAnchoredCellElements = true;
		}
		if (!listContentParent)
		{
			Log.Warning("List content transform parent not found", base.gameObject);
		}
		scrollableArea = GetComponent<tk2dUIScrollableArea>();
		if (!scrollableArea)
		{
			Log.Warning("Scrollable area reference not found", base.gameObject);
		}
		else
		{
			containterCachedTransform = scrollableArea.contentContainer.transform;
		}
		xOffset = cellWidth * 0.5f;
		yOffset = cellHeight * 0.5f;
		if (NUMBER_OF_COLUMNS <= 0)
		{
			NUMBER_OF_COLUMNS = 1;
		}
		CalculateVisibleCellsCount();
		CreateCellPool((int)scrollableArea.scrollAxes);
	}

	private void Start()
	{
		Singleton<InitializationManager>.instance.ExecuteOnState(InitializationManager.State.OnlineReady, delegate
		{
			Init();
		});
	}

	private void Init()
	{
	}

	private void OnDestroy()
	{
		cellPrefab = null;
	}

	private void OnEnable()
	{
		Refresh();
	}

	public void Update()
	{
		if (allCellsData != null)
		{
			previousInitialIndex = initialIndex;
			CalculateCurrentIndex();
			InternalCellsUpdate();
		}
	}

	private void InternalCellsUpdate()
	{
		if (previousInitialIndex != initialIndex)
		{
			bool flag = previousInitialIndex < initialIndex;
			int num = Mathf.Abs(previousInitialIndex - initialIndex);
			int num2 = (flag ? 1 : (-1));
			for (int i = 1; i <= num; i++)
			{
				UpdateContent(previousInitialIndex + i * num2, flag);
			}
		}
	}

	public void OnDataChanged()
	{
		foreach (GameObject item in cellsInUse)
		{
			localCellsPool.AddLast(item);
		}
		cellsInUse.Clear();
		InitializeWithData(allCellsData);
		if (this.OnDataChange != null)
		{
			this.OnDataChange();
		}
	}

	private LinkedListNode<GameObject> GetCellFromPool(bool scrollingPositive)
	{
		if (localCellsPool.Count == 0)
		{
			Log.Error("Local Cells Pool is empty. This should not happen", base.gameObject);
			return null;
		}
		LinkedListNode<GameObject> first = localCellsPool.First;
		localCellsPool.RemoveFirst();
		if (scrollingPositive)
		{
			cellsInUse.AddLast(first);
		}
		else
		{
			cellsInUse.AddFirst(first);
		}
		return first;
	}

	private void FreeCell(bool scrollingPositive)
	{
		LinkedListNode<GameObject> linkedListNode = null;
		if (scrollingPositive)
		{
			linkedListNode = cellsInUse.First;
			cellsInUse.RemoveFirst();
			localCellsPool.AddLast(linkedListNode);
		}
		else
		{
			linkedListNode = cellsInUse.Last;
			cellsInUse.RemoveLast();
			localCellsPool.AddFirst(linkedListNode);
		}
	}

	private void CalculateVisibleCellsCount()
	{
		ScrollAxis scrollAxes = (ScrollAxis)scrollableArea.scrollAxes;
		BoxCollider component = scrollableArea.GetComponent<BoxCollider>();
		if ((bool)component)
		{
			StretchCollider component2 = scrollableArea.GetComponent<StretchCollider>();
			if ((bool)component2)
			{
				component2.Stretch();
			}
			scrollableArea.VisibleAreaLength = ((scrollAxes != ScrollAxis.HORIZONTAL) ? component.size.y : component.size.x);
		}
		switch (scrollAxes)
		{
		case ScrollAxis.HORIZONTAL:
			visibleCellsRowCount = Mathf.CeilToInt(scrollableArea.VisibleAreaLength / cellWidth);
			visibleCellsTotalCount = visibleCellsRowCount + 1;
			visibleCellsTotalCount *= NUMBER_OF_COLUMNS;
			break;
		case ScrollAxis.VERTICAL:
			visibleCellsRowCount = Mathf.CeilToInt(scrollableArea.VisibleAreaLength / cellHeight);
			visibleCellsTotalCount = visibleCellsRowCount + 1;
			visibleCellsTotalCount *= NUMBER_OF_COLUMNS;
			break;
		}
	}

	private void CreateCellPool(int axis)
	{
		GameObject gameObject = null;
		for (int i = 0; i < visibleCellsTotalCount; i++)
		{
			gameObject = InstantiateCell();
			localCellsPool.AddLast(gameObject);
		}
	}

	private void CalculateCurrentIndex()
	{
		switch ((ScrollAxis)scrollableArea.scrollAxes)
		{
		case ScrollAxis.HORIZONTAL:
			initialIndex = Mathf.FloorToInt((0f - (containterCachedTransform.localPosition.x + GetSizeAdjustment(DataSource.Count))) / cellWidth);
			break;
		case ScrollAxis.VERTICAL:
			initialIndex = Mathf.FloorToInt((containterCachedTransform.localPosition.y - GetSizeAdjustment(DataSource.Count) + padding.y) / cellHeight);
			break;
		}
		if (clampCellsWithinRange)
		{
			int num = Mathf.CeilToInt((float)allCellsData.Count / (float)NUMBER_OF_COLUMNS) - visibleCellsRowCount;
			if (initialIndex >= num)
			{
				initialIndex = num - 1;
			}
			if (initialIndex < 0)
			{
				initialIndex = 0;
			}
		}
	}

	public void InitializeWithData(IList cellDataList)
	{
		if (cellsInUse.Count > 0)
		{
			foreach (GameObject item in cellsInUse)
			{
				localCellsPool.AddLast(item);
			}
			cellsInUse.Clear();
		}
		if (cellDataList == null)
		{
			Log.Warning("Trying to populate a scrollable area with a null list");
			return;
		}
		allCellsData = cellDataList;
		ScrollAxis scrollAxes = (ScrollAxis)scrollableArea.scrollAxes;
		int num = 0;
		int num2 = 0;
		LinkedListNode<GameObject> linkedListNode = null;
		for (int i = 0; i < visibleCellsTotalCount; i++)
		{
			linkedListNode = GetCellFromPool(true);
			if (linkedListNode != null && !(linkedListNode.Value == null))
			{
				num2 = i + initialIndex;
				PositionCell(linkedListNode.Value, num2, scrollAxes);
				num++;
				if (num2 < cellDataList.Count)
				{
					InitCell(linkedListNode.Value, cellDataList[num2], num2);
				}
				else
				{
					InitCell(linkedListNode.Value, null, num2);
				}
			}
		}
		UpdateContentLength(cellDataList.Count, scrollAxes);
	}

	public void FixScrollLimits()
	{
		scrollableArea.ContentLength -= scrollableArea.VisibleAreaLength;
		scrollableArea.ContentLength -= padding.y;
		scrollableArea.VisibleAreaLength = 0.6f;
	}

	public List<T> GetCellComponents<T>(bool inChildren = false) where T : MonoBehaviour
	{
		List<T> list = new List<T>();
		T val = (T)null;
		foreach (GameObject item in cellsInUse)
		{
			val = (inChildren ? item.GetComponentInChildren<T>() : item.GetComponent<T>());
			if (val != null)
			{
				list.Add(val);
			}
		}
		return list;
	}

	private void UpdateContent(int cellIndex, bool scrollingPositive)
	{
		int num = ((!scrollingPositive) ? (cellIndex * NUMBER_OF_COLUMNS) : ((cellIndex - 1) * NUMBER_OF_COLUMNS + visibleCellsTotalCount));
		LinkedListNode<GameObject> linkedListNode = null;
		int num2 = 0;
		for (int i = 0; i < NUMBER_OF_COLUMNS; i++)
		{
			FreeCell(scrollingPositive);
			linkedListNode = GetCellFromPool(scrollingPositive);
			num2 = num + i;
			if (linkedListNode != null && !(linkedListNode.Value == null))
			{
				PositionCell(linkedListNode.Value, num + i, Axis);
				if (num2 >= 0 && num2 < allCellsData.Count)
				{
					InitCell(linkedListNode.Value, allCellsData[num2], num2);
				}
				else
				{
					InitCell(linkedListNode.Value, null, num2);
				}
			}
		}
	}

	public void Refresh()
	{
		foreach (GameObject item in cellsInUse)
		{
			if (item != null)
			{
				item.GetComponent<ScrollableCell>().ConfigureCellData();
			}
		}
	}

	private void UpdateContentLength(int cellsCounter, ScrollAxis axis)
	{
		switch (axis)
		{
		case ScrollAxis.HORIZONTAL:
			UpdateContentLengthHorizontally(cellsCounter);
			break;
		case ScrollAxis.VERTICAL:
			UpdateContentLengthVertically(cellsCounter);
			break;
		}
	}

	private void UpdateContentLengthHorizontally(int cellCounter)
	{
		scrollableArea.ContentLength = cellWidth * Mathf.Ceil((float)cellCounter / (float)NUMBER_OF_COLUMNS) + contentBottomPadding;
		scrollableArea.ContentLength += GetSizeAdjustment(DataSource.Count);
	}

	private void UpdateContentLengthVertically(int cellCounter)
	{
		scrollableArea.ContentLength = cellHeight * Mathf.Ceil((float)cellCounter / (float)NUMBER_OF_COLUMNS) + contentBottomPadding;
		scrollableArea.ContentLength += GetSizeAdjustment(DataSource.Count);
	}

	public void ContentToBottom()
	{
		Transform transform = scrollableArea.contentContainer.transform;
		Vector3 localPosition = transform.localPosition;
		float num = scrollableArea.ContentLength - scrollableArea.VisibleAreaLength;
		switch ((ScrollAxis)scrollableArea.scrollAxes)
		{
		case ScrollAxis.HORIZONTAL:
			transform.localPosition = new Vector3(0f - num, localPosition.y, localPosition.z);
			break;
		case ScrollAxis.VERTICAL:
			transform.localPosition = new Vector3(localPosition.x, num, localPosition.z);
			break;
		}
	}

	public void ContentToTop()
	{
		ContentToPosition(0f);
	}

	public void ContentToPosition(float position)
	{
		Transform transform = scrollableArea.contentContainer.transform;
		Vector3 localPosition = transform.localPosition;
		SetScrollPercentWithoutEvent(position / scrollableArea.ContentLength);
		switch ((ScrollAxis)scrollableArea.scrollAxes)
		{
		case ScrollAxis.HORIZONTAL:
			transform.localPosition = new Vector3(0f - position, localPosition.y, localPosition.z);
			break;
		case ScrollAxis.VERTICAL:
			transform.localPosition = new Vector3(localPosition.x, 0f - position, localPosition.z);
			break;
		}
	}

	public float GetContentPosition()
	{
		float result = 0f;
		switch ((ScrollAxis)scrollableArea.scrollAxes)
		{
		case ScrollAxis.HORIZONTAL:
			result = scrollableArea.contentContainer.transform.localPosition.x;
			break;
		case ScrollAxis.VERTICAL:
			result = scrollableArea.contentContainer.transform.localPosition.y;
			break;
		}
		return result;
	}

	public void SetScrollPercentWithoutEvent(float scrollPercent)
	{
		if ((bool)scrollableArea)
		{
			scrollableArea.SetScrollPercentWithoutEvent(scrollPercent);
		}
	}

	private GameObject InstantiateCell()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(cellPrefab.gameObject) as GameObject;
		gameObject.transform.parent = listContentParent;
		gameObject.transform.localScale = cellsScale;
		return gameObject;
	}

	private void InitCell(GameObject cell, object cellData, int index)
	{
		ScrollableCell component = cell.GetComponent<ScrollableCell>();
		if (containsAnchoredCellElements && (bool)component && component.GetAnchorRequirement())
		{
			component.SetAnchors(_leftAnchor, _rightAnchor);
		}
		if ((bool)component)
		{
			if (Axis == ScrollAxis.HORIZONTAL)
			{
				component.Init(this, cellData, index, cellHeight, cellWidth + GetCellsAdjustedSize(index));
			}
			else if (Axis == ScrollAxis.VERTICAL)
			{
				component.Init(this, cellData, index, cellHeight + GetCellsAdjustedSize(index), cellWidth);
			}
			component.ConfigureCell();
		}
	}

	public Vector3 GetCellLocalPosition(int cellIndex)
	{
		Vector3 zero = Vector3.zero;
		int num = cellIndex % NUMBER_OF_COLUMNS;
		if (Axis == ScrollAxis.HORIZONTAL)
		{
			zero.x = xOffset + (float)(cellIndex / NUMBER_OF_COLUMNS) * cellWidth;
			zero.x += GetSizeAdjustment(cellIndex);
			zero.y = 0f - yOffset - cellHeight * (float)num;
		}
		else if (Axis == ScrollAxis.VERTICAL)
		{
			zero.x = xOffset + cellWidth * (float)num;
			zero.y = 0f - yOffset - (float)(cellIndex / NUMBER_OF_COLUMNS) * cellHeight;
			zero.y -= GetSizeAdjustment(cellIndex);
		}
		return zero + new Vector3(padding.x, padding.y);
	}

	public float GetSizeAdjustment(int cellIndex)
	{
		float num = GetCellsAdjustedSize(cellIndex) / 2f;
		if (cellIndex < adjustedPlacement.Count)
		{
			if (Axis == ScrollAxis.HORIZONTAL)
			{
				num += adjustedPlacement[cellIndex] * cellWidth;
			}
			else if (Axis == ScrollAxis.VERTICAL)
			{
				num += adjustedPlacement[cellIndex] * cellHeight;
			}
		}
		return num;
	}

	private float GetCellsAdjustedSize(int cellIndex)
	{
		if (cellIndex < adjustedSize.Count)
		{
			if (Axis == ScrollAxis.HORIZONTAL)
			{
				return adjustedSize[cellIndex] * cellWidth;
			}
			if (Axis == ScrollAxis.VERTICAL)
			{
				return adjustedSize[cellIndex] * cellHeight;
			}
		}
		return 0f;
	}

	public Vector3 GetCellWorldPosition(int cellIndex)
	{
		Vector3 cellLocalPosition = GetCellLocalPosition(cellIndex);
		return scrollableArea.contentContainer.transform.TransformPoint(cellLocalPosition);
	}

	private void PositionCell(GameObject tempCell, int cellIndex, ScrollAxis axis)
	{
		tempCell.transform.localPosition = GetCellLocalPosition(cellIndex);
	}

	public int GetIndexOfWorldPosition(Vector3 worldPosition)
	{
		Vector3 vector = scrollableArea.contentContainer.transform.InverseTransformPoint(worldPosition);
		int a = -1;
		if (Axis == ScrollAxis.HORIZONTAL)
		{
			a = Mathf.FloorToInt(vector.x / cellWidth);
		}
		else if (Axis == ScrollAxis.VERTICAL)
		{
			a = Mathf.FloorToInt(vector.y / cellHeight);
		}
		return Mathf.Max(0, Mathf.Min(a, allCellsData.Count));
	}
}
