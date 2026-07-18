using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class StretchCollider : MonoBehaviour
{
	public Transform upPoint;

	public Transform downPoint;

	public Transform leftPoint;

	public Transform rightPoint;

	public ColliderAnchorType colliderAnchorType;

	public ColliderStretchType colliderStretchType;

	private BoxCollider thisCollider;

	private void Start()
	{
		Stretch();
	}

	[ContextMenu("Stretch Collider")]
	public void Stretch()
	{
		thisCollider = GetComponent<BoxCollider>();
		switch (colliderStretchType)
		{
		case ColliderStretchType.HORIZONTAL:
			StretchHorizontal();
			break;
		case ColliderStretchType.VERTICAL:
			StretchVertical();
			break;
		case ColliderStretchType.HORIZONTAL_VERTICAL:
			StretchVertical();
			StretchHorizontal();
			break;
		}
		if (colliderAnchorType != ColliderAnchorType.NONE)
		{
			CalculateCenter();
		}
	}

	private void CalculateCenter()
	{
		float x = 0f;
		float y = ((!upPoint || !downPoint) ? thisCollider.center.y : ((upPoint.position.y + downPoint.position.y) * 0.5f));
		float num = ((!leftPoint || !rightPoint) ? 0f : Mathf.Abs(leftPoint.position.x - rightPoint.position.x));
		switch (colliderAnchorType)
		{
		case ColliderAnchorType.MIDDLE_CENTER:
			x = 0f;
			break;
		case ColliderAnchorType.MIDDLE_LEFT:
			x = num * 0.5f;
			break;
		case ColliderAnchorType.MIDDLE_RIGHT:
			x = (0f - num) * 0.5f;
			break;
		}
		thisCollider.center = new Vector3(x, y, thisCollider.center.z) - base.transform.localPosition;
	}

	private void StretchHorizontal()
	{
		if ((bool)leftPoint && (bool)rightPoint)
		{
			float num = Mathf.Abs(leftPoint.position.x - rightPoint.position.x);
			float num2 = thisCollider.size.y - num;
			thisCollider.size = new Vector3(num, thisCollider.size.y, thisCollider.size.z);
			if (colliderAnchorType == ColliderAnchorType.NONE)
			{
				Vector3 center = new Vector3(thisCollider.center.x - num2 * 0.5f, thisCollider.center.y, thisCollider.center.z);
				thisCollider.center = center;
			}
		}
	}

	private void StretchVertical()
	{
		if ((bool)upPoint && (bool)downPoint)
		{
			float num = Mathf.Abs(upPoint.position.y - downPoint.position.y);
			float num2 = thisCollider.size.y - num;
			thisCollider.size = new Vector3(thisCollider.size.x, num, thisCollider.size.z);
			if (colliderAnchorType == ColliderAnchorType.NONE)
			{
				Vector3 center = new Vector3(thisCollider.center.x, thisCollider.center.y + num2 * 0.5f, thisCollider.center.z);
				thisCollider.center = center;
			}
		}
	}
}
