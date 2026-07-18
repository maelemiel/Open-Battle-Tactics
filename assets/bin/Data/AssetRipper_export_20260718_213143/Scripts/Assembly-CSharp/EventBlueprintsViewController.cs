using System.Collections.Generic;
using UnityEngine;

public class EventBlueprintsViewController : MonoBehaviour
{
	public ScrollableAreaController eventUnitsScrollableArea;

	public GameObject[] anchoredGameObjects;

	public Color activeEventCountdownColor = Color.white;

	public Color cooldownEventCountdownColor = Color.white;

	public Color nextEventCountdownColor = Color.white;

	private EventDataModel currentEventDataModel;

	private EventDataModel nextEventDataModel;

	[SerializeField]
	private GameObject noClubState;

	[SerializeField]
	private GameObject nextEventState;

	[SerializeField]
	private GameObject noActiveEventState;

	[SerializeField]
	private tk2dTextMesh eventName;

	[SerializeField]
	private tk2dTextMesh countdownLabel;

	[SerializeField]
	private tk2dTextMesh eventMessage;

	[SerializeField]
	private PrefabProxy noEventLogoPrefabProxy;

	[SerializeField]
	private EventLogoController eventLogoController;

	[SerializeField]
	private GameObject noUnitsGameObject;

	private EventBlueprintsState currentState = EventBlueprintsState.NONE;

	private long currentTimestamp;

	private bool initialized;

	private void Update()
	{
		if (!initialized)
		{
			return;
		}
		switch (currentState)
		{
		case EventBlueprintsState.EVENT_ACTIVE:
			currentTimestamp = NonUnitySingleton<TimeManager>.instance.GetTimeDelta(currentEventDataModel.DateEndTimeStamp);
			break;
		case EventBlueprintsState.EVENT_COOLDOWN:
			currentTimestamp = NonUnitySingleton<TimeManager>.instance.GetTimeDelta(currentEventDataModel.DateEndWithCooldownTimeStamp);
			break;
		case EventBlueprintsState.NO_EVENT:
		case EventBlueprintsState.NEXT_EVENT:
			if (nextEventDataModel != null)
			{
				currentTimestamp = NonUnitySingleton<TimeManager>.instance.GetTimeDelta(nextEventDataModel.DateStartTimeStamp);
			}
			break;
		}
		if ((bool)countdownLabel)
		{
			countdownLabel.text = TimeFormats.GetTimeString(currentTimestamp, TimeFormat.LEADERBOARD_COUNTDOWN);
		}
		RefreshState();
	}

	public void Init(EventDataModel eventDataModel)
	{
		if (eventDataModel == null)
		{
			Log.Warning("Null EventDataModel");
		}
		currentEventDataModel = eventDataModel;
		if ((bool)eventName && currentEventDataModel != null)
		{
			eventName.text = currentEventDataModel.name;
		}
		RefreshState();
		initialized = true;
	}

	private void RefreshState()
	{
		EventBlueprintsState eventBlueprintsState = EventBlueprintsState.NO_EVENT;
		if (currentEventDataModel != null)
		{
			if (currentEventDataModel.IsActive)
			{
				eventBlueprintsState = EventBlueprintsState.EVENT_ACTIVE;
			}
			else if (currentEventDataModel.IsOnCooldown)
			{
				eventBlueprintsState = EventBlueprintsState.EVENT_COOLDOWN;
			}
		}
		if (eventBlueprintsState == EventBlueprintsState.NO_EVENT)
		{
			currentEventDataModel = null;
			if (nextEventDataModel != null)
			{
				eventBlueprintsState = EventBlueprintsState.NEXT_EVENT;
				if (nextEventDataModel.IsActive)
				{
					currentEventDataModel = nextEventDataModel;
					nextEventDataModel = null;
					eventBlueprintsState = EventBlueprintsState.EVENT_ACTIVE;
				}
				else if (nextEventDataModel.IsOnCooldown)
				{
					currentEventDataModel = nextEventDataModel;
					nextEventDataModel = null;
					eventBlueprintsState = EventBlueprintsState.EVENT_COOLDOWN;
				}
			}
			else
			{
				nextEventDataModel = UserProfile.player.GetNextEvent();
				if (nextEventDataModel != null)
				{
					currentEventDataModel = null;
					eventBlueprintsState = EventBlueprintsState.NEXT_EVENT;
				}
			}
		}
		SetState(eventBlueprintsState);
	}

