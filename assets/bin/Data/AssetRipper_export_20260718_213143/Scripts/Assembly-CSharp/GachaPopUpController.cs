using System.Collections.Generic;
using UnityEngine;

public class GachaPopUpController : MonoBehaviour
{
	public GachaUnitController[] gachaUnitControllers;

	public void UpdateGachaPopUp(List<UnitDataModel> units)
	{
		if (units != null && gachaUnitControllers != null)
		{
			if (gachaUnitControllers.Length < units.Count)
			{
				Log.Error("Gacha units and gacha views count are inconsistent", base.gameObject);
			}
			for (int i = 0; i < units.Count; i++)
			{
				gachaUnitControllers[i].UpdateGachaUnit(units[i]);
			}
		}
	}
}
