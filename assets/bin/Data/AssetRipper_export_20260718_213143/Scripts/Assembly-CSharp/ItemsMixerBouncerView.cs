using System.Collections;
using UnityEngine;

public class ItemsMixerBouncerView : MonoBehaviour
{
	public AudioTrigger sound;

	public float finalEffectScale = 2f;

	public Color bouncerColor;

	private void OnTriggerEnter(Collider coll)
	{
		sound.Play();
		StartCoroutine(PlayEffect());
	}

	private IEnumerator PlayEffect()
	{
		EffectInstance bouncerEffect = GlobalEffectsManager.Create(EffectType.ITEMS_MIXER_BOUNCER, base.transform.position, base.gameObject);
		ItemsMixerBouncerEffect effect = bouncerEffect.GetComponent<ItemsMixerBouncerEffect>();
		if ((bool)effect)
		{
			effect.initialColor = bouncerColor;
			effect.PlayEffect(finalEffectScale);
		}
		yield return new WaitForSeconds(0.2f);
		if ((bool)bouncerEffect)
		{
			bouncerEffect.Destroy();
		}
	}
}
