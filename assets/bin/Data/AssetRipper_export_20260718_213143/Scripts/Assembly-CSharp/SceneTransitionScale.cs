using UnityEngine;

public class SceneTransitionScale : SceneTransition
{
	public Vector3 amount = new Vector3(0f, 0f, 0f);

	private Vector3 original;

	public override void PrepareTransition()
	{
		original = base.gameObject.transform.localScale;
		base.PrepareTransition();
	}

	public override void EndTransition()
	{
		base.gameObject.transform.localScale = original;
		base.EndTransition();
	}

	public override void UpdateTransition(float t)
	{
		base.gameObject.transform.localScale = original + amount * t;
	}
}
