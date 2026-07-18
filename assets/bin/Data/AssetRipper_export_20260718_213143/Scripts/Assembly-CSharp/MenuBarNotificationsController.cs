using System.Collections.Generic;
using UnityEngine;

public class MenuBarNotificationsController : MonoBehaviour
{
	[SerializeField]
	private List<MenuBarNotification> notificationsByScene;

	private Dictionary<SceneTransitionManager.Scene, INotificationHandler> notificationHandlers;

	private void Awake()
	{
	}

	public void UpdateGameObjects(SceneTransitionManager.Scene scene, List<NotificationObject> notifications)
	{
		for (int i = 0; i < notificationsByScene.Count; i++)
		{
			if (notificationsByScene[i].scene == scene)
			{
				notificationsByScene[i].notifications = notifications;
			}
		}
	}

	public void SetupHandlers()
	{
		notificationHandlers = new Dictionary<SceneTransitionManager.Scene, INotificationHandler>();
		notificationHandlers.Add(SceneTransitionManager.Scene.BlueprintsScene, new BlueprintsNotificationHandler(notificationsByScene.Find((MenuBarNotification x) => x.scene == SceneTransitionManager.Scene.BlueprintsScene)));
		notificationHandlers.Add(SceneTransitionManager.Scene.EditTeamAbilitiesScene, new AbilitiesNotificationHandler(notificationsByScene.Find((MenuBarNotification x) => x.scene == SceneTransitionManager.Scene.EditTeamAbilitiesScene)));
		notificationHandlers.Add(SceneTransitionManager.Scene.ShopItemsSuppliesScene, new PrizeGachaNotificationHandler(notificationsByScene.Find((MenuBarNotification x) => x.scene == SceneTransitionManager.Scene.ShopItemsSuppliesScene)));
		notificationHandlers.Add(SceneTransitionManager.Scene.ClubScene, new ClubNotificationHandler(notificationsByScene.Find((MenuBarNotification x) => x.scene == SceneTransitionManager.Scene.ClubScene)));
		notificationHandlers.Add(SceneTransitionManager.Scene.TankUpgradeScene, new TankUpgradeNotificationHandler(notificationsByScene.Find((MenuBarNotification x) => x.scene == SceneTransitionManager.Scene.TankUpgradeScene)));
		notificationHandlers.Add(SceneTransitionManager.Scene.ArenaScene, new TankUpgradeNotificationHandler(notificationsByScene.Find((MenuBarNotification x) => x.scene == SceneTransitionManager.Scene.ArenaScene)));
	}

	public NotificationType UpdateNotifications()
	{
		NotificationType notificationType = NotificationType.NONE;
		if (notificationHandlers != null)
		{
			foreach (SceneTransitionManager.Scene key in notificationHandlers.Keys)
			{
				notificationType |= notificationHandlers[key].UpdateNotifications();
			}
		}
		return notificationType;
	}

	public NotificationType UpdateNotificationForScene(SceneTransitionManager.Scene sceneType)
	{
		NotificationType result = NotificationType.NONE;
		if (notificationHandlers != null && notificationHandlers.ContainsKey(sceneType))
		{
			result = notificationHandlers[sceneType].UpdateNotifications();
		}
		return result;
	}
}
