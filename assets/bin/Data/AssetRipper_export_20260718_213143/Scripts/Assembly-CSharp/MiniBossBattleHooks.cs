using System.Collections;
using UnityEngine;

public class MiniBossBattleHooks : BattleHooks
{
	public override IEnumerator PreIntroEnemyRollIn()
	{
		while (battleController.eventLogoController.IsAnimating)
		{
			yield return new WaitForEndOfFrame();
		}
		yield return battleController.StartCoroutine(battleController.miniBossManager.AnimateMiniBossMoment());
	}
}
