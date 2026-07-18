public class MBUserSessionReestablishedNotification : MBNotification
{
	public static MBNotification Notification
	{
		get
		{
			return new MBUserSessionReestablishedNotification();
		}
	}

	public static void PostEmptyNotification()
	{
		Notification.Post();
	}
}
