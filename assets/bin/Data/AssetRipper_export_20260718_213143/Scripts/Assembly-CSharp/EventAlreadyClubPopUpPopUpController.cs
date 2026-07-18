using UnityEngine;

public class EventAlreadyClubPopUpPopUpController : PopupController
{
	public static string EVENT_ALREADY_POP_UP_SHOWN_KEY_VALUE = "event_already_pop_up_shown";

	[SerializeField]
	private EventLogoController eventLogoController;

	[SerializeField]
	private UnitProxy leftUnit;

	[SerializeField]
	private UnitProxy rightUnit;

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
			_title.text = eventDataModel.PopupAlreadyMemberTitle;
		}
		if ((bool)_message)
		{
			_message.text = eventDataModel.PopupAlreadyMemberDescription;
		}
		KeyValueStorage keyValueStorage = KeyValueStorage.Instance(KeyValueStorage.Storage.USER_PROFILE);
		if (keyValueStorage != null)
		{
			keyValueStorage.SetValue(EVENT_ALREADY_POP_UP_SHOWN_KEY_VALUE, int.Parse(eventDataModel.id));
		}
	}
}
