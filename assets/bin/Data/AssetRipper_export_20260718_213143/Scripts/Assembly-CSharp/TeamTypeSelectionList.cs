using System.Collections.Generic;
using UnityEngine;

public class TeamTypeSelectionList : SelectionListController<ClubTypes>
{
	[SerializeField]
	private tk2dTextMesh teamTypeLabel;

	[SerializeField]
	private GameObject passwordEnabled;

	[SerializeField]
	private GameObject passwordDisabled;

	protected void Awake()
	{
		List<ClubTypes> list = new List<ClubTypes>();
		for (int i = 0; i < 2; i++)
		{
			list.Add((ClubTypes)i);
		}
		Init(list);
	}

	public override void SetSelectedItem(int index)
	{
		base.SetSelectedItem(index);
		if ((bool)teamTypeLabel)
		{
			teamTypeLabel.text = GetSelectedItem().GetLocalizedName();
		}
		bool flag = GetSelectedItem() == ClubTypes.PRIVATE;
		if ((bool)passwordEnabled)
		{
			passwordEnabled.gameObject.SetActive(flag);
		}
		if ((bool)passwordDisabled)
		{
			passwordDisabled.gameObject.SetActive(!flag);
		}
	}
}
