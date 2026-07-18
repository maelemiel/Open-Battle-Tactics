using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using UnityEngine;

public class GachaItemController : MonoBehaviour
{
	public delegate void GachaTankRequestCallback(GachaItemController item);

	private const int SORTING_ORDER_TANK_SPRITE = 1;

	private const string COMMON_REVEAL_BOX_ANIMATION_NAME = "CommonReveal";

	[SerializeField]
	private UnitProxy unitProxy;

	[SerializeField]
	private tk2dSpineAnimation crateBoxSpineAnimation;

	[SerializeField]
	private tk2dSpineAnimation[] fireworkAnimations;

	[SerializeField]
	private tk2dSpineAnimation confettiAnimation;

	[SerializeField]
	private tk2dSpineAnimation revealAnimation;

	[HideInInspector]
	public GachaTankRequestCallback buttonPressedCallback;

	public tk2dUIItem uiButton;

	private bool animating;

	private Tweener starbustTweener;

	private CurrencyEffect currencyEffect;

	private PartFoundEffect partsEffect;

	public bool Animating
	{
		get
		{
			return animating || (currencyEffect != null && currencyEffect.animating) || (partsEffect != null && partsEffect.animating);
		}
	}

	private void GetTankButtonPressed()
	{
		if (buttonPressedCallback != null)
		{
			buttonPressedCallback(this);
		}
	}

	public void GachaResult(ItemCollectionDataModel.Item item)
	{
		uiButton.enabled = false;
		StartCoroutine(ShowNewResult(item));
	}

	public void ResetBox()
	{
		if (unitProxy.Prefab != null)
		{
			Object.Destroy(unitProxy.Prefab);
		}
		if (currencyEffect != null)
		{
			Object.Destroy(currencyEffect.gameObject);
		}
		if (partsEffect != null)
		{
			Object.Destroy(partsEffect.gameObject);
		}
		StartCoroutine(crateBoxSpineAnimation.PlayAnimCoroutine("Still"));
	}

	private IEnumerator ShowNewResult(ItemCollectionDataModel.Item result)
	{
		animating = true;
		AudioTrigger.RevealTank.Play();
		AudioTrigger.CrateBreak.Play();
		AudioTrigger.CrowdExcited.Play();
		StartCoroutine(ShowBoxOpenAnimation());
		EffectInstance gachaStarbust = GlobalEffectsManager.Create(EffectType.GACHA_STARBUST, base.transform.position, base.gameObject);
		gachaStarbust.transform.localScale = Vector3.zero;
		starbustTweener = gachaStarbust.transform.TweenLocalScale(1f, 0.5f);
		switch (result.itemType)
		{
		case UserInventory.ItemType.Unit:
		{
			UnitDataModel unitDataModel = UnitDataModel.GetSingle(result.itemId);
			if (unitDataModel != null)
			{
				yield return StartCoroutine(unitProxy.ChangeAssetCoroutine("Prefab.prefab", unitDataModel.Levels[0].assetBundleId));
			}
			break;
		}
		case UserInventory.ItemType.Parts:
		{
			partsEffect = GlobalEffectsManager.Create(EffectType.PART_DROP, base.transform.position, base.gameObject).SetLayer(base.gameObject.layer).GetComponent<PartFoundEffect>();
			partsEffect.rowWidth = 6;
			partsEffect.SortingOrder = 51;
			List<UnitPartTypesDataModel> partsResult = new List<UnitPartTypesDataModel>();
			UnitPartTypesDataModel partDataModel = UnitPartTypesDataModel.GetSingle(result.itemId);
			if (partDataModel != null)
			{
				for (int i = 0; i < result.amount; i++)
				{
					partsResult.Add(partDataModel);
				}
				if ((bool)partsEffect)
				{
					partsEffect.PlayAnimation(partsResult, null);
				}
				yield return new WaitForSeconds(0.1f);
			}
			break;
		}
		case UserInventory.ItemType.Energy:
		case UserInventory.ItemType.Coins:
		case UserInventory.ItemType.PremiumCurrency:
			currencyEffect = CurrencyEffect.Create(result.itemType, result.amount);
			currencyEffect.gameObject.SetLayerRecursively(base.gameObject.layer);
			currencyEffect.transform.SetParent(base.transform);
			currencyEffect.transform.localPosition = Vector3.zero;
			currencyEffect.SortingOrder = 53;
			yield return new WaitForSeconds(0.1f);
			break;
		}
		StartCoroutine(ShowFireworksAnimation());
		yield return new WaitForSeconds(2f);
		starbustTweener = gachaStarbust.transform.TweenLocalScale(0f, 0.5f);
		yield return new WaitForSeconds(0.5f);
		gachaStarbust.Destroy();
		yield return new WaitForEndOfFrame();
		animating = false;
	}

	private void OnDestroy()
	{
		if (starbustTweener != null)
		{
			starbustTweener.Kill();
		}
	}

	private IEnumerator ShowBoxOpenAnimation()
	{
		crateBoxSpineAnimation.loop = false;
		crateBoxSpineAnimation.AnimationName = "CommonReveal";
		ActivateAndAutodestroyAnimation(confettiAnimation);
		ActivateAndAutodestroyAnimation(revealAnimation);
		yield break;
	}

	private IEnumerator ShowFireworksAnimation()
	{
		AudioTrigger.Fireworks.Play();
		for (int i = 0; i < fireworkAnimations.Length; i++)
		{
			ActivateAndAutodestroyAnimation(fireworkAnimations[i]);
			yield return new WaitForSeconds(0.5f);
		}
	}

	private void ActivateAndAutodestroyAnimation(tk2dSpineAnimation spineAnimation)
	{
		spineAnimation.gameObject.SetActive(true);
		spineAnimation.AnimationComplete += delegate
		{
			spineAnimation.gameObject.SetActive(false);
		};
	}

	public void SetGachaAnimationSkeleton(tk2dSpineSkeletonDataAsset skeletonData)
	{
		if ((bool)skeletonData)
		{
			crateBoxSpineAnimation.Skeleton.skeletonDataAsset = skeletonData;
		}
	}
}
