public class ContractsNotificationHandler : INotificationHandler
{
	private const SceneTransitionManager.Scene NOTIFICATION_SCENE = SceneTransitionManager.Scene.ContractsScene;

	private MenuBarNotification menuBarNotification;

	public ContractsNotificationHandler(MenuBarNotification menuBarNotification)
	{
		this.menuBarNotification = menuBarNotification;
	}

	public NotificationType UpdateNotifications()
	{
		MenuBarNotification menuBarNotification = this.menuBarNotification;
		if (menuBarNotification == null)
		{
			return NotificationType.NONE;
		}
		UserContract contract = UserProfile.player.contract;
		NotificationType result = NotificationType.NONE;
		if (contract != null)
		{
			if (contract.contractID != -1)
			{
				if (contract.CanClaim)
				{
					menuBarNotification.EnableNotification(NotificationType.GREEN, 1);
					result = NotificationType.GREEN;
				}
				else
				{
					menuBarNotification.DisableNotification(NotificationType.GREEN);
					result = NotificationType.NONE;
				}
				menuBarNotification.DisableNotification(NotificationType.RED);
			}
			else
			{
				menuBarNotification.EnableNotification(NotificationType.RED, -1);
				menuBarNotification.DisableNotification(NotificationType.GREEN);
				result = NotificationType.RED;
			}
		}
		return result;
	}
}
