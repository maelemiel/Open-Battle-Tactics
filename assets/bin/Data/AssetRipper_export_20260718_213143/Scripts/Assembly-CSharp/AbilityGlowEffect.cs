using Holoville.HOTween;
using UnityEngine;

public class AbilityGlowEffect : MonoBehaviour
{
	private tk2dSprite sprite;

	private void Start()
	{
		sprite = GetComponent<tk2dSprite>();
		base.renderer.enabled = false;
		PlayAnimation();
	}

	public void PlayAnimation()
	{
		base.renderer.enabled = true;
		sprite.scale = Vector3.one;
		HOTween.To(sprite, 1f, new TweenParms().Prop("scale", sprite.scale * 2f).Loops(-1, LoopType.Yoyo));
	}

	private void OnAnimationComplete()
	{
		base.renderer.enabled = false;
	}
}
