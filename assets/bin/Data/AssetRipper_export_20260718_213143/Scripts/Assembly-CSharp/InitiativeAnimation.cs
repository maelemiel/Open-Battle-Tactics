using Holoville.HOTween;
using UnityEngine;

public class InitiativeAnimation : MonoBehaviour
{
	private tk2dSprite sprite;

	private void Start()
	{
		sprite = GetComponent<tk2dSprite>();
		base.renderer.enabled = false;
	}

	public void PlayAnimation()
	{
		base.renderer.enabled = true;
		sprite.Alpha = 1f;
		sprite.scale = Vector3.one;
		HOTween.To(sprite, 1f, new TweenParms().Prop("scale", Vector3.one * 5f).Prop("Alpha", 0f).OnComplete(OnAnimationComplete));
	}

	private void OnAnimationComplete()
	{
		base.renderer.enabled = false;
	}
}
