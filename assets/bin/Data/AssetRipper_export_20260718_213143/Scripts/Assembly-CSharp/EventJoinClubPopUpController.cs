using UnityEngine;

public class EventJoinClubPopUpController : PopupController
{
	public static string EVENT_JOIN_POP_UP_SHOWN_KEY_VALUE = "event_join_pop_up_shown";

	[SerializeField]
	private EventLogoController eventLogoController;

	[SerializeField]
	private UnitProxy leftUnit;

	[SerializeField]
	private UnitProxy rightUnit;

	[SerializeField]
	private tk2dUIToggleControl checkBox;

	protected override void Start()
	{
		base.Start();
		EventDataModel eventDataModel = (EventDataModel)model.payload;
		if (eventDataModel == null)
		{
			OnCloseButton();
			return;
		}
		if ((bool)eventLogoController)
		{
			StartCoroutine(eventLogoController.LoadLogoCoroutine(eventDataModel));
		}
		if ((bool)leftUnit)
		{
			StartCoroutine(leftUnit.ChangeAssetCoroutine(eventDataModel.EventLeftUnitAssetBundleId));
		}
		if ((bool)rightUnit)
		{
			StartCoroutine(rightUnit.ChangeAssetCoroutine(eventDataModel.EventRightUnitAssetBundleId));
		}
		if ((bool)_title)
		{
			_title.text = eventDataModel.PopupJoinTitle;
		}
		if ((bool)_message)
		{
			_message.text = eventDataModel.PopupJoinDescription;
		}
	}

	public override void OnCloseButton()
	{
		EventDataModel eventDataModel = (EventDataModel)model.payload;
		if ((bool)checkBox && checkBox.IsOn && eventDataModel != null)
		{
			KeyValueStorage keyValueStorage = KeyValueStorage.Instance(KeyValueStorage.Storage.USER_PROFILE);
			if (keyValueStorage != null)
			{
				keyValueStorage.SetValue(EVENT_JOIN_POP_UP_SHOWN_KEY_VALUE, int.Parse(eventDataModel.id));
			}
		}
		base.OnCloseButton();
	}

	private void OnJoin()
	{
		SceneTransitionManager.PushToScene(SceneTransitionManager.Scene.ClubScene);
		OnCloseButton();
	}

	private void OnLater()
	{
		Singleton<UserProfileManager>.instance.SaveShowJoinClub();
		OnCloseButton();
	}
}
