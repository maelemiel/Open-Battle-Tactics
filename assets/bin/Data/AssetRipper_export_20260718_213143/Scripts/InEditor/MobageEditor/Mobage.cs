using System;
using System.Collections.Generic;
using UnityEngine;

namespace MobageEditor
{
	public class Mobage : ISessionEstablishmentInfo
	{
		public delegate void MobageUIVisibleNotificationDelegate(bool visible);

		private static List<MobageUIVisibleNotificationDelegate> mobageUIVisibleNotificationList = new List<MobageUIVisibleNotificationDelegate>();

		private static object lockObject = new object();

		private EventReporterSessionFactory analyticsSessionFactory;

		public EventReporterSession AnalyticsSession;

		private bool initialized;

		private static Mobage globalContext;

		public string ConsumerKey { get; set; }

		public string ConsumerSecret { get; set; }

		public string AppId { get; set; }

		public string AppVersion { get; set; }

		public ServerStage ServerStage { get; set; }

		public ServerEnvironment ServerEnvironment { get; set; }

		public string SDKVersion
		{
			get
			{
				TextAsset textAsset = Resources.Load("MobageNDKVersion") as TextAsset;
				return textAsset.text;
			}
		}

		public static Mobage sharedInstance
		{
			get
			{
				lock (lockObject)
				{
					if (globalContext == null)
					{
						globalContext = new Mobage();
					}
					return globalContext;
				}
			}
		}

		public static event MobageUIVisibleNotificationDelegate MobageUIVisible
		{
			add
			{
				mobageUIVisibleNotificationList.Add(value);
			}
			remove
			{
				mobageUIVisibleNotificationList.Remove(value);
			}
		}

		public static void initializeMobageWithStandardParameters(ServerEnvironment serverEnvironment, string appId, string appVersion, string consumerKey, string consumerSecret)
		{
			InitializeMobage(serverEnvironment, appId, appVersion, consumerKey, consumerSecret);
		}

		public void InitializeMobage()
		{
			lock (lockObject)
			{
				if (initialized)
				{
					Debug.Log("MobageNDK: Framework is already initialized, only call this once!");
					return;
				}
				initialized = true;
			}
			if (ServerEnvironment != ServerEnvironment.Production || ServerStage != ServerStage.Production)
			{
			}
			analyticsSessionFactory = new EventReporterSessionFactory((ServerEnvironment != ServerEnvironment.Production) ? AnalyticsServer.Sandbox : AnalyticsServer.Production, "US", AppId, AppVersion, SDKVersion);
			AnalyticsSession = analyticsSessionFactory.NewSession();
			InitializeMobageSession();
		}

		public void InitializeMobageSession()
		{
			MobageSession.Session(this, AnalyticsSession);
		}

		public static void InitializeMobage(ServerEnvironment serverEnvironment, string appId, string appVersion, string consumerKey, string consumerSecret)
		{
			InitializeMobage(serverEnvironment, ServerStage.Production, appId, appVersion, consumerKey, consumerSecret);
		}

		public static void InitializeMobage(ServerEnvironment serverEnvironment, ServerStage serverStage, string appId, string appVersion, string consumerKey, string consumerSecret)
		{
			lock (lockObject)
			{
				Mobage mobage = sharedInstance;
				mobage.ServerEnvironment = serverEnvironment;
				mobage.ServerStage = serverStage;
				mobage.AppId = appId;
				mobage.AppVersion = appVersion;
				mobage.ConsumerKey = consumerKey;
				mobage.ConsumerSecret = consumerSecret;
				mobage.InitializeMobage();
			}
		}

		public static void getMobageVendorId(Action<SimpleAPIStatus, Error, string> onComplete)
		{
			onComplete(SimpleAPIStatus.Success, null, "12fa520");
		}

		public static void MobageUIVisiblePost(bool visible)
		{
			for (int i = 0; i < mobageUIVisibleNotificationList.Count; i++)
			{
				mobageUIVisibleNotificationList[i](visible);
			}
		}
	}
}
