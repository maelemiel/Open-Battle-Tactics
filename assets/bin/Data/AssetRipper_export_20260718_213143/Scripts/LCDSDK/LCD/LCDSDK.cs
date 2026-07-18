using System;
using System.Collections.Generic;
using System.Reflection;
using System.Timers;
using LCD.Internal.Impl;
using LCD.Internal.Interface;
using LCD.Internal.Model;
using LCD.Internal.Util;
using LCD.Internal.Web;
using LCD.User;

namespace LCD
{
	public class LCDSDK
	{
		public enum SDKWebViewProcess
		{
			STARTED = 0,
			APPEARED = 1,
			FINISHED = 2
		}

		public interface EventHandler
		{
			void OnSessionUpdate(string accessToken, LCD.User.User user);

			void OnSessionError(LCDError error);

			void OnSDKWebViewProcess(SDKWebViewProcess process);

			void OnRemoteMessage(string message, string extra);
		}

		public delegate void ReportEventCallback(LCDError error);

		private static Timer updateTimer;

		private static string TAG = "LCDSDK";

		private static LCDSDK instance;

		private static Type lcdNativeAssemblyType = Type.GetType("LCD.LCDNative,Assembly-CSharp");

		private LCDSDK(EventHandler eventHandler, bool registerNotification)
		{
			LCDUnityEventHandler.SharedManager.SetEventHandler(eventHandler);
			if (lcdNativeAssemblyType != null)
			{
				MethodInfo method = lcdNativeAssemblyType.GetMethod("Init");
				if ((bool)method.Invoke(null, new object[2] { eventHandler, registerNotification }))
				{
					Capabilities.Init();
					Credentials.Init();
					MainThreadEventProcessor.Init();
				}
			}
		}

		public static void Init(EventHandler eventHandler)
		{
			Init(eventHandler, true);
		}

		public static void Init(EventHandler eventHandler, bool registerForNotifications)
		{
			LCDSDKLog.Debug(TAG, "Init");
			if (instance == null)
			{
				instance = new LCDSDK(eventHandler, registerForNotifications);
			}
		}

		public static void Resume()
		{
			if (lcdNativeAssemblyType == null)
			{
				return;
			}
			MethodInfo method = lcdNativeAssemblyType.GetMethod("Resume");
			if ((bool)method.Invoke(null, null))
			{
				Credentials.GetSession(Credentials.SessionAction.RESUME, null);
				if (updateTimer == null)
				{
					updateTimer = new Timer(180000.0);
					updateTimer.Elapsed += OnUpdateTimer;
					updateTimer.Start();
				}
			}
		}

		public static void Pause()
		{
			if (lcdNativeAssemblyType != null)
			{
				MethodInfo method = lcdNativeAssemblyType.GetMethod("Pause");
				if ((bool)method.Invoke(null, null))
				{
					Credentials.GetSession(Credentials.SessionAction.PAUSE, null);
				}
			}
		}

		public static LCD.User.User GetCurrentUser()
		{
			return LCD.User.User.getInstance();
		}

		public static string GetAccessToken()
		{
			return Credentials.accessToken;
		}

		public static void ReportEvent(string eventCategory, string eventId, long? timestamp, string payload, string playerState)
		{
			ReportEvent(eventCategory, eventId, timestamp, payload, playerState, null);
		}

		public static void ReportEvent(string eventCategory, string eventId, long? timestamp, string payload, string playerState, ReportEventCallback callback)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("eventType", "GAME");
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			dictionary2.Add("payload", payload);
			dictionary2.Add("playerState", playerState);
			if (!timestamp.HasValue)
			{
				timestamp = (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
			}
			dictionary2.Add("eventTime", timestamp);
			SDKWebUtil.Execute("POST", "/analytics/" + eventCategory + "/" + eventId, dictionary, dictionary2, new ReportEventCallbackImpl(callback));
		}

		public static string GetSDKVersion()
		{
			return Capabilities.sdkVersion;
		}

		public static bool IsSandbox()
		{
			return Capabilities.sharedManager.sandbox;
		}

		public static string GetCurrentAppVersion()
		{
			return Capabilities.sharedManager.appVersion;
		}

		public static string GetPublishedAppVersion()
		{
			return Capabilities.sharedManager.publishedAppVersion;
		}

		private static void OnUpdateTimer(object source, ElapsedEventArgs e)
		{
			MainThreadEventProcessor.Instance.QueueEvent(delegate
			{
				Credentials.GetSession(Credentials.SessionAction.UPDATE, null);
			});
		}
	}
}
