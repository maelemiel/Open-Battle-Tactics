using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.adjust.sdk
{
	public class AdjustAndroid : IAdjust
	{
		private AndroidJavaClass ajcAdjust;

		private AndroidJavaClass ajcAdjustUnity;

		private AndroidJavaObject ajoCurrentActivity;

		public AdjustAndroid()
		{
			ajcAdjust = new AndroidJavaClass("com.adjust.sdk.Adjust");
			ajcAdjustUnity = new AndroidJavaClass("com.adjust.sdk.AdjustUnity");
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			ajoCurrentActivity = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		}

		public void appDidLaunch(string appToken, AdjustUtil.AdjustEnvironment environment, string sdkPrefix, AdjustUtil.LogLevel logLevel, bool eventBuffering)
		{
			string text = environment.ToString().ToLower();
			string text2 = logLevel.ToString().ToLower();
			ajcAdjust.CallStatic("appDidLaunch", ajoCurrentActivity, appToken, text, text2, eventBuffering);
			ajcAdjust.CallStatic("setSdkPrefix", sdkPrefix);
			onResume();
		}

		public void trackEvent(string eventToken, Dictionary<string, string> parameters = null)
		{
			AndroidJavaObject androidJavaObject = ConvertDicToJava(parameters);
			ajcAdjust.CallStatic("trackEvent", eventToken, androidJavaObject);
		}

		public void trackRevenue(double cents, string eventToken = null, Dictionary<string, string> parameters = null)
		{
			AndroidJavaObject androidJavaObject = ConvertDicToJava(parameters);
			ajcAdjust.CallStatic("trackRevenue", cents, eventToken, androidJavaObject);
		}

		public void onPause()
		{
			ajcAdjust.CallStatic("onPause");
		}

		public void onResume()
		{
			ajcAdjust.CallStatic("onResume", ajoCurrentActivity);
		}

		public void setResponseDelegate(string sceneName)
		{
			ajcAdjustUnity.CallStatic("setResponseDelegate", sceneName);
		}

		public void setResponseDelegateString(Action<string> responseDelegate)
		{
		}

		public void setEnabled(bool enabled)
		{
			ajcAdjust.CallStatic("setEnabled", ConvertBoolToJava(enabled));
		}

		public bool isEnabled()
		{
			AndroidJavaObject ajo = ajcAdjust.CallStatic<AndroidJavaObject>("isEnabled", new object[0]);
			bool? flag = ConvertBoolFromJava(ajo);
			return flag.HasValue && flag.Value;
		}

		private AndroidJavaObject ConvertBoolToJava(bool value)
		{
			return new AndroidJavaObject("java.lang.Boolean", value.ToString().ToLower());
		}

		private bool? ConvertBoolFromJava(AndroidJavaObject ajo)
		{
			if (ajo == null)
			{
				return null;
			}
			string value = ajo.Call<string>("toString", new object[0]);
			try
			{
				return Convert.ToBoolean(value);
			}
			catch (FormatException)
			{
				return null;
			}
		}

		private AndroidJavaObject ConvertDicToJava(Dictionary<string, string> dictonary)
		{
			if (dictonary == null)
			{
				return null;
			}
			AndroidJavaObject androidJavaObject = new AndroidJavaObject("java.util.HashMap", dictonary.Count);
			foreach (KeyValuePair<string, string> item in dictonary)
			{
				if (item.Value != null)
				{
					androidJavaObject.Call<string>("put", new object[2] { item.Key, item.Value });
				}
			}
			return androidJavaObject;
		}
	}
}
