using System.Collections;
using UnityEngine;

public class BattleTimerView : MonoBehaviour
{
	public Color timerNormalColor = Color.white;

	public Color timerBlinkingColor = Color.red;

	[SerializeField]
	private tk2dSprite background;

	[SerializeField]
	private tk2dTextMesh text;

	private TimerStates currentState;

	private int previousValue;

	private bool blinkingFlag;

	public void SetTimerValue(float value)
	{
		int num = (int)value;
		if (num != previousValue)
		{
			text.text = num.ToString();
		}
		previousValue = num;
	}

	public void SetTimerState(TimerStates state)
	{
		currentState = state;
		switch (currentState)
		{
		case TimerStates.NORMAL:
			TimerNormalState();
			break;
		case TimerStates.BLINKING_RED:
			TimerBlinkingState();
			break;
		}
	}

	private void TimerNormalState()
	{
		StopAllCoroutines();
		text.color = timerNormalColor;
	}

	private void TimerBlinkingState()
	{
		StartCoroutine(TimerBlinkingStateRepeat(0.3f));
	}

	private IEnumerator TimerBlinkingStateRepeat(float period)
	{
		while (true)
		{
			text.color = ((!blinkingFlag) ? timerNormalColor : timerBlinkingColor);
			blinkingFlag = !blinkingFlag;
			yield return new WaitForSeconds(period);
		}
	}

	public void Show()
	{
		SetActiveState(true);
	}

	public void Hide()
	{
		SetActiveState(false);
	}

	private void SetActiveState(bool state)
	{
		background.gameObject.SetActive(state);
		text.gameObject.SetActive(state);
	}
}
