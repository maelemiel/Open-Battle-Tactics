using UnityEngine;

public class LocalNotificationController : MonoBehaviour
{
	[SerializeField]
	private LocalNotificationView localNotificationView;

	private LocalUserNotificationModel currentNotification;

	public bool NotificationEnabled { get; set; }

	public bool ShowNotification(LocalUserNotificationModel notification)
	{
		currentNotification = notification;
		if ((bool)localNotificationView)
		{
			Reporting.NotificationShown(currentNotification.NotificationType.ToString());
			return localNotificationView.ConfigureView(notification);
		}
		return false;
	}

	public void OnTouch()
	{
		if (currentNotification != null && NotificationEnabled)
		{
			Reporting.NotificationClicked(currentNotification.NotificationType.ToString());
			currentNotification.ExecuteNotification();
		}
	}
}
