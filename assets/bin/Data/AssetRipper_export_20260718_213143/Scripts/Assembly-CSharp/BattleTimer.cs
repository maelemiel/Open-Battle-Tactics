using System;
using UnityEngine;

public class BattleTimer : MonoBehaviour
{
	private const float THRESHOLD_CLOSE_TO_END = 10f;

	[SerializeField]
	private BattleTimerView timerView;

	private float remainingTime;

	private float totalTime;

	private long lastUpdateTime = -1L;

	private bool isRunning;

	private bool closeToEnd;

	private long timeClockStopped;

	private Action timerCallback;

	public float RemainingTime
	{
		get
		{
			return remainingTime;
		}
	}

	public float GetTimeSinceClockStopped
	{
		get
		{
			if (timeClockStopped == 0L)
			{
				return 0f;
			}
			return (float)(TimeManager.ServerTime - timeClockStopped) / 1000f;
		}
	}

	private void Start()
	{
		if (!timerView)
		{
			Log.Warning("No TimerView found!!");
		}
	}

	private void Update()
	{
		if (!isRunning)
		{
			return;
		}
		float num = 0f;
		if (lastUpdateTime == -1)
		{
			lastUpdateTime = TimeManager.ServerTime;
		}
		else
		{
			int num2 = Mathf.FloorToInt((float)(TimeManager.ServerTime - lastUpdateTime) / 1000f);
			if (num2 > 0)
			{
				remainingTime -= num2;
				lastUpdateTime += 1000 * num2;
			}
		}
		remainingTime -= num;
		if (remainingTime <= 10f && !closeToEnd)
		{
			closeToEnd = true;
			timerView.SetTimerState(TimerStates.BLINKING_RED);
			AudioTrigger.TimerEnding.Play();
		}
		if (remainingTime <= 0f)
		{
			timeClockStopped = TimeManager.ServerTime;
			remainingTime = 0f;
			if (timerCallback != null)
			{
				timerCallback();
			}
			isRunning = false;
		}
		if ((bool)timerView)
		{
			timerView.SetTimerValue(remainingTime);
		}
	}

	public void ConfigureNewTimer(float totalTime, Action onFinishedCallback, bool activateGraphics)
	{
		this.totalTime = totalTime;
		remainingTime = this.totalTime;
		timerCallback = onFinishedCallback;
		isRunning = true;
		closeToEnd = false;
		lastUpdateTime = TimeManager.ServerTime;
		timerView.SetTimerState(TimerStates.NORMAL);
		if (activateGraphics)
		{
			ShowTimer();
		}
	}

	public void ResumeTimer()
	{
		isRunning = true;
		timeClockStopped = 0L;
	}

	public void PauseTimer()
	{
		isRunning = false;
		timeClockStopped = 0L;
	}

	public void StopTimer()
	{
		isRunning = false;
		totalTime = 0f;
		remainingTime = 0f;
		timeClockStopped = 0L;
		timerCallback = null;
		closeToEnd = false;
		timerView.SetTimerState(TimerStates.NORMAL);
	}

	public void ShowTimer()
	{
		if ((bool)timerView)
		{
			timerView.Show();
		}
	}

	public void HideTimer()
	{
		if ((bool)timerView)
		{
			timerView.Hide();
		}
	}
}
