using System.Collections;
using UnityEngine;

public class RewardBoxView : MonoBehaviour
{
	private const string IDLE_ANIMATION_NAME = "Still";

	private const string REVEAL_ANIMATION_NAME = "CommonReveal";

	private static Vector3 gachaBoxScale = new Vector3(0.1f, 0.1f, 0.1f);

	[SerializeField]
	private GameObject starBust;

	private EffectInstance gachaBoxEffect;

	private tk2dSpineAnimation gachaBoxAnimation;

	private bool isAnimating;

	public void SetupView(tk2dSpineSkeletonDataAsset gachaBoxSkeletonAsset = null)
	{
		if (gachaBoxEffect == null)
		{
			gachaBoxEffect = GlobalEffectsManager.Create(EffectType.GACHA_BOX, base.gameObject.transform.position, base.gameObject.transform);
			gachaBoxEffect.transform.localScale = gachaBoxScale;
		}
		gachaBoxAnimation = gachaBoxEffect.SpineAnimation;
		tk2dBaseSprite component = starBust.GetComponent<tk2dBaseSprite>();
		if ((bool)component)
		{
			gachaBoxAnimation.Skeleton.SortOrder = component.SortingOrder + 1;
		}
		if (gachaBoxSkeletonAsset != null)
		{
			gachaBoxAnimation.Skeleton.skeletonDataAsset = gachaBoxSkeletonAsset;
		}
		ResetEffect();
	}

	public void ResetEffect()
	{
		StartCoroutine(ResetEffectCoroutine());
	}

	private IEnumerator ResetEffectCoroutine()
	{
		while (isAnimating)
		{
			yield return new WaitForEndOfFrame();
		}
		if ((bool)starBust)
		{
			starBust.transform.localScale = Vector3.zero;
			starBust.gameObject.SetActive(false);
		}
		if ((bool)gachaBoxAnimation)
		{
			gachaBoxAnimation.AnimationName = "Still";
			gachaBoxAnimation.loop = true;
		}
	}

	public void PlayEffect()
	{
		StopAllCoroutines();
		StartCoroutine(PlayEffectCoroutine());
	}

	private IEnumerator PlayEffectCoroutine()
	{
		isAnimating = true;
		if ((bool)starBust)
		{
			starBust.gameObject.SetActive(true);
			starBust.transform.TweenLocalScale(1f, 0.6f);
			yield return new WaitForSeconds(0.6f);
			starBust.transform.TweenLocalScale(0f, 0.3f);
		}
		if ((bool)gachaBoxAnimation)
		{
			gachaBoxAnimation.loop = false;
			yield return StartCoroutine(gachaBoxAnimation.PlayAnimCoroutine("CommonReveal"));
		}
		isAnimating = false;
	}
}
