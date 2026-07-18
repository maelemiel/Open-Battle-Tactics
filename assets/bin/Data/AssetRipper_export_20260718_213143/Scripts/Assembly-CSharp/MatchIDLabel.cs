using UnityEngine;

public class MatchIDLabel : MonoBehaviour
{
	public tk2dTextMesh label;

	private string prefix = "MATCH-ID: ";

	private BattleController controller;

	private void Start()
	{
		controller = GetComponent<BattleController>();
		if ((bool)label)
		{
			label.text = prefix;
		}
	}

	private void Update()
	{
		if ((bool)controller && controller.matchManager != null && controller.matchManager.MatchData != null && controller.matchManager.MatchData.matchId != null)
		{
			if ((bool)label)
			{
				label.text = prefix + controller.matchManager.MatchData.matchId;
			}
			else
			{
				Log.Warning(prefix + controller.matchManager.MatchData.matchId);
			}
			base.enabled = false;
		}
	}
}
