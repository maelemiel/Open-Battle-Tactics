using System;
using UnityEngine;

public class RaidBossMatchTypeView : BaseMatchTypeView
{
	[SerializeField]
	private GameObject[] childViews;

	[SerializeField]
	private BountyBoardController bountyBoardcontroller;

	public Action<BattleSceneModel> playRaidBossCallback;

	public override void SetEnabled(bool state)
	{
		base.SetEnabled(state);
		GameObject[] array = childViews;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(state);
		}
	}

	protected override void SetupMatchTypeView()
	{
		if (bountyBoardcontroller != null)
		{
			bountyBoardcontroller.Init(playRaidBossCallback);
		}
	}
}
