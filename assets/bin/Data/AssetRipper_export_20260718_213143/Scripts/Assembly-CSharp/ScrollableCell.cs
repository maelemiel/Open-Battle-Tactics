using UnityEngine;

public class ScrollableCell : MonoBehaviour
{
	protected ScrollableAreaController controller;

	protected object dataObject;

	protected int dataIndex;

	protected float cellHeight;

	protected float cellWidth;

	protected bool deactivateIfNull = true;

	protected ScrollableCell parentCell;

	public object DataObject
	{
		get
		{
			return dataObject;
		}
		set
		{
			dataObject = value;
			ConfigureCellData();
		}
	}

	public virtual void Init(ScrollableAreaController controller, object data, int index, float cellHeight = 0f, float cellWidth = 0f, ScrollableCell parentCell = null)
	{
		this.controller = controller;
		dataObject = data;
		dataIndex = index;
		this.cellHeight = cellHeight;
		this.cellWidth = cellWidth;
		this.parentCell = parentCell;
		if (deactivateIfNull)
		{
			if (data == null)
			{
				base.gameObject.SetActive(false);
			}
			else
			{
				base.gameObject.SetActive(true);
			}
		}
	}

	public virtual void ConfigureCell()
	{
		ConfigureCellData();
	}

	public virtual bool GetAnchorRequirement()
	{
		Debug.LogWarning("GetAnchorRequirement from the BASE : " + base.gameObject.name, base.gameObject);
		return false;
	}

	public virtual void SetAnchors(Transform leftAnchor, Transform rightAnchor)
	{
	}

	public virtual void ConfigureCellData()
	{
	}
}
