using System;
using System.Collections.Generic;
using System.Reflection;
using LCD.User;
using UnityEngine;

namespace LCD.Internal.Model
{
	public class Capabilities
	{
		private static string mLCDSDKVersion = "Unity-1.4-d8ec18e";

		private static Capabilities sharedInstance;

		public bool sandbox { get; private set; }

		public bool staging { get; private set; }

		public bool debugLog { get; private set; }

		public bool fullScreen { get; private set; }

		public string bundleId { get; set; }

		public StoreAccount.StoreType storeType { get; private set; }

		internal string advertisingId { get; private set; }

		internal string appVersion { get; private set; }

		public static string sdkVersion
		{
			get
			{
				return mLCDSDKVersion;
			}
			private set
			{
				mLCDSDKVersion = value;
			}
		}

		internal string deviceToken { get; private set; }

		internal string manufacturer { get; private set; }

		internal string deviceName { get; private set; }

		internal string osVersion { get; private set; }

		internal string locale { get; private set; }

		internal string carrier { get; private set; }

		internal string networkType { get; private set; }

		internal int deviceWidth { get; private set; }

		internal int deviceHeight { get; private set; }

		internal string timeZoneName { get; private set; }

		internal int timeZoneOffset { get; private set; }

		internal string publishedAppVersion { get; set; }

		public static Capabilities sharedManager
		{
			get
			{
				if (sharedInstance == null)
				{
					sharedInstance = new Capabilities();
					getLCDSettings();
				}
				return sharedInstance;
			}
		}

		internal static void getLCDSettings()
		{
			Type type = Type.GetType("LCDSettings,Assembly-CSharp-Editor");
			if (type != null)
			{
				MethodInfo method = type.GetMethod("ToJson", BindingFlags.Static | BindingFlags.Public);
				Dictionary<string, object> dictionary = (Dictionary<string, object>)method.Invoke(null, null);
				sharedManager.sandbox = Convert.ToBoolean(dictionary["sandbox"]);
				sharedManager.debugLog = Convert.ToBoolean(dictionary["debugLog"]);
				sharedManager.fullScreen = Convert.ToBoolean(dictionary["fullScreen"]);
				if (!sharedManager.sandbox && !sharedManager.staging)
				{
					sharedManager.debugLog = false;
				}
				sharedManager.bundleId = (string)dictionary["bundleId"];
				sharedManager.appVersion = (string)dictionary["appVersion"];
				sharedManager.locale = (string)dictionary["locale"];
				sharedManager.timeZoneName = (string)dictionary["timeZoneName"];
				sharedManager.timeZoneOffset = Convert.ToInt32(dictionary["timeZoneOffset"]) * 1000;
			}
		}

		public static void Init()
		{
			Capabilities capabilities = sharedManager;
			capabilities.staging = false;
			Type type = Type.GetType("LCDSettings,Assembly-CSharp-Editor");
			if (type != null)
			{
				MethodInfo method = type.GetMethod("set_Staging", BindingFlags.Static | BindingFlags.Public);
				method.Invoke(null, new object[1] { capabilities.staging });
			}
			capabilities.storeType = StoreAccount.StoreType.UNITY_EDITOR;
			capabilities.advertisingId = string.Empty;
			capabilities.deviceToken = null;
			capabilities.manufacturer = "Unity";
			capabilities.deviceName = SystemInfo.deviceName;
			capabilities.osVersion = SystemInfo.operatingSystem;
			capabilities.carrier = SystemInfo.deviceType.ToString();
			capabilities.networkType = "wifi";
			capabilities.deviceWidth = 600;
			capabilities.deviceHeight = 800;
			getLCDSettings();
		}

		internal static void SetLCDSetting(bool sandbox, bool staging, bool debugLog, bool fullScreen, string sdkVersion, string appVersion)
		{
			sharedInstance.sandbox = sandbox;
			sharedInstance.staging = staging;
			sharedInstance.debugLog = debugLog;
			sharedInstance.fullScreen = fullScreen;
			sharedInstance.appVersion = appVersion;
			Capabilities.sdkVersion = sdkVersion;
		}
	}
}
