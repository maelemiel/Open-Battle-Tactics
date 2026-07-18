using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Timers;
using LCD.Internal.Impl;
using LCD.Internal.Model;
using LCD.Internal.Util;
using UnityEngine;

namespace LCD.Internal.Web
{
	[ExecuteInEditMode]
	public class LCDWeb
	{
		private const int mWebViewLoadingTimeOut = 10000;

		private static string TAG = "LCDWeb";

		private static Timer updateTimer;

		private static bool loading = false;

		private static readonly LCDWeb instance = new LCDWeb();

		private Dictionary<string, object> parameters;

		private static LCDWebView webView;

		private static SDKWebCallbackHandler callback;

		private bool callbackSet;

		internal LCDWebView WebViewObject
		{
			get
			{
				return webView;
			}
			set
			{
				if (webView == null)
				{
					webView = value;
				}
			}
		}

		private SDKWebCallbackHandler Callback
		{
			get
			{
				return callback;
			}
			set
			{
				callback = value;
				callbackSet = true;
			}
		}

		public static LCDWeb Instance
		{
			get
			{
				return instance;
			}
		}

		private LCDWeb()
		{
			if (parameters != null)
			{
			}
		}

		private static void LCDCallback(string message)
		{
			if (message != null && Instance.callbackSet)
			{
				Instance.Callback.onFailure(new LCDErrorImpl("LCDWebView", 500, message).ToJsonString());
				Instance.reset();
			}
		}

		public static void OpenLCDWeb(string url, Dictionary<string, object> parameters, SDKWebCallbackHandler webCallback, bool display)
		{
			LCDWeb lCDWeb = Instance;
			ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, (RemoteCertificateValidationCallback)((object s, X509Certificate ce, X509Chain ca, SslPolicyErrors p) => true));
			if (parameters != null && parameters.Count > 0)
			{
				lCDWeb.parameters = parameters;
			}
			try
			{
				GameObject gameObject = GameObject.Find("LCDWebView");
				if (gameObject != null)
				{
					lCDWeb.WebViewObject = gameObject.GetComponent<LCDWebView>();
				}
				else
				{
					GameObject gameObject2 = new GameObject("LCDWebView");
					lCDWeb.WebViewObject = gameObject2.AddComponent<LCDWebView>();
				}
				lCDWeb.Callback = webCallback;
				lCDWeb.WebViewObject.Display = display;
				Action<string> cb = LCDCallback;
				lCDWeb.WebViewObject.Init(0, 0, Screen.width, Screen.height, lCDWeb.WebViewObject.Display, cb);
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary["Authorization"] = "Bearer " + Credentials.accessToken;
				dictionary["X-LCD-BundleId"] = Capabilities.sharedManager.bundleId;
				dictionary["X-LCD-SignatureHash"] = Capabilities.sharedManager.storeType.ToString();
				string headers = Json.Serialize(dictionary);
				updateTimer = new Timer(10000.0);
				updateTimer.Elapsed += OnUpdateTimer;
				updateTimer.Start();
				loading = true;
				lCDWeb.WebViewObject.LoadURL(url, headers);
				lCDWeb.WebViewObject.SetVisibility(true);
			}
			catch (WebException ex)
			{
				LCDErrorImpl lCDErrorImpl;
				if (ex.Response != null)
				{
					lCDErrorImpl = new LCDErrorImpl(LCDError.ErrorType.NETWORK_ERROR.ToString(), (int)((HttpWebResponse)ex.Response).StatusCode, ((HttpWebResponse)ex.Response).StatusDescription);
					LCDSDKLog.Error(TAG, "Network Error: " + (int)((HttpWebResponse)ex.Response).StatusCode, lCDErrorImpl);
				}
				else
				{
					lCDErrorImpl = new LCDErrorImpl(LCDError.ErrorType.NETWORK_ERROR.ToString(), -1, ex.Message);
					LCDSDKLog.Error(TAG, "Network Error: -1", lCDErrorImpl);
				}
				Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
				dictionary2.Add("error", (Dictionary<string, object>)Json.Deserialize(lCDErrorImpl.ToJsonString()));
				Instance.ResetSDKWebView(dictionary2);
			}
		}

		private static void OnUpdateTimer(object source, ElapsedEventArgs e)
		{
			updateTimer.Stop();
			updateTimer = null;
			if (loading)
			{
				string errorMessage = "Content can't be loaded.";
				LCDErrorImpl lCDErrorImpl = new LCDErrorImpl(LCDError.ErrorType.NETWORK_ERROR.ToString(), -1, errorMessage);
				LCDSDKLog.Error(TAG, "Network Error: -1", lCDErrorImpl);
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary.Add("error", (Dictionary<string, object>)Json.Deserialize(lCDErrorImpl.ToJsonString()));
				Instance.ResetSDKWebView(dictionary);
			}
		}

