using System;
using System.Collections.Generic;
using System.Reflection;
using LCD.Internal.Impl;
using LCD.Internal.Model;
using LCD.Internal.Util;
using LCD.User;
using UnityEngine;

namespace LCD.Internal.Interface
{
	public class LCDUnityEventHandler : MonoBehaviour
	{
		private string TAG = "LCDUnityEventHandler";

		private static LCDUnityEventHandler instance;

		private static LCDSDK.EventHandler eventHandler;

		public static LCDUnityEventHandler SharedManager
		{
			get
			{
				if (instance == null)
				{
					GameObject gameObject = GameObject.Find("LCDUnityEventHandler");
					if (gameObject == null)
					{
						gameObject = new GameObject("LCDUnityEventHandler");
						instance = gameObject.AddComponent("LCDUnityEventHandler") as LCDUnityEventHandler;
					}
					else
					{
						instance = gameObject.GetComponent("LCDUnityEventHandler") as LCDUnityEventHandler;
					}
					UnityEngine.Object.DontDestroyOnLoad(instance);
				}
				return instance;
			}
			set
			{
				instance = value;
			}
		}

		public LCDSDK.EventHandler EventHandler
		{
			get
			{
				return eventHandler;
			}
			private set
			{
			}
		}

		private void Awake()
		{
			if (instance == null)
			{
				instance = this;
				UnityEngine.Object.DontDestroyOnLoad(this);
			}
			else if (this != instance)
			{
				UnityEngine.Object.Destroy(this);
			}
		}

		public void SetEventHandler(LCDSDK.EventHandler lcdEventHandler)
		{
			GameObject gameObject = GameObject.Find("LCDUnityEventHandler");
			if (gameObject == null)
			{
				gameObject = new GameObject("LCDUnityEventHandler");
				gameObject.AddComponent("LCDUnityEventHandler");
			}
			eventHandler = lcdEventHandler;
		}

		public void OnSDKWebViewProcess(string message)
		{
			LCDSDK.SDKWebViewProcess process = LCDSDK.SDKWebViewProcess.FINISHED;
			if (message.Equals("STARTED"))
			{
				process = LCDSDK.SDKWebViewProcess.STARTED;
			}
			else if (message.Equals("APPEARED"))
			{
				process = LCDSDK.SDKWebViewProcess.APPEARED;
			}
			else if (message.Equals("FINISHED"))
			{
				process = LCDSDK.SDKWebViewProcess.FINISHED;
			}
			if (eventHandler == null)
			{
				LCDSDKLog.Debug(TAG, "EventHandler null?" + eventHandler);
			}
			else
			{
				eventHandler.OnSDKWebViewProcess(process);
			}
		}

		public void OnRemoteMessage(string message)
		{
			Dictionary<string, string> dictionary = (Dictionary<string, string>)Json.Deserialize(message);
			string message2 = dictionary["message"];
			string value = null;
			dictionary.TryGetValue("extras", out value);
			eventHandler.OnRemoteMessage(message2, value);
		}

		public void OnSessionUpdate(string message)
		{
			Credentials.setSessionResponse(message);
			string accessToken = Credentials.accessToken;
			LCD.User.User user = LCD.User.User.getInstance();
			eventHandler.OnSessionUpdate(accessToken, user);
		}

		public void OnSessionError(string message)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(message);
			string errorType = (string)dictionary["type"];
			long num = (long)dictionary["code"];
			string errorMessage = (string)dictionary["message"];
			LCDErrorImpl error = new LCDErrorImpl(errorType, (int)num, errorMessage);
			eventHandler.OnSessionError(error);
		}

		public void OnCapabilitiesUpdate(string message)
		{
			LCDSDKLog.Debug(TAG, "onSuccess : " + message);
			if (message != null)
			{
				Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(message);
				bool sandbox = Convert.ToBoolean(dictionary["sandbox"]);
				bool staging = Convert.ToBoolean(dictionary["staging"]);
				bool debugLog = Convert.ToBoolean(dictionary["debugLog"]);
				bool fullScreen = Convert.ToBoolean(dictionary["fullScreen"]);
				string sdkVersion = Convert.ToString(dictionary["sdkVersion"]);
				string appVersion = Convert.ToString(dictionary["appVersion"]);
				Capabilities.SetLCDSetting(sandbox, staging, debugLog, fullScreen, sdkVersion, appVersion);
			}
		}

		private void Update()
		{
			Type type = Type.GetType("LCD.Internal.Interface.LCDUnityEventHandleriOS,Assembly-CSharp");
			if (type != null)
			{
				MethodInfo method = type.GetMethod("Update");
				if (method != null)
				{
					method.Invoke(null, null);
				}
			}
		}
	}
}
