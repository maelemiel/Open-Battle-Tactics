using System;
using System.Collections.Generic;
using LCD.Internal.Util;
using UnityEngine;

namespace LCD.Internal.Web
{
	internal class SDKWebCallbackManager : MonoBehaviour, SDKWebCallbackHandler
	{
		private static string TAG = "SDKWebCallbackManager";

		private static Dictionary<string, SDKWebCallbackHandler> callbacks = new Dictionary<string, SDKWebCallbackHandler>();

		private static SDKWebCallbackManager instance;

		public static SDKWebCallbackManager SharedManager
		{
			get
			{
				if (instance == null)
				{
					GameObject gameObject = GameObject.Find("SDKWebCallbackManager");
					if (gameObject == null)
					{
						gameObject = new GameObject("SDKWebCallbackManager");
						instance = gameObject.AddComponent("SDKWebCallbackManager") as SDKWebCallbackManager;
					}
					else
					{
						instance = gameObject.GetComponent("SDKWebCallbackManager") as SDKWebCallbackManager;
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

		private string GetRequestId()
		{
			return DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds.ToString();
		}

		internal string AddCallback(SDKWebCallbackHandler callback)
		{
			string requestId = GetRequestId();
			callbacks.Add(requestId, callback);
			return requestId;
		}

		public void onSuccess(string message)
		{
			if (message == null)
			{
				return;
			}
			Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(message);
			object value = null;
			dictionary.TryGetValue("requestId", out value);
			SDKWebCallbackHandler value2 = null;
			if (value != null)
			{
				callbacks.TryGetValue((string)value, out value2);
				if (value2 != null)
				{
					LCDSDKLog.Debug(TAG, "Calling onSuccess: " + value2.GetType().FullName);
					value2.onSuccess(message);
					callbacks.Remove((string)value);
				}
			}
		}

		public void onFailure(string message)
		{
			if (message == null)
			{
				return;
			}
			Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(message);
			object value = null;
			dictionary.TryGetValue("requestId", out value);
			SDKWebCallbackHandler value2 = null;
			if (value != null)
			{
				callbacks.TryGetValue((string)value, out value2);
				if (value2 != null)
				{
					LCDSDKLog.Debug(TAG, "Calling onFailure: " + value2.GetType().FullName);
					value2.onFailure(message);
					callbacks.Remove((string)value);
				}
			}
		}
	}
}
