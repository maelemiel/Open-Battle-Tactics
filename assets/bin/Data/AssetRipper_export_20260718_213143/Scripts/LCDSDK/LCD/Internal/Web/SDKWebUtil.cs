using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace LCD.Internal.Web
{
	internal class SDKWebUtil
	{
		internal static void Execute(string method, string path, Dictionary<string, string> param, Dictionary<string, object> data, SDKWebCallbackHandler callback)
		{
			Type type = Type.GetType("LCD.Internal.Web.SDKHttpClient,Assembly-CSharp");
			if (type == null)
			{
				return;
			}
			MethodInfo method2 = type.GetMethod("Execute", BindingFlags.Static | BindingFlags.Public);
			if (method2 != null)
			{
				string value = SDKWebCallbackManager.SharedManager.AddCallback(callback);
				if (param == null)
				{
					param = new Dictionary<string, string>();
				}
				param.Add("requestId", value);
				object[] parameters = new object[5]
				{
					method,
					path,
					param,
					data,
					SDKWebCallbackManager.SharedManager
				};
				method2.Invoke(null, parameters);
			}
			else
			{
				Debug.LogError("SDKHttpClient.execute not found");
			}
		}

		internal static LCDSDK.SDKWebViewProcess currentProcess()
		{
			Type type = Type.GetType("LCD.Internal.Web.SDKWebViewManager,Assembly-CSharp");
			LCDSDK.SDKWebViewProcess result = LCDSDK.SDKWebViewProcess.FINISHED;
			if (type != null)
			{
				MethodInfo method = type.GetMethod("get_currentProcess");
				if (method != null)
				{
					MethodInfo method2 = type.GetMethod("get_sharedManager");
					object obj = method2.Invoke(null, null);
					result = (LCDSDK.SDKWebViewProcess)(int)method.Invoke(obj, null);
				}
			}
			return result;
		}

		internal static void openSDKWebView(string action, Dictionary<string, object> param, SDKWebCallbackHandler callback, bool display)
		{
			Type type = Type.GetType("LCD.Internal.Web.SDKWebViewManager,Assembly-CSharp");
			if (type == null)
			{
				return;
			}
			MethodInfo method = type.GetMethod("openSDKWebViewStringAction");
			if (method != null)
			{
				string value = SDKWebCallbackManager.SharedManager.AddCallback(callback);
				if (param == null)
				{
					param = new Dictionary<string, object>();
				}
				param.Add("requestId", value);
				object[] parameters = new object[4]
				{
					action,
					param,
					SDKWebCallbackManager.SharedManager,
					display
				};
				MethodInfo method2 = type.GetMethod("get_sharedManager");
				object obj = method2.Invoke(null, null);
				method.Invoke(obj, parameters);
			}
			else
			{
				Debug.LogError("SDKWebViewManager.openSDKWebView not found");
			}
		}
	}
}