		internal bool HandleRequest(string url, Action<string> callback)
		{
			bool result = false;
			Uri uri = new Uri(url);
			if (uri.Scheme == "lcd")
			{
				if (loading)
				{
					loading = false;
				}
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				if (!string.IsNullOrEmpty(uri.Query))
				{
					string[] array = uri.Query.TrimStart('?').Split('&');
					foreach (string text in array)
					{
						string[] array2 = text.Split('=');
						dictionary[array2[0]] = array2[1];
					}
				}
				result = true;
				string[] array3 = uri.Query.Split('=');
				string text2 = null;
				if (array3 != null && array3.Length > 1)
				{
					text2 = array3[1];
					text2 = text2.Replace("+", " ");
				}
				Dictionary<string, object> json = null;
				if (text2 != null)
				{
					json = (Dictionary<string, object>)Json.Deserialize(WWW.UnEscapeURL(text2));
				}
				string requestId = uri.AbsolutePath.TrimStart('/');
				Action<string, int, Dictionary<string, object>> action = delegate(string id, int code, Dictionary<string, object> obj)
				{
					string js2 = string.Format("javascript:respondToWeb('{0}', '{1}', {2})", requestId, code, Json.Serialize(obj));
					LCDWebView webViewObject2 = WebViewObject;
					webViewObject2.EvaluateJS(js2);
				};
				if (uri.Host != "logger")
				{
					LCDSDKLog.Debug(TAG, "Request " + requestId + ": " + uri.Host + " " + uri.Query);
				}
				if (uri.Host == "showwebview")
				{
					SDKWebViewManagerInternal sharedManager = SDKWebViewManagerInternal.sharedManager;
					if (sharedManager.currentProcess != LCDSDK.SDKWebViewProcess.APPEARED)
					{
						Instance.WebViewObject.Display = true;
						LCDWebView webViewObject = WebViewObject;
						string js = "javascript:onAttachedWebView()";
						webViewObject.EvaluateJS(js);
						sharedManager.currentProcess = LCDSDK.SDKWebViewProcess.APPEARED;
						sharedManager.eventHandler.OnSDKWebViewProcess(sharedManager.currentProcess.ToString());
					}
					return false;
				}
				if (uri.Host == "logger")
				{
					string absolutePath = uri.AbsolutePath;
					if (text2 != null)
					{
						if ("/warn".Equals(absolutePath))
						{
							LCDSDKLog.Warn("LOGGER", text2, null);
						}
						else if ("/info".Equals(absolutePath))
						{
							LCDSDKLog.Info("LOGGER", text2);
						}
						else if ("/debug".Equals(absolutePath))
						{
							LCDSDKLog.Debug("LOGGER", text2);
						}
					}
				}
				if (uri.Host == "getcapabilities")
				{
					NativeAPIImpl.GetCapabilities(requestId, action);
				}
				else if (uri.Host == "setstoretype")
				{
					NativeAPIImpl.GetCapabilities(requestId, action);
				}
				else if (uri.Host == "getrequestparameter")
				{
					action(requestId, 200, parameters);
				}
				else if (uri.Host == "getcredentials")
				{
					NativeAPIImpl.GetCredentials(requestId, action);
				}
				else if (uri.Host == "resetkeychain")
				{
					NativeAPIImpl.ResetKeyChain(requestId, action);
				}
				else if (uri.Host == "savesessionresponse")
				{
					NativeAPIImpl.SaveSessionResponse(json, requestId, action);
				}
				else if (uri.Host == "closewebview")
				{
					ResetSDKWebView(json);
				}
			}
			return result;
		}

		internal void ResetSDKWebView(Dictionary<string, object> json)
		{
			reset();
			if (!Instance.callbackSet)
			{
				return;
			}
			object value = null;
			if (Instance.parameters != null)
			{
				Instance.parameters.TryGetValue("requestId", out value);
			}
			if (json != null)
			{
				if (json.ContainsKey("error") && json["error"] != null)
				{
					Dictionary<string, object> dictionary = (Dictionary<string, object>)json["error"];
					string errorType = (string)dictionary["type"];
					int errorCode = -1;
					if (dictionary["code"] != null)
					{
						errorCode = Convert.ToInt32(dictionary["code"]);
					}
					string errorMessage = null;
					if (dictionary["message"] != null)
					{
						errorMessage = (string)dictionary["message"];
					}
					LCDErrorImpl lCDErrorImpl = new LCDErrorImpl(errorType, errorCode, errorMessage);
					Instance.Callback.onFailure(lCDErrorImpl.ToJsonString(value));
				}
				else
				{
					if (value != null)
					{
						json.Add("requestId", value);
					}
					Instance.Callback.onSuccess(Json.Serialize(json));
				}
			}
			else if (value != null)
			{
				json = new Dictionary<string, object>();
				json.Add("requestId", value);
				Instance.Callback.onSuccess(Json.Serialize(json));
			}
			else
			{
				Instance.Callback.onSuccess(string.Empty);
			}
			Instance.Callback = null;
		}

		internal void reset()
		{
			SDKWebViewManagerInternal.sharedManager.currentProcess = LCDSDK.SDKWebViewProcess.FINISHED;
			SDKWebViewManagerInternal.sharedManager.eventHandler.OnSDKWebViewProcess(LCDSDK.SDKWebViewProcess.FINISHED.ToString());
			try
			{
				if (webView != null)
				{
					webView.OnDestroy();
				}
			}
			catch (Exception)
			{
			}
		}
	}
}
