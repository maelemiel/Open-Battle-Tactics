using Holoville.HOTween;
using UnityEngine;

public class FloatingBehaviour : MonoBehaviour
{
	private float duration;

	private float distance;

	private Sequence floating;

	public void SetParams(float time, float dist)
	{
		duration = time;
		distance = dist;
	}

	public void Start()
	{
		duration = 3f;
		distance = 80f;
	}

	public void StartFloat()
	{
		Vector3 localPosition = base.gameObject.transform.localPosition;
		Vector3 vector = new Vector3(localPosition.x, localPosition.y + distance, localPosition.z);
		Vector3 vector2 = new Vector3(localPosition.x, localPosition.y - distance, localPosition.z);
		Sequence sequence = new Sequence(new SequenceParms().OnComplete(doFloat));
		sequence.Append(HOTween.To(base.gameObject.transform, duration * 0.5f, new TweenParms().Prop("localPosition", vector2).Ease(EaseType.Linear)));
		floating = new Sequence(new SequenceParms().Loops(-1, LoopType.Yoyo));
		floating.Append(HOTween.To(base.gameObject.transform, duration, new TweenParms().Prop("localPosition", vector).Ease(EaseType.Linear)));
		floating.Append(HOTween.To(base.gameObject.transform, duration, new TweenParms().Prop("localPosition", vector2).Ease(EaseType.Linear)));
		sequence.Play();
	}

	public void doFloat()
	{
		floating.Play();
	}

	private void OnDestroy()
	{
		if (floating != null)
		{
			floating.Kill();
		}
	}
}
