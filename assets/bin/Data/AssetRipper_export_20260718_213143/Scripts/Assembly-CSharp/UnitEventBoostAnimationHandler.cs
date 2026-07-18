using System.Collections;
using UnityEngine;

public class UnitEventBoostAnimationHandler : AbilityAnimationHandler
{
	public override IEnumerator OntoFieldAnimation()
	{
		StartCoroutine(ShowBoostAnimation());
		yield return new WaitForSeconds(0.5f);
	}

	private IEnumerator ShowBoostAnimation()
	{
		UnitView unit = abilityState.target.unitView;
		EventUnitBoostDataModel boost = unit.state.metadata.UnitBoost;
		if (boost == null)
		{
			yield break;
		}
		int[] values = new int[unit.DiceSides.Length];
		int[] originals = unit.DiceValues;
		for (int i = 0; i < unit.DiceSides.Length; i++)
		{
			switch (unit.DiceSides[i])
			{
			case DieFaceType.DirectDamage:
				values[i] = unit.DiceValues[i] - boost.dieBoostDamage;
				break;
			case DieFaceType.Initiative:
				values[i] = unit.DiceValues[i] - boost.dieBoostInitiative;
				break;
			case DieFaceType.ArmourPiercing:
				values[i] = unit.DiceValues[i] - boost.dieBoostArmourPiercing;
				break;
			}
		}
		unit.ToggleDieFaces(values, true, true);
		yield return new WaitForSeconds(0.5f);
		unit.PlayUpgradeAnimation();
		for (int j = 0; j < originals.Length; j++)
		{
			yield return new WaitForSeconds(0.2f);
			unit.SetDieFace(j, originals[j]);
		}
		yield return new WaitForSeconds(2.5f);
		unit.ToggleDieFaces(originals, true, false);
	}
}
