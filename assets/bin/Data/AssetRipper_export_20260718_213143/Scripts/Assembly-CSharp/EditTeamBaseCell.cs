using UnityEngine;

public class EditTeamBaseCell : ScrollableCell
{
	protected string CELL_BACKGROUND_INACTIVE_SPRITE_NAME = "Vehicle_backing_Not_selected";

	protected string CELL_BACKGROUND_ACTIVE_SPRITE_NAME = "Vehicle_backing";

	[SerializeField]
	private tk2dBaseSprite background;

	public EditTeamCellGroup teamCellGroup;

	private DragDropItem dragItem;

	public int index
	{
		get
		{
			return dataIndex;
		}
		set
		{
			dataIndex = value;
		}
	}

	public bool isTeamCell
	{
		get
		{
			return teamCellGroup != null;
		}
	}

	public tk2dBaseSprite Background
	{
		get
		{
			return background;
		}
	}

	public DragDropItem DragItem
	{
		get
		{
			return dragItem;
		}
	}

	public Vector3 OriginWorldPosition
	{
		get
		{
			if (isTeamCell)
			{
				return teamCellGroup.GetCellWorldPosition(index);
			}
			return controller.GetCellWorldPosition(index);
		}
	}

	private void Awake()
	{
		deactivateIfNull = true;
		dragItem = GetComponent<DragDropItem>();
	}

	public override void Init(ScrollableAreaController controller, object data, int index, float cellHeight = 0f, float cellWidth = 0f, ScrollableCell parentCell = null)
	{
		base.Init(controller, data, index, cellHeight, cellWidth);
	}

	public override void ConfigureCellData()
	{
		if (isTeamCell)
		{
			if (base.DataObject != null)
			{
				if ((bool)background)
				{
					background.gameObject.SetActive(true);
				}
			}
			else if ((bool)background)
			{
				background.gameObject.SetActive(false);
			}
		}
		DragItem.enabled = base.DataObject != null;
	}

	protected void SetBackgroundState(bool state)
	{
		if ((bool)background)
		{
			background.color = ((!state) ? new Color(0.874f, 0.243f, 0.031f) : Color.white);
		}
	}

	public void ResetWithData(object newData)
	{
		base.DataObject = newData;
		base.transform.position = OriginWorldPosition;
	}
}
