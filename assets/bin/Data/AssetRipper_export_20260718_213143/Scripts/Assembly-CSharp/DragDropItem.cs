using System;
using UnityEngine;

[RequireComponent(typeof(tk2dUIItem))]
public class DragDropItem : MonoBehaviour
{
	public enum DragState
	{
		NotDragging = 0,
		TryDragStart = 1,
		Dragging = 2
	}

	private tk2dUIItem uiItem;

	private DragState dragState;

	private float dragStartTime;

	private float dragMaxDist;

	private Vector3 dragStartScreenPos;

	private Vector3 dragStartPosition;

	private Vector3 dragStartLocalPosition;

	private DragDropTarget _dropTarget;

	private bool _locked;

	[SerializeField]
	private float dragMinAngle = -360f;

	[SerializeField]
	private float dragMaxAngle = 360f;

	[SerializeField]
	private float dragStartRadius = 10f;

	[SerializeField]
	private float tapAndHoldDragSeconds = 0.7f;

	[SerializeField]
	private bool verticalAutoDrag;

	[SerializeField]
	private float autoDragThreshold = -40f;

	public DragDropTarget DropTarget
	{
		get
		{
			return _dropTarget;
		}
		set
		{
			DragDropTarget dropTarget = _dropTarget;
			_dropTarget = value;
			if (_dropTarget != dropTarget && this.OnDropTargetChanged != null)
			{
				this.OnDropTargetChanged(this);
			}
		}
	}

	public float DragMinAngle
	{
		get
		{
			return dragMinAngle;
		}
		set
		{
			dragMinAngle = value;
		}
	}

	public float DragMaxAngle
	{
		get
		{
			return dragMaxAngle;
		}
		set
		{
			dragMaxAngle = value;
		}
	}

	public Vector3 OriginWorldPosition
	{
		get
		{
			return dragStartPosition;
		}
	}

	public tk2dUIItem UIItem
	{
		get
		{
			return uiItem;
		}
	}

	public bool IsDragging
	{
		get
		{
			return dragState == DragState.Dragging;
		}
	}

	public bool Locked
	{
		get
		{
			return _locked;
		}
		set
		{
			_locked = value;
		}
	}

	public event Action<DragDropItem> OnDragStart;

	public event Action<DragDropItem> OnDragEnd;

	public event Action<DragDropItem> OnDropTargetChanged;

	public event Action<DragDropItem> OnTap;

	public void SnapToOriginalPosition(float duration = 0.2f)
	{
		base.transform.TweenLocalPosition(dragStartLocalPosition, duration);
	}

	public void SnapToPosition(Vector3 position, float duration = 0.2f)
	{
		base.transform.TweenPosition(position, duration);
	}

	private void Awake()
	{
		uiItem = GetComponent<tk2dUIItem>();
	}

	private void OnEnable()
	{
		uiItem.OnDown += HandleOnDown;
		uiItem.OnRelease += HandleOnRelease;
	}

	private void OnDisable()
	{
		uiItem = GetComponent<tk2dUIItem>();
		uiItem.OnDown -= HandleOnDown;
		uiItem.OnRelease -= HandleOnRelease;
	}

	private void Update()
	{
		if (dragState == DragState.TryDragStart)
		{
			Vector2 a = GetScreenPos();
			float num = Vector2.Distance(a, dragStartScreenPos);
			float num2 = Mathf.Atan2(a.y - dragStartScreenPos.y, a.x - dragStartScreenPos.x) * 57.29578f;
			float num3 = Time.time - dragStartTime;
			dragMaxDist = Mathf.Max(num, dragMaxDist);
			if (dragMaxDist < dragStartRadius && num3 > tapAndHoldDragSeconds)
			{
				BeginDragging();
			}
			if (num > dragStartRadius && num2 > dragMinAngle && num2 < dragMaxAngle && dragMaxDist < 30f)
			{
				BeginDragging();
			}
			float num4 = a.y - dragStartScreenPos.y;
			if (verticalAutoDrag && num4 >= autoDragThreshold)
			{
				BeginDragging();
			}
		}
		if (dragState == DragState.Dragging)
		{
			Vector3 screenPos = GetScreenPos();
			base.transform.position = dragStartPosition + (screenPos - dragStartScreenPos);
			DropTarget = GetDropTarget();
		}
	}

	private void TryStartDragging()
	{
		dragState = DragState.TryDragStart;
		dragStartTime = Time.time;
		dragStartScreenPos = GetScreenPos();
		dragStartPosition = base.transform.position;
		dragMaxDist = 0f;
	}

	private void BeginDragging()
	{
		if (!_locked && PopupManager.PopupCount <= 0 && dragState != DragState.Dragging)
		{
			dragStartLocalPosition = base.transform.localPosition;
			dragState = DragState.Dragging;
			if (this.OnDragStart != null)
			{
				this.OnDragStart(this);
			}
		}
	}

	private void EndDragging()
	{
		if (this.OnDragEnd != null)
		{
			this.OnDragEnd(this);
		}
		DropTarget = null;
	}

	public Vector3 GetScreenPos()
	{
		Vector2 position = uiItem.Touch.position;
		Camera uICameraForControl = tk2dUIManager.Instance.GetUICameraForControl(base.gameObject);
		Vector3 result = uICameraForControl.ScreenToWorldPoint(new Vector3(position.x, position.y, base.transform.position.z - uICameraForControl.transform.position.z));
		result.z = base.transform.position.z;
		return result;
	}

	private DragDropTarget GetDropTarget()
	{
		Vector3 screenPos = GetScreenPos();
		screenPos.z = -1000f;
		DragDropTarget result = null;
		float num = float.MaxValue;
		DragDropTarget[] array = UnityEngine.Object.FindObjectsOfType<DragDropTarget>();
		foreach (DragDropTarget dragDropTarget in array)
		{
			if (dragDropTarget.gameObject == base.gameObject)
			{
				continue;
			}
			Collider collider = dragDropTarget.collider;
			if (collider != null)
			{
				Ray ray = new Ray(screenPos, Vector3.forward);
				RaycastHit hitInfo;
				if (collider.Raycast(ray, out hitInfo, float.MaxValue) && hitInfo.distance < num)
				{
					result = dragDropTarget;
					num = hitInfo.distance;
				}
			}
		}
		return result;
	}

	private void HandleOnDown()
	{
		if (dragState == DragState.NotDragging)
		{
			TryStartDragging();
		}
	}

	private void HandleOnRelease()
	{
		if (dragState == DragState.TryDragStart)
		{
			if (dragMaxDist < 10f && this.OnTap != null)
			{
				this.OnTap(this);
			}
		}
		else if (dragState == DragState.Dragging)
		{
			EndDragging();
		}
		dragState = DragState.NotDragging;
	}

	private void OnDrawGizmosSelected()
	{
		if (base.enabled)
		{
			Gizmos.color = Color.black;
			Gizmos.DrawWireSphere(base.transform.position, dragStartRadius);
			Gizmos.color = Color.red;
			Gizmos.DrawLine(base.transform.position, base.transform.position + Quaternion.AngleAxis(dragMinAngle, Vector3.forward) * (Vector3.right * 100f));
			Gizmos.DrawLine(base.transform.position, base.transform.position + Quaternion.AngleAxis(dragMaxAngle, Vector3.forward) * (Vector3.right * 100f));
		}
	}
}
