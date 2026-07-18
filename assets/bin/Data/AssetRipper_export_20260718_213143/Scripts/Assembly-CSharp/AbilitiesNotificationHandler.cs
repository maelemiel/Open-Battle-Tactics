public class AbilitiesNotificationHandler : INotificationHandler
{
	private const SceneTransitionManager.Scene NOTIFICATION_SCENE = SceneTransitionManager.Scene.EditTeamAbilitiesScene;

	private MenuBarNotification menuBarNotification;

	public AbilitiesNotificationHandler(MenuBarNotification menuBarNotification)
	{
		this.menuBarNotification = menuBarNotification;
	}

	public NotificationType UpdateNotifications()
	{
		UserProfile player = UserProfile.player;
		MenuBarNotification menuBarNotification = this.menuBarNotification;
		if (menuBarNotification == null)
		{
			return NotificationType.NONE;
		}
		int num = 0;
		if (player != null)
		{
			num = player.newAbilities.Count;
		}
		NotificationType result = NotificationType.NONE;
		if (num > 0)
		{
			menuBarNotification.EnableNotification(NotificationType.GREEN, num);
			result = NotificationType.GREEN;
		}
		else
		{
			menuBarNotification.DisableNotification(NotificationType.GREEN);
		}
		return result;
	}
}
