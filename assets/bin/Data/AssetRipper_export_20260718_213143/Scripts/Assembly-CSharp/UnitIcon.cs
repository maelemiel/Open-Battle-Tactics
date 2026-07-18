using UnityEngine;

public class UnitIcon : MonoBehaviour
{
	[SerializeField]
	private tk2dBaseSprite unitIcon;

	public void SetUnitIcon(UnitType unitType)
	{
		if ((bool)unitIcon)
		{
			unitIcon.SetSprite(unitType.GetUnitIconName());
		}
	}
}
