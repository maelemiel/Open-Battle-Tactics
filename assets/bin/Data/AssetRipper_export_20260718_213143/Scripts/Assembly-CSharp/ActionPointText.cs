using Holoville.HOTween;
using UnityEngine;

public class ActionPointText : MonoBehaviour
{
	[SerializeField]
	private tk2dTextMesh actionPointText;

	[SerializeField]
	private float holdDistance = 10f;

	[SerializeField]
	private float holdTime = 0.7f;

	private float topY;

	private float bottomY;

	private void Awake()
	{
		actionPointText.gameObject.SetActive(false);
		topY = actionPointText.transform.localPosition.y;
		bottomY = 0f - topY;
	}

	public void ShowActionPointText(string text)
	{
		actionPointText.text = text;
		actionPointText.gameObject.SetActive(true);
		actionPointText.transform.localPosition = new Vector3(0f, bottomY, 0f);
		Sequence sequence = new Sequence();
		sequence.Append(actionPointText.transform.TweenLocalYPosition(topY, 0.5f, EaseType.EaseOutBack));
		sequence.Append(actionPointText.transform.TweenLocalYPosition(topY - holdDistance, holdTime, EaseType.Linear));
		sequence.Append(actionPointText.transform.TweenLocalYPosition(bottomY, 0.5f, EaseType.EaseInExpo));
		sequence.AppendCallback(OnComplete);
		sequence.Play();
	}

	private void OnComplete()
	{
		actionPointText.gameObject.SetActive(false);
	}
}
