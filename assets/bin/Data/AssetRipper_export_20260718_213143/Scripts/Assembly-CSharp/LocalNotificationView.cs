using UnityEngine;

public class LocalNotificationView : MonoBehaviour
{
	[SerializeField]
	private PrefabProxy prefabProxy;

	[SerializeField]
	private UnitProxy unitProxy;

	[SerializeField]
	private tk2dTextMesh notificationText;

	[SerializeField]
	private GameObject cratePrefab;

	public bool ConfigureView(LocalUserNotificationModel notification)
	{
		UnitDataModel unitDataModel = null;
		GachaPoolsDataModel gachaPoolsDataModel = null;
		cratePrefab.SetActive(false);
		switch (notification.NotificationType)
		{
		case LocalNotificationTypes.NONE:
			prefabProxy.ResetProxy();
			unitProxy.ResetProxy();
			SetNotificationText("ui_notification_empty".Localize("Empty Notification"));
			break;
		case LocalNotificationTypes.TANK_BUILT:
			prefabProxy.ResetProxy();
			unitDataModel = (UnitDataModel)notification.DataObject;
			SetupUnitView(unitDataModel);
			SetNotificationText(string.Format("ui_notification_tankbuilt".Localize("New Tank Built! \n [{0}]"), unitDataModel.name));
			break;
		case LocalNotificationTypes.PARTS_COLLECTED:
			prefabProxy.ResetProxy();
			unitDataModel = (UnitDataModel)notification.DataObject;
			SetupUnitView(unitDataModel);
			SetNotificationText(string.Format("ui_notification_tankreadytobuild".Localize("New Tank ready to build! \n [{0}]"), unitDataModel.name));
			break;
		case LocalNotificationTypes.PRIZE_GACHA_READY:
		{
			unitProxy.ResetProxy();
			gachaPoolsDataModel = (GachaPoolsDataModel)notification.DataObject;
			UserGachaPrize gachaPrizeData = UserProfile.player.GetGachaPrizeData(int.Parse(gachaPoolsDataModel.ID));
			if (gachaPrizeData != null && !gachaPrizeData.IsOnCooldown)
			{
				StartCoroutine(prefabProxy.ChangeAssetCoroutine(gachaPoolsDataModel.AssetLinkage));
				SetNotificationText(string.Format("ui_notification_prize_gacha_ready".Localize("Free prizes ready to be claimed! \n [{0}]"), gachaPoolsDataModel.name));
				break;
			}
			return false;
		}
		case LocalNotificationTypes.CLUB_CRATE_READY:
			prefabProxy.ResetProxy();
			unitProxy.ResetProxy();
			cratePrefab.SetActive(true);
			SetNotificationText("ui_notification_club_crate_received".Localize("You have Club Crates to open!"));
			break;
		}
		return true;
	}

	private void SetupUnitView(UnitDataModel unitDataModel)
	{
		UnitLevelProgressionDataModel unitLevelProgressionDataModel = unitDataModel.Levels[0];
		StartCoroutine(unitProxy.ChangeAssetCoroutine(unitLevelProgressionDataModel.assetBundleId));
	}

	private void SetNotificationText(string text)
	{
		if ((bool)notificationText)
		{
			notificationText.text = text;
		}
	}
}
