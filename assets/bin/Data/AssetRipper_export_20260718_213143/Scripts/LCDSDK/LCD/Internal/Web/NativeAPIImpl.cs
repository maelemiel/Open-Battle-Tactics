using System;
using System.Collections.Generic;
using LCD.Internal.Model;
using LCD.Internal.Util;

namespace LCD.Internal.Web
{
	public class NativeAPIImpl
	{
		private void Start()
		{
		}

		private void Update()
		{
		}

		public static void GetCapabilities(string requestId, Action<string, int, Dictionary<string, object>> callback)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("bundleId", Capabilities.sharedManager.bundleId);
			dictionary.Add("storeType", Capabilities.sharedManager.storeType.ToString());
			dictionary.Add("signatureSHA1", Capabilities.sharedManager.storeType.ToString());
			string advertisingId = Capabilities.sharedManager.advertisingId;
			if (advertisingId != null)
			{
				dictionary.Add("advertisingId", advertisingId);
			}
			string deviceToken = Capabilities.sharedManager.deviceToken;
			if (deviceToken != null)
			{
				dictionary.Add("deviceToken", deviceToken);
			}
			dictionary.Add("appVersion", Capabilities.sharedManager.appVersion);
			dictionary.Add("sdkVersion", Capabilities.sdkVersion);
			dictionary.Add("sandbox", Capabilities.sharedManager.sandbox);
			dictionary.Add("staging", Capabilities.sharedManager.staging);
			dictionary.Add("manufacturer", Capabilities.sharedManager.manufacturer);
			dictionary.Add("deviceName", Capabilities.sharedManager.deviceName);
			dictionary.Add("osVersion", Capabilities.sharedManager.osVersion);
			dictionary.Add("locale", Capabilities.sharedManager.locale);
			string carrier = Capabilities.sharedManager.carrier;
			if (carrier != null)
			{
				dictionary.Add("carrier", carrier);
			}
			string networkType = Capabilities.sharedManager.networkType;
			if (networkType != null)
			{
				dictionary.Add("networkType", networkType);
			}
			dictionary.Add("deviceWidth", Capabilities.sharedManager.deviceWidth);
			dictionary.Add("deviceHeight", Capabilities.sharedManager.deviceHeight);
			dictionary.Add("timeZoneName", Capabilities.sharedManager.timeZoneName);
			dictionary.Add("timeZoneOffset", Capabilities.sharedManager.timeZoneOffset);
			string accessToken = Credentials.accessToken;
			if (accessToken != null)
			{
				dictionary.Add("accessToken", accessToken);
			}
			callback(requestId, 200, dictionary);
		}

		public static void GetCredentials(string requestId, Action<string, int, Dictionary<string, object>> callback)
		{
			Credentials sharedManager = Credentials.SharedManager;
			Dictionary<string, object> arg = sharedManager.ToJson();
			callback(requestId, 200, arg);
		}

		public static void SaveSessionResponse(Dictionary<string, object> json, string requestId, Action<string, int, Dictionary<string, object>> callback)
		{
			Credentials.SharedManager.setSessionResponse(json);
			SDKWebViewManagerInternal.sharedManager.eventHandler.OnSessionUpdate(Json.Serialize(json));
			callback(requestId, 200, null);
		}

		public static void ResetKeyChain(string requestId, Action<string, int, Dictionary<string, object>> callback)
		{
			Credentials sharedManager = Credentials.SharedManager;
			sharedManager.resetKeyChain();
			callback(requestId, 200, null);
		}
	}
}
