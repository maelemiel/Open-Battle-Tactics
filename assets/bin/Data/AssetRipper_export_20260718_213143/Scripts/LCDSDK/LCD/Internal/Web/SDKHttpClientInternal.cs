using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using LCD.Internal.Impl;
using LCD.Internal.Model;
using LCD.Internal.Util;

namespace LCD.Internal.Web
{
	public class SDKHttpClientInternal
	{
		private class AuthRetryCallback : SDKWebCallbackHandler
		{
			private string method;

			private string path;

			private Dictionary<string, string> param;

			private Dictionary<string, object> data;

			private SDKWebCallbackHandler callback;

			private bool retry;

			internal AuthRetryCallback(string method, string path, Dictionary<string, string> param, Dictionary<string, object> data, SDKWebCallbackHandler callback, bool retry)
			{
				this.method = method;
				this.path = path;
				this.param = param;
				this.data = data;
				this.callback = callback;
				this.retry = retry;
			}

			public void onSuccess(string message)
			{
				if (path.Contains("session"))
				{
					callback.onSuccess(message);
				}
				else
				{
					ExecuteUnity(method, path, param, data, callback, retry);
				}
			}

			public void onFailure(string message)
			{
				LCDSDKLog.Info(TAG, "AuthRetryCallback onFailure");
				LCDErrorImpl lCDErrorImpl = new LCDErrorImpl(LCDError.ErrorType.LCD_ERROR, 401, message);
				if (callback != null)
				{
					callback.onFailure(lCDErrorImpl.ToJsonString());
				}
			}
		}

		private static string TAG = "SDKHttpClient";

		public static void ExecuteUnity(string method, string path, Dictionary<string, string> param, Dictionary<string, object> data, SDKWebCallbackHandler callback)
		{
			ExecuteUnity(method, path, param, data, callback, false);
		}

		internal static void ExecuteUnity(string method, string path, Dictionary<string, string> param, Dictionary<string, object> data, SDKWebCallbackHandler callback, bool retry)
		{
			LCDSDKLog.Debug(TAG, string.Format("ExecuteUnity: {0} {1} {2} {3} (retry={4})", method, path, param, data, retry));
			string text = string.Empty;
			string value = null;
			if (param != null)
			{
				int num = 0;
				foreach (KeyValuePair<string, string> item in param)
				{
					text = ((num != 0) ? (text + "&") : "?");
					text += item.Key;
					text += "=";
					text += item.Value;
					num++;
				}
				param.TryGetValue("requestId", out value);
			}
			string requestUriString = SDKWebViewManagerInternal.sharedManager.getWebServerHost() + path + text;
			ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, (RemoteCertificateValidationCallback)((object s, X509Certificate ce, X509Chain ca, SslPolicyErrors p) => true));
			try
			{
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUriString);
				httpWebRequest.Method = method;
				httpWebRequest.Timeout = 10000;
				httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + Credentials.accessToken);
				httpWebRequest.Headers.Add("X-LCD-BundleId", Capabilities.sharedManager.bundleId);
				httpWebRequest.Headers.Add("X-LCD-SignatureHash", Capabilities.sharedManager.storeType.ToString());
				httpWebRequest.Accept = "application/json";
				httpWebRequest.ContentType = "application/json";
				httpWebRequest.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 6_1_4 like Mac OS X) AppleWebKit/536.26 (KHTML, like Gecko) Mobile/10B350";
				if ("POST".Equals(method) && data != null)
				{
					using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
					{
						string value2 = Json.Serialize(data);
						streamWriter.Write(value2);
						streamWriter.Flush();
						streamWriter.Close();
					}
				}
				HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				using (Stream stream = httpWebResponse.GetResponseStream())
				{
					using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
					{
						string text2 = streamReader.ReadToEnd();
						int statusCode = (int)httpWebResponse.StatusCode;
						if (statusCode >= 200 && statusCode < 300)
						{
							if (callback != null)
							{
								if (value != null)
								{
									Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(text2);
									dictionary.Add("requestId", value);
									callback.onSuccess(Json.Serialize(dictionary));
								}
								else
								{
									callback.onSuccess(text2);
								}
							}
						}
						else
						{
							LCDErrorImpl lCDErrorImpl = new LCDErrorImpl(LCDError.ErrorType.LCD_ERROR, statusCode, text2);
							if (callback != null)
							{
								callback.onFailure(lCDErrorImpl.ToJsonString(value));
							}
						}
					}
				}
			}
			catch (WebException ex)
			{
				if (ex.Response == null)
				{
					string errorMessage = "Content can't be loaded.";
					LCDErrorImpl lCDErrorImpl2 = new LCDErrorImpl(LCDError.ErrorType.NETWORK_ERROR.ToString(), -1, errorMessage);
					LCDSDKLog.Error(TAG, "Network Error: -1", lCDErrorImpl2);
					if (callback != null)
					{
						callback.onFailure(lCDErrorImpl2.ToJsonString(value));
					}
					return;
				}
				HttpStatusCode statusCode2 = ((HttpWebResponse)ex.Response).StatusCode;
				string errorMessage2 = ex.Message;
				LCDSDKLog.Debug(TAG, method + " " + path + " returned " + statusCode2);
				switch (statusCode2)
				{
				case HttpStatusCode.Unauthorized:
					Credentials.accessToken = null;
					if (!retry)
					{
						SDKWebCallbackHandler callback2 = new AuthRetryCallback(method, path, param, data, callback, retry);
						Credentials.GetSession(Credentials.SessionAction.UPDATE, callback2);
						return;
					}
					break;
				case HttpStatusCode.ServiceUnavailable:
					errorMessage2 = "This application is temporarily over its serving quota. Please try again later.";
					break;
				}
				LCDErrorImpl lCDErrorImpl3 = new LCDErrorImpl(LCDError.ErrorType.NETWORK_ERROR, (int)statusCode2, errorMessage2);
				if (callback != null)
				{
					callback.onFailure(lCDErrorImpl3.ToJsonString(value));
				}
			}
		}
	}
}
