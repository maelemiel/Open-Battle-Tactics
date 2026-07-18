using System.Collections;
using UnityEngine;

public class EvadeAOEAnimationHandler : AbilityAnimationHandler
{
	public override IEnumerator OntoFieldAnimation()
	{
		abilityState.target.unitView.ShowPassiveEffect("Initiative Passive Effect", abilityState.Name);
		yield return new WaitForSeconds(0.1f);
	}

	public override IEnumerator UnitReceivedDamageAnimation(UnitState unit)
	{
		if (!abilityState.target.IsDead && abilityState.target == unit)
		{
			abilityState.target.unitView.TakeDamageAnimationList.Add(ShowEvade());
		}
		yield return null;
	}

	private IEnumerator ShowEvade()
	{
		yield return new WaitForSeconds(0.2f);
		abilityState.target.unitView.ShowPassiveEffect("Initiative Passive Effect", abilityState.Name);
		yield return new WaitForSeconds(0.25f);
	}
}
