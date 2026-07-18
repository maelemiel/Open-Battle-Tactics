using System;
using System.Collections.Generic;

[Serializable]
public class MenuBarNotification
{
	public SceneTransitionManager.Scene scene;

	public List<NotificationObject> notifications;

	public void EnableNotification(NotificationType notificationType, int count)
	{
		NotificationObject notificationObject = notifications.Find((NotificationObject x) => x.notificationType == notificationType);
		if (notificationObject != null)
		{
			notificationObject.SetNotificationState(true, count);
		}
	}

	public void DisableNotification(NotificationType notificationType)
	{
		NotificationObject notificationObject = notifications.Find((NotificationObject x) => x.notificationType == notificationType);
		if (notificationObject != null)
		{
			notificationObject.SetNotificationState(false);
		}
	}
}
