using System.Collections.Generic;
using LCD.Internal.Interface;
using LCD.Internal.Util;
using UnityEngine;

namespace LCD.Internal.Web
{
	public class SDKWebViewManager : SDKWebViewManagerInternal
	{
		private static SDKWebViewManagerInternal internalInstance;

		private static SDKWebViewManager instance;

		public new static SDKWebViewManager sharedManager
		{
			get
			{
				internalInstance = SDKWebViewManagerInternal.sharedManager;
				if (instance == null)
				{
					instance = new SDKWebViewManager();
				}
				return instance;
			}
			private set
			{
			}
		}

		public new LCDSDK.SDKWebViewProcess currentProcess
		{
			get
			{
				getInternalInstance();
				return internalInstance.currentProcess;
			}
			set
			{
				getInternalInstance();
				internalInstance.currentProcess = value;
			}
		}

		public new LCDUnityEventHandler eventHandler
		{
			get
			{
				getInternalInstance();
				return internalInstance.eventHandler;
			}
			set
			{
				getInternalInstance();
				if (value != null)
				{
					internalInstance.eventHandler = value;
				}
			}
		}

		private void getInternalInstance()
		{
			if (internalInstance == null)
			{
				internalInstance = SDKWebViewManagerInternal.sharedManager;
			}
		}

		public void openSDKWebViewStringAction(string action, Dictionary<string, object> parameters, SDKWebCallbackHandler callback, bool display)
		{
			WebViewAction action2 = WebViewAction.ACTION_CREATE_SESSION;
			if (action != null)
			{
				if ("createSession".Equals(action))
				{
					action2 = WebViewAction.ACTION_CREATE_SESSION;
				}
				else if ("purchase".Equals(action))
				{
					action2 = WebViewAction.ACTION_PURCHASE;
				}
				else if ("linkAccount".Equals(action))
				{
					action2 = WebViewAction.ACTION_LINK_ACCOUNT;
				}
				else if ("loadAccount".Equals(action))
				{
					action2 = WebViewAction.ACTION_LOAD_ACCOUNT;
				}
				else if ("invitation".Equals(action))
				{
					action2 = WebViewAction.ACTION_INVITATION;
				}
			}
			openSDKWebView(action2, parameters, callback, display);
		}

		public void openSDKWebView(WebViewAction action, Dictionary<string, object> parameters, SDKWebCallbackHandler callback, bool display)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.dena.west.lcd.sdk.internal.unity.UnityWrapper");
			androidJavaClass2.CallStatic("openSDKWebView", androidJavaObject, getActionPath(action), Json.Serialize(parameters), callback.GetType().Name);
		}
	}
}
