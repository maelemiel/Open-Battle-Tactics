using UnityEngine;

public class SceneTransitionRotate : SceneTransition
{
	public Vector3 amount = new Vector3(0f, 0f, 0f);

	private Quaternion destination;

	private Quaternion original;

	public override void PrepareTransition()
	{
		destination = Quaternion.Euler(base.gameObject.transform.eulerAngles + amount);
		original = base.gameObject.transform.rotation;
		base.PrepareTransition();
	}

	public override void UpdateTransition(float t)
	{
		base.gameObject.transform.rotation = Quaternion.Slerp(original, destination, t);
	}
}
