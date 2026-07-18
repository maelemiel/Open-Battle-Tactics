using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Mobage;
using UnityEngine;

namespace MobageEditor
{
	public class MobageRequest
	{
		private const string apiVersion = "1";

		public INetworkingContext NetworkingContext;

		public string APIMethod = string.Empty;

		public ArrayList Attachments = new ArrayList();

		public string HTTPMethod = "GET";

		public Dictionary<string, string> HTTPHeaders = new Dictionary<string, string>();

		public AuthenticationType AuthenticationType;

		public RequestType RequestType = RequestType.Fromdata;

		public string APIDomain;

		public string EntityTag;

		public bool Secure;

		public OAuth OAuth;

		public Dictionary<string, object> PostBody;

		public Dictionary<string, object> QueryString;

		public Dictionary<string, object> RawPostHash;

		public byte[] RawPostData;

		private HttpWebRequest request;

		public static MobageRequest Request
		{
			get
			{
				return new MobageRequest(MobageSession.CurrentSession);
			}
		}

		public static string PlatformVersion
		{
			get
			{
				return Mobage.sharedInstance.SDKVersion;
			}
		}

		public string StringRepresentationOfPostData
		{
			get
			{
				return string.Join("&", PostBody.Select((KeyValuePair<string, object> x) => OAuth.URLEncodeParameter(x.Key) + "=" + OAuth.URLEncodeParameter((x.Value != null) ? x.Value.ToString() : string.Empty)).ToArray());
			}
		}

		public string RequestURL
		{
			get
			{
				string appId = AppId;
				string text = ((!Secure) ? "http" : "https");
				return string.Format("{0}://{1}/{2}/{3}/{4}", text, APIDomain, "1", appId, APIMethod);
			}
		}

		public string AppId
		{
			get
			{
				return NetworkingContext.AppId;
			}
		}

		public string AppVersion
		{
			get
			{
				return NetworkingContext.OAuthContext.AppVersion;
			}
		}

		public MobageRequest(INetworkingContext networkingContextIn)
		{
			NetworkingContext = networkingContextIn;
			APIDomain = NetworkingContext.SocialServer;
			Secure = NetworkingContext.ServerModeIsProduction;
			OAuthConsumerInfo oAuthConsumerInfo = NetworkingContext.OAuthContext.OAuthConsumerInfo;
			string consumerKey = oAuthConsumerInfo.ConsumerKey;
			string consumerSecret = oAuthConsumerInfo.ConsumerSecret;
			OAuth = new OAuth(consumerKey, consumerSecret);
			OAuth.AccessToken = oAuthConsumerInfo.Token;
			OAuth.AccessSecret = oAuthConsumerInfo.TokenSecret;
			RequestType = RequestType.Fromdata;
		}

		public static MobageRequest NewBankRequestWithContext(MobageSession networkingContext)
		{
			MobageRequest mobageRequest = new MobageRequest(networkingContext);
			string text = "bank";
			if (networkingContext.ServerEnvironment == ServerEnvironment.Sandbox)
			{
				text += "-sandbox";
			}
			switch (networkingContext.ServerStage)
			{
			case ServerStage.Integration:
				mobageRequest.APIDomain = text + ".integration.mobage.com";
				break;
			case ServerStage.Staging:
				mobageRequest.APIDomain = text + ".staging.mobage.com";
				break;
			case ServerStage.Production:
				mobageRequest.APIDomain = text + ".mobage.com";
				break;
			default:
				mobageRequest.APIDomain = "bank-sandbox.mobage.com";
				break;
			}
			mobageRequest.AuthenticationType = AuthenticationType.Oauth2;
			mobageRequest.Secure = true;
			return mobageRequest;
		}

		public void Send(MobageRequestCallbackBlock callback)
		{
			SendInternal(callback);
		}

