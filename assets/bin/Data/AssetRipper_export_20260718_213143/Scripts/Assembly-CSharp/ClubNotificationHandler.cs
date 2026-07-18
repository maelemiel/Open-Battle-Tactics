public class ClubNotificationHandler : INotificationHandler
{
	private const SceneTransitionManager.Scene NOTIFICATION_SCENE = SceneTransitionManager.Scene.ClubScene;

	private MenuBarNotification menuBarNotification;

	public ClubNotificationHandler(MenuBarNotification menuBarNotification)
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
		NotificationType result = NotificationType.NONE;
		int pendingClubCrateCount = UserProfile.player.pendingClubCrateCount;
		if (pendingClubCrateCount > 0)
		{
			menuBarNotification.EnableNotification(NotificationType.GREEN, pendingClubCrateCount);
			result = NotificationType.GREEN;
		}
		else
		{
			menuBarNotification.DisableNotification(NotificationType.GREEN);
		}
		return result;
	}
}
