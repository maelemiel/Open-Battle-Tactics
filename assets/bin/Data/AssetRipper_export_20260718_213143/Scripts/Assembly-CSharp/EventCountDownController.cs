using System;
using UnityEngine;

public class EventCountDownController : MonoBehaviour
{
	[SerializeField]
	private tk2dTextMesh countdownLabel;

	[SerializeField]
	private tk2dTextMesh countdownDescriptionLabel;

	public Action AlertLessThanZero;

	public string eventActiveKeyString;

	public string eventCooldownKeyString;

	public string nextEventKeyString;

	public Color activeEventCountdownColor = Color.white;

	public Color cooldownEventCountdownColor = Color.white;

	public Color nextEventCountdownColor = Color.white;

	private EventDataModel eventDataModel;

	private EventBlueprintsState currentEventState = EventBlueprintsState.NONE;

	private long currentTimestamp;

	private bool initialized;

	public void Init(EventDataModel currentEvent, EventBlueprintsState currentState)
	{
		currentEventState = currentState;
		eventDataModel = currentEvent;
		if ((bool)countdownDescriptionLabel)
		{
			countdownDescriptionLabel.text = GetCountdownLabel();
		}
		initialized = true;
	}

	private void Update()
	{
		if (initialized && eventDataModel != null)
		{
			switch (currentEventState)
			{
			case EventBlueprintsState.EVENT_ACTIVE:
				currentTimestamp = NonUnitySingleton<TimeManager>.instance.GetTimeDelta(eventDataModel.DateEndTimeStamp);
				break;
			case EventBlueprintsState.EVENT_COOLDOWN:
				currentTimestamp = NonUnitySingleton<TimeManager>.instance.GetTimeDelta(eventDataModel.DateEndWithCooldownTimeStamp);
				break;
			case EventBlueprintsState.NEXT_EVENT:
				currentTimestamp = NonUnitySingleton<TimeManager>.instance.GetTimeDelta(eventDataModel.DateStartTimeStamp);
				break;
			default:
				Log.Warning("Event State not Supported: " + currentEventState);
				break;
			}
			if (currentTimestamp < 0 && AlertLessThanZero != null)
			{
				AlertLessThanZero();
			}
			currentTimestamp = ((currentTimestamp >= 0) ? currentTimestamp : 0);
			if ((bool)countdownLabel)
			{
				countdownLabel.color = GetCountdownColor();
				countdownLabel.text = TimeFormats.GetTimeString(currentTimestamp, TimeFormat.LEADERBOARD_COUNTDOWN);
			}
		}
	}

	private string GetCountdownLabel()
	{
		string result = string.Empty;
		switch (currentEventState)
		{
		case EventBlueprintsState.EVENT_ACTIVE:
			result = eventActiveKeyString.Localize("EVENT NOW ON!");
			break;
		case EventBlueprintsState.EVENT_COOLDOWN:
			result = eventCooldownKeyString.Localize("EVENT NOW ON!");
			break;
		case EventBlueprintsState.NEXT_EVENT:
			if (eventDataModel != null)
			{
				result = nextEventKeyString.Localize("NEXT EVENT IN:");
			}
			break;
		default:
			Log.Warning("Event State not Supported: " + currentEventState);
			break;
		}
		return result;
	}

	private Color GetCountdownColor()
	{
		Color white = Color.white;
		switch (currentEventState)
		{
		case EventBlueprintsState.EVENT_ACTIVE:
			white = activeEventCountdownColor;
			break;
		case EventBlueprintsState.EVENT_COOLDOWN:
			white = cooldownEventCountdownColor;
			break;
		case EventBlueprintsState.NEXT_EVENT:
			white = nextEventCountdownColor;
			break;
		default:
			Log.Warning("Event State not Supported: " + currentEventState);
			break;
		}
		return white;
	}
}