	private void SetState(EventBlueprintsState eventBlueprintsState)
	{
		if (eventBlueprintsState == currentState)
		{
			return;
		}
		bool flag = nextEventDataModel != null;
		switch (eventBlueprintsState)
		{
		case EventBlueprintsState.EVENT_ACTIVE:
			if ((bool)eventUnitsScrollableArea)
			{
				eventUnitsScrollableArea.gameObject.SetActive(true);
				eventUnitsScrollableArea.InitializeWithData(GetEventUnits(currentEventDataModel));
				if ((bool)noUnitsGameObject)
				{
					noUnitsGameObject.SetActive(GetEventUnits(currentEventDataModel).Count == 0);
				}
			}
			if ((bool)eventMessage)
			{
				eventMessage.text = "ui_event_units_tanks_available".Localize("TANKS AVAILABLE FOR:");
			}
			if ((bool)countdownLabel)
			{
				countdownLabel.gameObject.SetActive(true);
				countdownLabel.color = activeEventCountdownColor;
			}
			if ((bool)eventName)
			{
				eventName.text = currentEventDataModel.name;
				eventName.gameObject.SetActive(true);
			}
			if ((bool)noActiveEventState)
			{
				noActiveEventState.SetActive(false);
			}
			if ((bool)noClubState)
			{
				noClubState.SetActive(false);
			}
			if ((bool)nextEventState)
			{
				nextEventState.SetActive(false);
			}
			if ((bool)eventLogoController)
			{
				eventLogoController.gameObject.SetActive(true);
				eventLogoController.LoadLogo();
			}
			if ((bool)noEventLogoPrefabProxy)
			{
				noEventLogoPrefabProxy.gameObject.SetActive(false);
			}
			break;
		case EventBlueprintsState.EVENT_COOLDOWN:
			if ((bool)eventUnitsScrollableArea)
			{
				eventUnitsScrollableArea.gameObject.SetActive(true);
				eventUnitsScrollableArea.InitializeWithData(GetEventUnits(currentEventDataModel));
				if ((bool)noUnitsGameObject)
				{
					noUnitsGameObject.SetActive(GetEventUnits(currentEventDataModel).Count == 0);
				}
			}
			if ((bool)eventMessage)
			{
				eventMessage.text = "ui_event_units_event_finished".Localize("The event has ended. But you can still build event tanks!");
			}
			if ((bool)countdownLabel)
			{
				countdownLabel.gameObject.SetActive(true);
				countdownLabel.color = cooldownEventCountdownColor;
			}
			if ((bool)eventName)
			{
				eventName.text = currentEventDataModel.name;
				eventName.gameObject.SetActive(true);
			}
			if ((bool)noActiveEventState)
			{
				noActiveEventState.SetActive(false);
			}
			if ((bool)noClubState)
			{
				noClubState.SetActive(false);
			}
			if ((bool)nextEventState)
			{
				nextEventState.SetActive(false);
			}
			if ((bool)eventLogoController)
			{
				eventLogoController.gameObject.SetActive(true);
			}
			if ((bool)noEventLogoPrefabProxy)
			{
				noEventLogoPrefabProxy.gameObject.SetActive(false);
			}
			break;
		case EventBlueprintsState.NO_EVENT:
			if ((bool)countdownLabel)
			{
				countdownLabel.gameObject.SetActive(false);
			}
			if ((bool)eventName)
			{
				eventName.gameObject.SetActive(false);
			}
			if ((bool)noUnitsGameObject)
			{
				noUnitsGameObject.SetActive(false);
			}
			if ((bool)eventUnitsScrollableArea)
			{
				UnitDataModel unitBeingBuilt2 = GetUnitBeingBuilt();
				if (unitBeingBuilt2 != null)
				{
					eventUnitsScrollableArea.gameObject.SetActive(true);
					eventUnitsScrollableArea.InitializeWithData(new List<UnitDataModel> { unitBeingBuilt2 });
				}
				else
				{
					eventUnitsScrollableArea.gameObject.SetActive(false);
				}
			}
			if ((bool)noActiveEventState)
			{
				noActiveEventState.SetActive(true);
			}
			if ((bool)noClubState)
			{
				noClubState.SetActive(false);
			}
			if ((bool)nextEventState)
			{
				nextEventState.SetActive(false);
			}
			if ((bool)eventMessage)
			{
				eventMessage.text = string.Empty;
			}
			if ((bool)eventLogoController)
			{
				eventLogoController.gameObject.SetActive(false);
			}
			if ((bool)noEventLogoPrefabProxy)
			{
				noEventLogoPrefabProxy.gameObject.SetActive(true);
				StartCoroutine(noEventLogoPrefabProxy.ChangeAssetCoroutine(Constants.NoEventLogoAssetLinkage));
			}
			break;
		case EventBlueprintsState.NEXT_EVENT:
			if ((bool)countdownLabel)
			{
				countdownLabel.gameObject.SetActive(flag);
				countdownLabel.color = nextEventCountdownColor;
			}
			if ((bool)eventName)
			{
				eventName.gameObject.SetActive(false);
			}
			if ((bool)noUnitsGameObject)
			{
				noUnitsGameObject.SetActive(false);
			}
			if ((bool)eventUnitsScrollableArea)
			{
				UnitDataModel unitBeingBuilt = GetUnitBeingBuilt();
				if (unitBeingBuilt != null)
				{
					eventUnitsScrollableArea.gameObject.SetActive(true);
					eventUnitsScrollableArea.InitializeWithData(new List<UnitDataModel> { unitBeingBuilt });
				}
				else
				{
					eventUnitsScrollableArea.gameObject.SetActive(false);
				}
			}
			if ((bool)noActiveEventState)
			{
				noActiveEventState.SetActive(false);
			}
			if ((bool)noClubState)
			{
				noClubState.SetActive(false);
			}
			if ((bool)nextEventState)
			{
				nextEventState.SetActive(true);
			}
			if ((bool)eventMessage)
			{
				eventMessage.text = string.Empty;
			}
			if ((bool)eventLogoController)
			{
				eventLogoController.gameObject.SetActive(false);
			}
			if ((bool)noEventLogoPrefabProxy)
			{
				noEventLogoPrefabProxy.gameObject.SetActive(true);
				StartCoroutine(noEventLogoPrefabProxy.ChangeAssetCoroutine(Constants.NoEventLogoAssetLinkage));
			}
			break;
		}
		currentState = eventBlueprintsState;
	}

