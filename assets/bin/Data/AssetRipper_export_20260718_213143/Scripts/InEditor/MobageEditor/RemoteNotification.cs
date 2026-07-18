using System;
using System.Collections.Generic;

namespace MobageEditor
{
	public class RemoteNotification
	{
		public delegate void sendToUser_onCompleteCallback(SimpleAPIStatus status, Error error, RemoteNotificationResponse response);

		public delegate void getRemoteNotificationsEnabled_onCompleteCallback(SimpleAPIStatus status, Error error, bool canBeNotified);

		public delegate void setRemoteNotificationsEnabled_onCompleteCallback(SimpleAPIStatus status, Error error);

		private static int cbUidGenerator = 0;

		private static Dictionary<int, Delegate> pendingCallbacks = new Dictionary<int, Delegate>();
	}
}
