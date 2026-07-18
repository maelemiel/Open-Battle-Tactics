using System.Collections.Generic;
using UnityEngine;

public class EventInfoUnitsViewController : MonoBehaviour
{
	public UnitInfoView[] unitInfoViews;

	public UnitPositionSet[] positionSets;

	[SerializeField]
	private tk2dTextMesh unitSetTitle;

	public void ConfigureView(List<EventUnitsDataModel> eventUnits, string title)
	{
		if (eventUnits == null)
		{
			return;
		}
		Vector3 vector = Vector3.zero;
		if ((bool)unitSetTitle)
		{
			if (string.IsNullOrEmpty(title))
			{
				unitSetTitle.gameObject.SetActive(false);
				vector = new Vector3(0f, 30f, 0f);
			}
			else
			{
				unitSetTitle.text = title;
			}
		}
		int num = eventUnits.Count - 1;
		if (eventUnits.Count > unitInfoViews.Length)
		{
			Log.Warning("Inconsistent data on EventInfoUnitsViewController. More datamodels than views");
			num = unitInfoViews.Length - 1;
		}
		UnitPositionSet unitPositionSet = positionSets[num];
		for (int i = 0; i < eventUnits.Count && unitInfoViews.Length > i; i++)
		{
			unitInfoViews[i].transform.position = unitPositionSet.transformList[i].position + vector;
			unitInfoViews[i].ConfigureUnitView(UnitDataModel.GetSingle(eventUnits[i].unitId));
		}
		if (eventUnits.Count < unitInfoViews.Length)
		{
			for (int j = eventUnits.Count; j < unitInfoViews.Length; j++)
			{
				unitInfoViews[j].gameObject.SetActive(false);
			}
		}
	}
}