		private void SendInternal(MobageRequestCallbackBlock callback)
		{
			string hTTPMethod = HTTPMethod;
			string text = RequestURL;
			RawPostHash = PostBody;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string value;
			if (!HTTPHeaders.TryGetValue("Content-Type", out value) && hTTPMethod == "POST")
			{
				value = "application/x-www-form-urlencoded";
			}
			if (hTTPMethod != "POST" || value == "application/x-www-form-urlencoded")
			{
			}
			foreach (KeyValuePair<string, string> item in dictionary)
			{
				Debug.Log(string.Format("oauthParameter: {0}", item));
			}
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			dictionary2.Add("action", text);
			dictionary2.Add("method", hTTPMethod);
			dictionary2.Add("parameters", dictionary);
			Dictionary<string, object> dictionary3 = dictionary2;
			if (hTTPMethod.Contains("?"))
			{
				Debug.Log("Critical OAuth Error: use [request setQueryString: ...] instead of embedding a query string in your API Method. This will probably fail OAuth!");
			}
			switch (RequestType)
			{
			case RequestType.Fromdata:
				OAuth.ContentType = "application/x-www-form-urlencoded";
				break;
			case RequestType.Json:
				OAuth.ContentType = "application/json";
				break;
			}
			if (QueryString != null)
			{
				string text2 = QueryStringFromDictionary(QueryString);
				if (text2.Length > 0)
				{
					text = text + "?" + text2;
				}
				MobageLogger.log(string.Format("New URL: {0}", text));
			}
			request = JSONRequest.MakeRequest(text, null, dictionary3["method"] as string, null);
			if (AuthenticationType == AuthenticationType.Oauth1)
			{
				OAuth.AddSignature(request, RawPostHash, QueryString);
			}
			else if (AuthenticationType == AuthenticationType.Oauth2)
			{
				string oAuth2Token = NetworkingContext.OAuthContext.OAuth2Token;
				if (string.IsNullOrEmpty(oAuth2Token))
				{
					Error error = new Error();
					error.localizedDescription = "No internet connection";
					error.domain = "MB_G_MobageRequest";
					error.code = 10007;
					Error error2 = error;
					return;
				}
				string text3 = "Bearer: " + oAuth2Token;
				request.Headers["Authorization"] = text3;
				Debug.Log(string.Format("MB_G_MobageRequest: request header: {0}={1}", "Authorizattion", text3));
			}
			if (string.IsNullOrEmpty(NetworkingContext.AnalyticsSessionId))
			{
				Debug.Log("NoAuth");
				Error error = new Error();
				error.localizedDescription = "Analytics session not established (probably Mobage is paused, or not initialized)";
				error.domain = "MB_G_MobageRequest";
				error.code = 10007;
				Error error3 = error;
				return;
			}
			string userAgent = string.Format("{0}/{1} ndk/{2} ios/{3}", AppId, AppVersion, PlatformVersion, "6.1.4");
			string appId = Mobage.sharedInstance.AppId;
			if (appId != null)
			{
				appId = appId.ToLower();
				if (appId.Contains("-android"))
				{
					userAgent = string.Format("{0}/{1} ndk/{2} android/{3}", AppId, AppVersion, PlatformVersion, "6.1.4");
				}
			}
			request.UserAgent = userAgent;
			request.Accept = "application/json";
			request.AutomaticDecompression = DecompressionMethods.None;
			request.Headers.Add("Accept-Language", "en");
			request.Headers.Add("Accept-Language", "ja");
			request.Headers.Add("Accept-Language", "fr");
			request.Headers.Add("Accept-Language", "de");
			request.Headers.Add("Accept-Language", "nl");
			NativeAPI instance = NativeAPI.Instance;
			if (!string.IsNullOrEmpty(instance.Cookie))
			{
				request.Headers.Add("Cookie", instance.Cookie);
			}
			foreach (KeyValuePair<string, string> hTTPHeader in HTTPHeaders)
			{
				request.Headers[hTTPHeader.Key] = hTTPHeader.Value;
			}
			request.Headers["X-Stat-Session"] = NetworkingContext.AnalyticsSessionId;
			if (!string.IsNullOrEmpty(EntityTag))
			{
				request.Headers["If-None-Match"] = EntityTag;
			}
			if (RawPostHash != null && hTTPMethod != "GET" && hTTPMethod != "DELETE")
			{
				if (Attachments.Count > 0)
				{
					Debug.Log("Attachments > 0");
				}
				else
				{
					MobageLogger.log("We don't have attachments...");
					switch (RequestType)
					{
					case RequestType.Fromdata:
						request.ContentType = "application/x-www-form-urlencoded";
						RawPostData = Encoding.UTF8.GetBytes(StringRepresentationOfPostData);
						break;
					case RequestType.Json:
						request.ContentType = "application/json";
						RawPostData = Encoding.UTF8.GetBytes(ToJson(PostBody));
						break;
					}
				}
				MobageLogger.log("MobageRequest: post data: " + Encoding.UTF8.GetString(RawPostData));
				request.ContentLength = RawPostData.Length;
				using (Stream stream = request.GetRequestStream())
				{
					stream.Write(RawPostData, 0, RawPostData.Length);
					stream.Close();
				}
			}
			MobageLogger.log("MobageRequest: header data: " + request.Headers.ToString());
			SendPreparedRequest(request, callback);
		}

