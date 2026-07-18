using System.Collections.Generic;
using UnityEngine;

public class TeamBadgeSelectionList : AssetLinkageSelectionList
{
	private int TEAM_BADGE_BUNDLE_ID = 1015;

	[SerializeField]
	private tk2dTextMesh indexLabel;

	protected void Awake()
	{
		List<AssetLinkageDataModel> list = AssetLinkageDataModel.GetAll().FindAll((AssetLinkageDataModel x) => x.bundleId == TEAM_BADGE_BUNDLE_ID);
		if (list.Count == 0)
		{
			return;
		}
		List<AssetLinkageDataModel> list2 = new List<AssetLinkageDataModel>();
		foreach (AssetLinkageDataModel item in list)
		{
			list2.Add(item);
		}
		Init(list2);
	}

	public override void SetSelectedItem(int index)
	{
		base.SetSelectedItem(index);
		if ((bool)indexLabel)
		{
			indexLabel.text = currentIndex.ToString();
		}
	}
}
