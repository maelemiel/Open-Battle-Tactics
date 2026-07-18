using System;
using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using Spine;
using UnityEngine;

public class BadgeViewController : MonoBehaviour
{
	private const string SHEEN_BADGE_ANIMATION_NAME = "Sheen";

	public int radius = 135;

	public int gapBetweenStars = 35;

	public Vector3 starAnimationOffset;

	private List<tk2dSprite> badgeStars;

	private tk2dSprite badgeSprite;

	private float parentZ;

	private Bone badgeAnchor;

	private void Awake()
	{
		badgeStars = new List<tk2dSprite>(GetComponentsInChildren<tk2dSprite>());
		badgeSprite = GetComponent<tk2dSprite>();
		badgeStars.Remove(badgeSprite);
		parentZ = base.transform.position.z;
		Vector3 vector = badgeStars[0].transform.localPosition - badgeSprite.transform.localPosition;
		radius = (int)vector.magnitude;
		if (badgeStars.Count > 1)
		{
			Vector3 to = badgeStars[1].transform.localPosition - badgeSprite.transform.localPosition;
			gapBetweenStars = (int)Vector3.Angle(vector, to);
		}
		for (int i = 0; i < badgeStars.Count; i++)
		{
			badgeStars[i].gameObject.transform.localPosition = new Vector3(radius, 0f, 0f);
			badgeStars[i].gameObject.SetActive(false);
		}
	}

	public void ConfigureStars(int starsAmount)
	{
		starsAmount = Mathf.Min(starsAmount, badgeStars.Count);
		int[] starsPositionInDegrees = GetStarsPositionInDegrees(starsAmount - 1);
		for (int i = 0; i < badgeStars.Count; i++)
		{
			if (i >= starsAmount)
			{
				badgeStars[i].gameObject.SetActive(false);
				continue;
			}
			badgeStars[i].gameObject.SetActive(true);
			badgeStars[i].transform.localPosition = GetPositionFromDegrees(starsPositionInDegrees[i]);
		}
	}

	private Vector3 GetPositionFromDegrees(int degrees)
	{
		degrees %= 360;
		float num = Mathf.Cos((float)degrees * ((float)Math.PI / 180f));
		float num2 = Mathf.Sin((float)degrees * ((float)Math.PI / 180f));
		Vector2 right = Vector2.right;
		Vector2 vector = new Vector2
		{
			x = right.x * num - right.y * num2,
			y = right.x * num2 + right.y * num
		};
		vector *= (float)radius;
		return new Vector3(vector.x, vector.y, parentZ);
	}

	private int[] GetStarsPositionInDegrees(int starsAmount)
	{
		int[] array = new int[starsAmount + 1];
		switch (starsAmount)
		{
		case 0:
			array[0] = 270;
			break;
		case 1:
			array[0] = 270 - gapBetweenStars / 2;
			array[1] = 270 + gapBetweenStars / 2;
			break;
		case 2:
			array[0] = 270 - gapBetweenStars;
			array[1] = 270;
			array[2] = 270 + gapBetweenStars;
			break;
		case 3:
			array[0] = 270 - (int)((float)gapBetweenStars * 1.5f);
			array[1] = 270 - gapBetweenStars / 2;
			array[2] = 270 + gapBetweenStars / 2;
			array[3] = 270 + (int)((float)gapBetweenStars * 1.5f);
			break;
		case 4:
			array[0] = 270 - gapBetweenStars * 2;
			array[1] = 270 - gapBetweenStars * 1;
			array[2] = 270;
			array[3] = 270 + gapBetweenStars * 1;
			array[4] = 270 + gapBetweenStars * 2;
			break;
		}
		return array;
	}

