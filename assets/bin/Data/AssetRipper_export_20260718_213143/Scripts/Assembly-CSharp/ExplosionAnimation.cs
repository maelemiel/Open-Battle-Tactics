using System.Collections;
using Holoville.HOTween;
using UnityEngine;

public class ExplosionAnimation : MonoBehaviour
{
	private readonly Vector3 WRECK_VERTICAL_OFFSET = new Vector3(0f, -10f, 0f);

	[SerializeField]
	private int direction = 1;

	[SerializeField]
	private float tweenTime = 5f;

	private Transform originalParent;

	private Vector3 originalLocalPosition;

	private tk2dSpriteAnimator spriteAnimator;

	private MeshRenderer meshRenderer;

	private void Start()
	{
		originalLocalPosition = base.transform.localPosition;
		originalParent = base.transform.parent;
		Singleton<GlobalEffectsManager>.instance.EnsurePoolCapacity(EffectType.EXPLOSION, 1);
	}

	public void PlayAndMove(Transform tParent, Vector3 position, int damage, int rarity = 1, UnitType unitType = UnitType.NONE)
	{
		base.transform.parent = originalParent;
		base.transform.localPosition = originalLocalPosition;
		ExplosionEffect explosionEffect = null;
		StartCoroutine(GlobalEffectsManager.CreateCoroutine(EffectType.EXPLOSION, position, tParent.gameObject, delegate(EffectInstance result)
		{
			result.AutoDestroy();
			explosionEffect = result.GetComponent<ExplosionEffect>();
			if (damage > Constants.BigExplosionThreshold)
			{
				explosionEffect.PlayLargeAnimation();
			}
			else
			{
				explosionEffect.PlaySmallAnimation();
			}
		}));
		EffectType type = EffectType.WRECK;
		if (unitType != UnitType.RAID_BOSS)
		{
			type = ((rarity <= 2) ? EffectType.WRECK : EffectType.WRECK_RARE);
		}
		EffectInstance effect = GlobalEffectsManager.Create(type, position + WRECK_VERTICAL_OFFSET, tParent);
		base.transform.parent = tParent;
		StartCoroutine(PlayWreckEffect(0.25f, effect));
	}

	private IEnumerator PlayWreckEffect(float initialDelay, EffectInstance effect)
	{
		if ((bool)effect)
		{
			effect.gameObject.SetActive(false);
			yield return new WaitForSeconds(initialDelay);
			effect.gameObject.SetActive(true);
			effect.transform.TweenLocalXPosition(base.transform.localPosition.x + (float)(1000 * direction), tweenTime, EaseType.Linear);
			yield return new WaitForSeconds(tweenTime);
			if ((bool)effect)
			{
				effect.Destroy();
			}
			base.transform.parent = originalParent;
		}
	}
}
