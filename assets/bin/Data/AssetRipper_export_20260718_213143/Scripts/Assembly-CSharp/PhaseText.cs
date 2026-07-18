using System;
using Holoville.HOTween;
using Holoville.HOTween.Core;
using UnityEngine;

public class PhaseText : MonoBehaviour
{
	[SerializeField]
	private tk2dTextMesh textMesh;

	[SerializeField]
	private tk2dTextMesh smallTextMesh;

	[SerializeField]
	private tk2dBaseSprite background;

	private Vector3 startingPosition;

	[SerializeField]
	public GameObject endPoint;

	private Sequence seq1;

	[SerializeField]
	public Vector3 initialPosition = new Vector3(0f, -350f, 150f);

	public tk2dBaseSprite Background
	{
		get
		{
			return background;
		}
	}

	public tk2dTextMesh SmallTextMesh
	{
		get
		{
			return smallTextMesh;
		}
	}

	public tk2dTextMesh LargeTextMesh
	{
		get
		{
			return textMesh;
		}
	}

	private void Start()
	{
		seq1 = new Sequence();
		startingPosition = base.transform.localPosition;
	}

	public void Present(string text)
	{
		Present(text, delegate
		{
		}, 2f);
	}

	public void Present(string text, Action CallBack)
	{
		Present(text, CallBack, 2f);
	}

	public void Present(string text, Action CallBack, float duration, SpeechType type = SpeechType.EXCITED)
	{
		_Present(textMesh, text, CallBack, duration, type);
	}

	public void PresentSmall(string text, Action CallBack, float duration, SpeechType type = SpeechType.NORMAL)
	{
		_Present(smallTextMesh, text, CallBack, duration, type);
	}

	private void _Present(tk2dTextMesh textField, string text, Action CallBack, float duration, SpeechType type)
	{
		textMesh.gameObject.SetActive(false);
		if ((bool)background)
		{
			background.SetSprite(type.GetSpeechSpriteName());
		}
		if ((bool)smallTextMesh)
		{
			smallTextMesh.gameObject.SetActive(false);
		}
		textField.gameObject.SetActive(true);
		textField.text = text;
		duration -= 0.5f;
		base.transform.localPosition = startingPosition;
		if (seq1 != null)
		{
			seq1.Kill();
		}
		seq1 = new Sequence();
		base.transform.position = initialPosition;
		Vector3 vector = ((!(endPoint != null)) ? new Vector3(0f, 600f, 150f) : initialPosition);
		Vector3 vector2 = ((!(endPoint != null)) ? new Vector3(0f, 190f, 150f) : endPoint.transform.position);
		seq1.Append(HOTween.To(base.transform, 0.25f, new TweenParms().Prop("position", vector2)));
		seq1.AppendInterval(duration);
		seq1.Append(HOTween.To(base.transform, 0.25f, new TweenParms().Prop("position", vector).OnComplete((TweenDelegate.TweenCallback)delegate
		{
			if (CallBack != null)
			{
				CallBack();
			}
		})));
		seq1.Play();
	}

	public void SetText(string text)
	{
		textMesh.gameObject.SetActive(true);
		textMesh.text = text;
		if ((bool)smallTextMesh)
		{
			smallTextMesh.text = string.Empty;
		}
	}

	public void SetSmallText(string text)
	{
		textMesh.text = string.Empty;
		if ((bool)smallTextMesh)
		{
			smallTextMesh.gameObject.SetActive(true);
			smallTextMesh.text = text;
		}
	}
}