		public void SendPreparedRequest(HttpWebRequest jsonRequest, MobageRequestCallbackBlock callback)
		{
			try
			{
				using (HttpWebResponse response = (HttpWebResponse)jsonRequest.GetResponse())
				{
					HandleResponse(response, callback);
				}
			}
			catch (WebException ex)
			{
				if (ex.Status == WebExceptionStatus.ProtocolError)
				{
					HandleResponse((HttpWebResponse)ex.Response, callback);
				}
			}
		}

		private void HandleResponse(HttpWebResponse response, MobageRequestCallbackBlock callback)
		{
			string requestURL = RequestURL;
			using (Stream stream = response.GetResponseStream())
			{
				using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
				{
					string text = streamReader.ReadToEnd();
					WebHeaderCollection headers = response.Headers;
					HttpStatusCode statusCode = response.StatusCode;
					MobageLogger.log(string.Format("MB_G_MobageRequest: response headers: {0}", headers));
					MobageLogger.log(string.Format("MB_G_MobageRequest: got a response for {0}", requestURL));
					MobageLogger.log(string.Format("MB_G_MobageRequest: response for {0} is:\n {1}", requestURL, text));
					Error err = null;
					int statusCode2 = (int)response.StatusCode;
					JsonData jsonData = ((!(text == "[]")) ? JsonMapper.ToObject(text) : null);
					if (jsonData != null)
					{
						if (jsonData.Contains("error_msg"))
						{
							Error error = new Error();
							error.localizedDescription = (string)jsonData["error_msg"];
							error.domain = "com.mobage.error.api";
							error.code = (int)jsonData["error"];
							err = error;
						}
						else if (jsonData.Contains("Error"))
						{
							Error error = new Error();
							error.localizedDescription = (string)jsonData["Error"]["Message"];
							error.domain = "com.mobage.error.api";
							error.code = int.Parse((string)jsonData["Error"]["Code"]);
							err = error;
						}
					}
					else if ((statusCode2 < 200 || statusCode2 >= 300) && statusCode2 != 304)
					{
						Error error = new Error();
						error.domain = "MB_G_MobageRequest";
						error.code = 10001;
						err = error;
					}
					MobageLogger.log("response data collected: " + text);
					callback(err, jsonData, headers, statusCode);
				}
			}
		}

		private string ToJson(object obj)
		{
			if (obj == null)
			{
				return "{}";
			}
			if (obj is Dictionary<string, object>)
			{
				string[] value = (obj as Dictionary<string, object>).Select((KeyValuePair<string, object> d) => string.Format("\"{0}\":{1}", d.Key, ToJson(d.Value))).ToArray();
				return "{" + string.Join(",", value) + "}";
			}
			if (obj is Dictionary<string, string>)
			{
				string[] value2 = (obj as Dictionary<string, string>).Select((KeyValuePair<string, string> d) => string.Format("\"{0}\":\"{1}\"", d.Key, d.Value)).ToArray();
				return "{" + string.Join(",", value2) + "}";
			}
			if (obj is List<object>)
			{
				string[] value3 = (obj as List<object>).Select((object d) => ToJson(d)).ToArray();
				return "[" + string.Join(",", value3) + "]";
			}
			if (obj is string)
			{
				return string.Format("\"{0}\"", obj);
			}
			return string.Format("\"{0}\"", obj);
		}

		public string QueryStringFromDictionary(Dictionary<string, object> dictionary)
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, object> item in dictionary)
			{
				string key = OAuth.URLEncodeParameter(item.Key);
				list.Add(QueryStringPartFromKey(key, item.Value.ToString()));
			}
			return string.Join("&", list.ToArray());
		}

		public string QueryStringPartFromKey(string key, string val)
		{
			return string.Format("{0}={1}", OAuth.URLEncodeParameter(key), OAuth.URLEncodeParameter(val));
		}
	}
}
