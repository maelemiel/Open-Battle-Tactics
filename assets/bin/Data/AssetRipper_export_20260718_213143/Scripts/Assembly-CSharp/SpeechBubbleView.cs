using Holoville.HOTween;
using UnityEngine;

public class SpeechBubbleView : MonoBehaviour
{
	[SerializeField]
	private tk2dTextMesh messageLabel;

	[SerializeField]
	private tk2dBaseSprite bubbleSprite;

	private Vector3 offset = new Vector3(40f, 60f, 0f);

	private void SetBubbleTarget(Transform targetTransform)
	{
		if ((bool)targetTransform)
		{
			base.transform.position = targetTransform.position + offset;
		}
		base.gameObject.SetLayerRecursively(targetTransform.gameObject.layer);
	}

	private void SetBubbleText(string text)
	{
		if ((bool)messageLabel)
		{
			messageLabel.text = text;
		}
	}

	public void SetBubblePosition(Vector3 position)
	{
		base.transform.position = position;
	}

	public void Show(Transform target, string message)
	{
		SetBubbleTarget(target);
		SetBubbleText(message);
		base.gameObject.SetActive(true);
		base.transform.localScale = Vector3.zero;
		base.transform.TweenLocalScale(1f, 0.5f, EaseType.EaseOutBack);
	}

	public void Hide()
	{
		SetBubbleText(string.Empty);
		base.gameObject.SetActive(false);
	}
}
