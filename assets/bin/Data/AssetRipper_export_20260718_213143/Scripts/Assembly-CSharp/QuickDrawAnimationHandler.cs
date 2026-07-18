using System.Collections;
using UnityEngine;

public class QuickDrawAnimationHandler : AbilityAnimationHandler
{
	public override IEnumerator OntoFieldAnimation()
	{
		yield return StartCoroutine(DisplayQuickDrawEffect());
		yield return new WaitForSeconds(0.1f);
	}

	public IEnumerator DisplayQuickDrawEffect()
	{
		abilityState.target.unitView.PresentSpecialText(abilityState.Name, 1f);
		GlobalEffectsManager.Create(EffectType.QUICK_DRAW, abilityState.target.unitView.TankSpritesTransform.position, abilityState.target.unitView.transform).AutoDestroy();
		yield break;
	}
}
