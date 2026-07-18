using System;
using System.Collections.Generic;
using System.Net;

namespace MobageEditor
{
	public class Auth
	{
		public class UserSessionReestablishedNotification
		{
		}

		public class UserLoginNotification
		{
		}

		public class UserLogoutNotification
		{
		}

		public class UserGradeUpgradeNotification
		{
			public string previousNickname;

			public int previousGrade;

			public string currentNickname;

			public int currentGrade;
		}

		public delegate void authorizeToken_onCompleteCallback(SimpleAPIStatus status, Error error, string verifier);

		public delegate void executeUserUpgrade_onCompleteCallback(CancelableAPIStatus status, Error error);

		public delegate void executeUserUpgradeWithParams_onCompleteCallback(CancelableAPIStatus status, Error error);

		public delegate void UserSessionReestablishedNotificationDelegate(UserSessionReestablishedNotification notification);

		public delegate void UserLoginNotificationDelegate(UserLoginNotification notification);

		public delegate void UserLogoutNotificationDelegate(UserLogoutNotification notification);

		public delegate void UserGradeUpgradeNotificationDelegate(UserGradeUpgradeNotification notification);

		private static int cbUidGenerator = 0;

		private static Dictionary<int, Delegate> pendingCallbacks = new Dictionary<int, Delegate>();

		private static Dictionary<int, object> UserSessionReestablished_observers = new Dictionary<int, object>();

		private static Dictionary<object, int> UserSessionReestablished_reverseObservers = new Dictionary<object, int>();

		private static int UserSessionReestablishedObserverUIDGenerator = 0;

		private static Dictionary<int, object> UserLogin_observers = new Dictionary<int, object>();

		private static Dictionary<object, int> UserLogin_reverseObservers = new Dictionary<object, int>();

		private static int UserLoginObserverUIDGenerator = 0;

		private static Dictionary<int, object> UserLogout_observers = new Dictionary<int, object>();

		private static Dictionary<object, int> UserLogout_reverseObservers = new Dictionary<object, int>();

		private static int UserLogoutObserverUIDGenerator = 0;

		private static Dictionary<int, object> UserGradeUpgrade_observers = new Dictionary<int, object>();

		private static Dictionary<object, int> UserGradeUpgrade_reverseObservers = new Dictionary<object, int>();

		private static int UserGradeUpgradeObserverUIDGenerator = 0;

		private static List<UserSessionReestablishedNotificationDelegate> userSessionReestablishedList = new List<UserSessionReestablishedNotificationDelegate>();

		private static List<UserLoginNotificationDelegate> userLoginList = new List<UserLoginNotificationDelegate>();

		private static List<UserLogoutNotificationDelegate> userLogoutList = new List<UserLogoutNotificationDelegate>();

		private static List<UserGradeUpgradeNotificationDelegate> userGradeUpgradeList = new List<UserGradeUpgradeNotificationDelegate>();

		public static event UserSessionReestablishedNotificationDelegate UserSessionReestablished
		{
			add
			{
				userSessionReestablishedList.Add(value);
			}
			remove
			{
				userSessionReestablishedList.Remove(value);
			}
		}

		public static event UserLoginNotificationDelegate UserLogin
		{
			add
			{
				userLoginList.Add(value);
			}
			remove
			{
				userLoginList.Remove(value);
			}
		}

		public static event UserLogoutNotificationDelegate UserLogout
		{
			add
			{
				userLogoutList.Add(value);
			}
			remove
			{
				userLogoutList.Remove(value);
			}
		}

		public static event UserGradeUpgradeNotificationDelegate UserGradeUpgrade
		{
			add
			{
				userGradeUpgradeList.Add(value);
			}
			remove
			{
				userGradeUpgradeList.Remove(value);
			}
		}

		public static void authorizeToken(string token, authorizeToken_onCompleteCallback completeCb)
		{
			MobageRequest request = MobageRequest.Request;
			request.APIMethod = "oauth/authorize";
			request.HTTPMethod = "POST";
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("authorize", "1");
			dictionary.Add("oauth_token", token);
			Dictionary<string, object> postBody = dictionary;
			request.RequestType = RequestType.Fromdata;
			request.PostBody = postBody;
			request.Send(delegate(Error err, JsonData data, WebHeaderCollection headers, HttpStatusCode status)
			{
				Error error = null;
				string verifier = string.Empty;
				if (err != null)
				{
					error = new Error
					{
						localizedDescription = err.localizedDescription,
						domain = "com.mobage.error.api",
						code = err.code
					};
				}
				else if (data.Contains("oauth_verifier"))
				{
					verifier = (string)data["oauth_verifier"];
				}
				completeCb((error != null) ? SimpleAPIStatus.Error : SimpleAPIStatus.Success, error, verifier);
			});
		}

		public static void NotifySessionReestablished()
		{
			for (int i = 0; i < userSessionReestablishedList.Count; i++)
			{
				userSessionReestablishedList[i](new UserSessionReestablishedNotification());
			}
		}

		public static void NotifyLogin()
		{
			for (int i = 0; i < userLoginList.Count; i++)
			{
				userLoginList[i](new UserLoginNotification());
			}
		}

		public static void NotifyLogout()
		{
			for (int i = 0; i < userLogoutList.Count; i++)
			{
				userLogoutList[i](new UserLogoutNotification());
			}
		}

		public static void NotifyUserGradeUpgrade(UserGradeUpgradeNotification notification)
		{
			for (int i = 0; i < userGradeUpgradeList.Count; i++)
			{
				userGradeUpgradeList[i](new UserGradeUpgradeNotification
				{
					previousNickname = notification.previousNickname,
					previousGrade = notification.previousGrade,
					currentNickname = notification.currentNickname,
					currentGrade = notification.currentGrade
				});
			}
		}
	}
}
