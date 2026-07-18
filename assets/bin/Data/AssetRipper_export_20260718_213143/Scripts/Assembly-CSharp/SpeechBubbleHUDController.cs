using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechBubbleHUDController : MonoBehaviour
{
	private const float MESSAGE_TIME = 3f;

	public SpeechBubbleView[] speechBubbles;

	private List<SpeechBubbleView> availableSpeechBubbles = new List<SpeechBubbleView>();

	private void Start()
	{
		for (int i = 0; i < speechBubbles.Length; i++)
		{
			speechBubbles[i].Hide();
		}
		availableSpeechBubbles.AddRange(speechBubbles);
	}

	public void ShowSpeechBubble(Transform target, string message, float time = 3f)
	{
		if (availableSpeechBubbles.Count == 0)
		{
			Log.Error("Someone is trying to show a message but there are no speech bubbles available", base.gameObject);
		}
		else
		{
			StartCoroutine(ShowMessageWithTime(target, message, time));
		}
	}

	private IEnumerator ShowMessageWithTime(Transform target, string message, float time)
	{
		SpeechBubbleView currentSpeechBubble = availableSpeechBubbles[0];
		availableSpeechBubbles.Remove(currentSpeechBubble);
		currentSpeechBubble.Show(target, message);
		yield return new WaitForSeconds(time);
		currentSpeechBubble.Hide();
		availableSpeechBubbles.Add(currentSpeechBubble);
	}
}
