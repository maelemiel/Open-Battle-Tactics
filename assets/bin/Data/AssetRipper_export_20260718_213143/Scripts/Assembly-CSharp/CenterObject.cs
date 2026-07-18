using UnityEngine;

public class CenterObject : MonoBehaviour
{
	public Transform pointA;

	public Transform pointB;

	public LocateObjectTypes centerObjectType;

	private void Start()
	{
		AutoCenterObject();
	}

	[ContextMenu("Center Object")]
	public void AutoCenterObject()
	{
		if ((bool)pointA && (bool)pointB)
		{
			Transform component = GetComponent<Transform>();
			Vector3 position = component.position;
			switch (centerObjectType)
			{
			case LocateObjectTypes.HORIZONTAL:
				position.x = (pointA.position.x + pointB.position.x) * 0.5f;
				break;
			case LocateObjectTypes.VERTICAL:
				position.y = (pointA.position.y + pointB.position.y) * 0.5f;
				break;
			case LocateObjectTypes.HORIZONTAL_AND_VERTICAL:
				position.x = (pointA.position.x + pointB.position.x) * 0.5f;
				position.y = (pointA.position.y + pointB.position.y) * 0.5f;
				break;
			}
			component.position = position;
		}
	}
}
