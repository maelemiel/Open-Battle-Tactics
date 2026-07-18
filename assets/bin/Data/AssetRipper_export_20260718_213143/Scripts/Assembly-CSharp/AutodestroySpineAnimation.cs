using UnityEngine;

public class AutodestroySpineAnimation : MonoBehaviour
{
	public static void Autodestroy(tk2dSpineAnimation spineAnimation)
	{
		spineAnimation.AnimationComplete += DestroyAnimation;
	}

	private static void DestroyAnimation(tk2dSpineAnimation spineAnimation)
	{
		if ((bool)spineAnimation)
		{
			spineAnimation.AnimationComplete -= DestroyAnimation;
			Object.Destroy(spineAnimation.gameObject);
		}
	}
}