	public IEnumerator PlayPromotionEffect(int newStarsLevel)
	{
		for (int i = 0; i < badgeStars.Count; i++)
		{
			badgeStars[i].gameObject.SetActive(false);
		}
		float timeBetweenStars = 0.1f;
		newStarsLevel = Mathf.Min(newStarsLevel, badgeStars.Count - 1);
		Transform tempTransform = badgeSprite.transform;
		SimpleTween.Start(0f, 1f, 1f, EaseType.EaseInOutElastic, delegate(float val)
		{
			if ((bool)tempTransform)
			{
				tempTransform.localScale = Vector3.one * val;
			}
		});
		yield return new WaitForSeconds(0.3f);
		int[] positions = GetStarsPositionInDegrees(newStarsLevel);
		for (int i2 = 0; i2 < newStarsLevel; i2++)
		{
			AnimateStar(i2, positions[i2], 0.75f);
			yield return new WaitForSeconds(timeBetweenStars);
		}
		yield return new WaitForSeconds(0.2f * (float)(newStarsLevel + 1));
		AudioTrigger.SpecialResult.Play();
		badgeStars[newStarsLevel].gameObject.SetActive(true);
		badgeStars[newStarsLevel].transform.localPosition = GetPositionFromDegrees(positions[newStarsLevel]);
		Vector3 initialScale = badgeStars[newStarsLevel].transform.localScale;
		badgeStars[newStarsLevel].Alpha = 1f;
		Transform badgeStar = badgeStars[newStarsLevel].transform;
		SimpleTween.Start(0f, 2f, 0.75f, EaseType.EaseOutExpo, delegate(float val)
		{
			if ((bool)badgeStar)
			{
				badgeStar.localScale = initialScale * (3f - val);
			}
		});
		EffectInstance effect = GlobalEffectsManager.Create(EffectType.TIER_STAR, Vector3.zero, base.gameObject);
		effect.AutoDestroy();
		effect.transform.localPosition = GetPositionFromDegrees(positions[newStarsLevel]);
		effect.transform.localPosition += starAnimationOffset;
		tk2dSpineAnimation tierStarAnimation = effect.GetComponent<tk2dSpineAnimation>();
		if ((bool)tierStarAnimation)
		{
			badgeAnchor = tierStarAnimation.Skeleton.skeleton.FindBone("root");
		}
		yield return new WaitForSeconds(2.2f);
		EffectInstance badgeEffect = GlobalEffectsManager.Create(EffectType.TIER_STAR, Vector3.zero, base.gameObject);
		badgeEffect.AutoDestroy();
		badgeEffect.transform.position = new Vector3(0f, -190f, 0f);
		tierStarAnimation = badgeEffect.GetComponent<tk2dSpineAnimation>();
		if ((bool)tierStarAnimation)
		{
			tierStarAnimation.AnimationName = "Sheen";
		}
		AudioTrigger.GachaRareRevealed.Play();
		AudioTrigger.GachaSuperRareRevealed.Play();
		yield return new WaitForSeconds(0.6f);
		badgeSprite.gameObject.transform.localRotation = Quaternion.identity;
	}

	private void AnimateStar(int starIndex, int positionInDegrees, float time)
	{
		badgeStars[starIndex].gameObject.SetActive(true);
		Transform starTransform = badgeStars[starIndex].transform;
		SimpleTween.Start(0f, 360 - positionInDegrees, time, EaseType.EaseOutQuad, delegate(float val)
		{
			if ((bool)starTransform)
			{
				starTransform.localPosition = GetPositionFromDegrees(360 - (int)val);
			}
		});
	}

	private void Update()
	{
		if (badgeSprite != null && badgeAnchor != null)
		{
			badgeSprite.transform.localRotation = Quaternion.Euler(0f, 0f, badgeAnchor.Rotation);
			badgeSprite.transform.localScale = new Vector3(badgeAnchor.ScaleX, badgeAnchor.ScaleY, 1f);
		}
	}

	[ContextMenu("1 Star")]
	private void Test1()
	{
		ConfigureStars(1);
	}

	[ContextMenu("2 Stars")]
	private void Test2()
	{
		ConfigureStars(2);
	}

	[ContextMenu("3 Stars")]
	private void Test3()
	{
		ConfigureStars(3);
	}

	[ContextMenu("4 Stars")]
	private void Test4()
	{
		ConfigureStars(4);
	}

	[ContextMenu("5 Stars")]
	private void Test5()
	{
		ConfigureStars(5);
	}

	[ContextMenu("Effect Stars 5")]
	private void Test10()
	{
		StartCoroutine(PlayPromotionEffect(5));
	}
}
