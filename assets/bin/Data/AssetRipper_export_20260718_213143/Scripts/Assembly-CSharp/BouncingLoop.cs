using Holoville.HOTween;
using UnityEngine;

public class BouncingLoop : MonoBehaviour
{
	[SerializeField]
	private float bounceOffset = 20f;

	private void Start()
	{
		Vector3 vector = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y + bounceOffset, base.transform.localPosition.z);
		HOTween.To(base.transform, 0.5f, new TweenParms().Prop("localPosition", vector).Loops(100000, LoopType.Yoyo));
	}
}