	private UnitDataModel GetUnitBeingBuilt()
	{
		if (UserProfile.player != null)
		{
			foreach (UserResearcher researcher in UserProfile.player.researchers)
			{
				if (researcher.researchType == UserResearcher.ResearchType.BuildTank)
				{
					UnitDataModel single = UnitDataModel.GetSingle(researcher.itemID);
					if (single != null && single.UnitType.IsEvent())
					{
						return single;
					}
				}
			}
		}
		return null;
	}

	private List<UnitDataModel> GetEventUnits(EventDataModel eventDataModel)
	{
		if (currentEventDataModel == null)
		{
			return null;
		}
		List<UnitDataModel> list = new List<UnitDataModel>();
		List<EventUnitsDataModel> eventUnits = currentEventDataModel.GetEventUnits();
		UnitDataModel unitBeingBuilt = GetUnitBeingBuilt();
		if (unitBeingBuilt != null && unitBeingBuilt.UnitType.IsEvent() && !currentEventDataModel.UnitBelongsToEvent(unitBeingBuilt.id))
		{
			list.Add(unitBeingBuilt);
		}
		foreach (EventUnitsDataModel item in eventUnits)
		{
			if (item.CanBuild)
			{
				list.Add(UnitDataModel.GetSingle(item.unitId));
			}
		}
		return list;
	}

	private void SetAnchoredGameObjectsState(bool state)
	{
		for (int i = 0; i < anchoredGameObjects.Length; i++)
		{
			if ((bool)anchoredGameObjects[i])
			{
				anchoredGameObjects[i].SetActive(state);
			}
		}
	}

	private void OnEnable()
	{
		SetAnchoredGameObjectsState(true);
	}

	private void OnDisable()
	{
		SetAnchoredGameObjectsState(false);
	}

	public void LoadEventLogo()
	{
		if (eventLogoController.gameObject.activeInHierarchy)
		{
			eventLogoController.LoadLogo();
		}
		if (noEventLogoPrefabProxy.gameObject.activeInHierarchy)
		{
			StartCoroutine(noEventLogoPrefabProxy.ChangeAssetCoroutine(Constants.NoEventLogoAssetLinkage));
		}
	}
}
