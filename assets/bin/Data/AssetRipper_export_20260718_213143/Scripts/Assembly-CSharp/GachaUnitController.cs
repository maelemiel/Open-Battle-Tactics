using UnityEngine;

public class GachaUnitController : MonoBehaviour
{
	private GachaUnitView gachaUnitView;

	private UnitDataModel userUnit;

	private void Awake()
	{
		gachaUnitView = GetComponent<GachaUnitView>();
	}

	public void UpdateGachaUnit(UnitDataModel unit)
	{
		userUnit = unit;
		if ((bool)gachaUnitView)
		{
			gachaUnitView.UpdateGachaUnitInfo(userUnit);
		}
	}

	private void OnScrapGachaUnitButtonPressed(tk2dUIItem sender)
	{
	}
}
