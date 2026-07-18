using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using ICSharpCode.SharpZipLib.GZip;
using OAuth;

public class NetworkQueue : AsyncQueue<NetworkQueue, NetworkQueue.Request, NetworkQueue.Response>
{
	public class Request : BaseRequest
	{
		public string url;

		public string method;

		public bool useOAuth;

		public string oauthKey;

		public string oauthKeySecret;

		public string oauthToken;

		public string oauthTokenSecret;

		public Dictionary<string, string> headers = new Dictionary<string, string>();

		public string body = string.Empty;

		public object userArg;

		public bool trace;

		public bool profile;

		public bool retry = true;

		public int retries;

		public float showLoadingPopupDelay = 6f;

		public int loadingPopupId = -1;

		public float debugLongDelayProb;

		public float debugLongDelaySeconds = 30f;

		public float debugNotReachServerProb;

		public float debugLoseResponseProb;

		public Request(string url, string method, int retries)
		{
			this.url = url;
			this.method = method;
			this.retries = retries;
			queued = false;
			trace = AppConfig.traceNetworkRequests;
		}

		public void OAuth(string oauthKey, string oauthKeySecret, string oauthToken, string oauthTokenSecret)
		{
			useOAuth = true;
			this.oauthKey = oauthKey;
			this.oauthKeySecret = oauthKeySecret;
			this.oauthToken = oauthToken;
			this.oauthTokenSecret = oauthTokenSecret;
		}

		public void Callback(Callback callback)
		{
			base.callback = callback;
		}
	}

	public class TutorialRequest : Request
	{
		public string fakeJsonResponse;

		public TutorialRequest()
			: base(null, null, 0)
		{
		}
	}

	public class Response : BaseResponse
	{
		public string error;

		public HttpStatusCode httpStatusCode = HttpStatusCode.Unused;

		public string httpStatusDescription = "Unused";

		public Dictionary<string, string> headers = new Dictionary<string, string>();

		public string bodyString;

		public byte[] bodyBytes;
	}

	private Random rand;

	public NetworkQueue()
	{
		ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback(ValidateRemoteCertificate));
	}

	private static bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
	{
		return true;
	}

	protected override void ProcessRequest(Request request, Response response)
	{
		TutorialRequest tutorialRequest = request as TutorialRequest;
		if (tutorialRequest != null)
		{
			ProcessTutorialRequest(tutorialRequest, response);
		}
		else
		{
			ProcessNetworkRequest(request, response);
		}
	}

	protected void ProcessTutorialRequest(TutorialRequest request, Response response)
	{
		Log.DebugTag("ProcessTutorialRequest", null, "NetworkRequest");
		response.bodyString = request.fakeJsonResponse;
	}

	protected void ProcessNetworkRequest(Request request, Response response)
	{
		try
		{
			StringBuilder stringBuilder = null;
			if (request.trace)
			{
				stringBuilder = new StringBuilder();
			}
			long num = 0L;
			if (request.profile)
			{
				num = Stopwatch.GetTimestamp();
			}
			long num2 = num;
			string text = request.url;
			if (request.method == "GET" && !string.IsNullOrEmpty(request.body))
			{
				text = text + "?" + request.body;
			}
			HttpWebRequest httpWebRequest = WebRequest.Create(text) as HttpWebRequest;
			httpWebRequest.Method = request.method;
			httpWebRequest.Timeout = AppConfig.networkTimeout;
			httpWebRequest.Headers["Accept-Encoding"] = "gzip";
			if (request.trace)
			{
				stringBuilder.AppendLine("NetworkQueue Request");
				stringBuilder.AppendLine(string.Format("- {0} {1}", request.method, request.url));
				stringBuilder.AppendLine("- Headers:");
			}
			if (request.profile)
			{
				long timestamp = Stopwatch.GetTimestamp();
				Log.InfoTag("Request allocation: " + (float)(timestamp - num2) / 10000000f, null, "NetworkRequest");
				num2 = timestamp;
			}
			foreach (KeyValuePair<string, string> header in request.headers)
			{
				string key = header.Key;
				string value = header.Value;
				switch (key)
				{
				case "Accept":
					httpWebRequest.Accept = value;
					break;
				default:
					httpWebRequest.Headers[key] = value;
					break;
				}
				if (request.trace)
				{
					stringBuilder.AppendLine(string.Format("- | {0}: {1}", key, value));
				}
			}
			if (request.profile)
			{
				long timestamp2 = Stopwatch.GetTimestamp();
				Log.InfoTag("Request headers: " + (float)(timestamp2 - num2) / 10000000f, null, "NetworkRequest");
				num2 = timestamp2;
			}
			if (request.useOAuth)
			{
				string nonce = Manager.GenerateNonce();
				string timeStamp = Manager.GenerateTimeStamp();
				Uri url = new Uri(text);
				string signature = Manager.GenerateSignature(url, request.oauthKey, request.oauthKeySecret, request.oauthToken, request.oauthTokenSecret, request.method, timeStamp, nonce);
				string text2 = Manager.GenerateAuthorizationHeader(url, request.oauthKey, request.oauthToken, timeStamp, nonce, "HMAC-SHA1", signature);
				httpWebRequest.Headers.Add("Authorization", text2);
				if (request.trace)
				{
					string text3 = Manager.GenerateSignatureBase(url, request.oauthKey, request.oauthToken, request.oauthTokenSecret, request.method, timeStamp, nonce, "HMAC-SHA1");
					stringBuilder.AppendLine("- | Authorization: " + text2);
					stringBuilder.AppendLine("- BaseString: " + text3);
					stringBuilder.AppendLine("- OAuthKey: " + request.oauthKey + " / " + request.oauthKeySecret);
					stringBuilder.AppendLine("- OAuthKToken: " + request.oauthToken + " / " + request.oauthTokenSecret);
				}
				if (request.profile)
				{
					long timestamp3 = Stopwatch.GetTimestamp();
					Log.InfoTag("Request oauth: " + (float)(timestamp3 - num2) / 10000000f, null, "NetworkRequest");
				}
			}
			if (request.debugNotReachServerProb != 0f && RandomFloat() < request.debugNotReachServerProb)
			{
				Thread.Sleep(3000);
				throw new WebException("debugNotReachServerProb triggered", WebExceptionStatus.ConnectFailure);
			}
			if (request.method != "GET" && !string.IsNullOrEmpty(request.body))
			{
				if (!request.url.Contains("mobage"))
				{
					httpWebRequest.ContentType = "application/json";
				}
				byte[] bytes = Encoding.UTF8.GetBytes(request.body);
				httpWebRequest.ContentLength = bytes.Length;
				using (Stream stream = httpWebRequest.GetRequestStream())
				{
					stream.Write(bytes, 0, bytes.Length);
				}
			}
			if (request.trace)
			{
				stringBuilder.AppendLine("- Body: " + request.body);
				Log.Info(stringBuilder.ToString());
				stringBuilder.Length = 0;
			}
			if (request.profile)
			{
				long timestamp4 = Stopwatch.GetTimestamp();
				Log.InfoTag("Request body: " + (float)(timestamp4 - num2) / 10000000f, null, "NetworkRequest");
				num2 = timestamp4;
			}
			using (HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse)
			{
				response.httpStatusCode = httpWebResponse.StatusCode;
				response.httpStatusDescription = httpWebResponse.StatusDescription;
				if (request.trace)
				{
					stringBuilder.AppendLine("NetworkQueue Response");
					stringBuilder.AppendLine("- Status: " + response.httpStatusDescription);
					stringBuilder.AppendLine("- Headers:");
					stringBuilder.AppendLine("- | ContenLength: " + httpWebResponse.ContentLength);
					stringBuilder.AppendLine("- | ContenType: " + httpWebResponse.ContentType);
				}
				if (request.profile)
				{
					long timestamp5 = Stopwatch.GetTimestamp();
					Log.InfoTag("Response get: " + (float)(timestamp5 - num2) / 10000000f, null, "NetworkRequest");
					num2 = timestamp5;
				}
				for (int i = 0; i < httpWebResponse.Headers.Count; i++)
				{
					string text4 = httpWebResponse.Headers.Keys[i];
					string text5 = httpWebResponse.Headers[i];
					response.headers[text4] = text5;
					if (request.trace)
					{
						stringBuilder.AppendLine("- | " + text4 + ": " + text5);
					}
				}
				if (request.profile)
				{
					long timestamp6 = Stopwatch.GetTimestamp();
					Log.InfoTag("Response headers: " + (float)(timestamp6 - num2) / 10000000f, null, "NetworkRequest");
					num2 = timestamp6;
				}
				if (response.httpStatusCode == HttpStatusCode.OK)
				{
					Stream stream2 = httpWebResponse.GetResponseStream();
					if (httpWebResponse.ContentEncoding.ToLower() == "gzip")
					{
						if (request.trace)
						{
							stringBuilder.AppendLine("# Using GZIP");
						}
						stream2 = new GZipInputStream(stream2);
					}
					if (httpWebResponse.ContentType == "application/octet-stream")
					{
						using (BinaryReader binaryReader = new BinaryReader(stream2))
						{
							response.bodyBytes = binaryReader.ReadBytes((int)httpWebResponse.ContentLength);
						}
					}
					else
					{
						using (StreamReader streamReader = new StreamReader(stream2, Encoding.GetEncoding(httpWebResponse.CharacterSet)))
						{
							response.bodyString = streamReader.ReadToEnd();
						}
						if (request.trace)
						{
							stringBuilder.AppendLine("- Body: " + response.bodyString);
						}
					}
				}
				else
				{
					response.error = "Http error: " + response.httpStatusCode;
				}
				httpWebResponse.Close();
				if (request.profile)
				{
					long timestamp7 = Stopwatch.GetTimestamp();
					Log.InfoTag("Response body: " + (float)(timestamp7 - num2) / 10000000f, null, "NetworkRequest");
					Log.InfoTag("Total: " + (float)(timestamp7 - num) / 10000000f, null, "NetworkRequest");
				}
				if (request.trace)
				{
					Log.InfoTag(stringBuilder.ToString(), null, "NetworkQueue");
				}
			}
			if (request.debugLongDelayProb != 0f && RandomFloat() < request.debugLongDelayProb)
			{
				Log.Warning("debugLongDelayProb triggered, sleeping thread for: {0}s", request.debugLongDelaySeconds);
				Thread.Sleep((int)(request.debugLongDelaySeconds * 1000f));
			}
			if (request.debugLoseResponseProb != 0f && RandomFloat() < request.debugLoseResponseProb)
			{
				throw new WebException("debugLoseResponseProb triggered", WebExceptionStatus.ReceiveFailure);
			}
		}
		catch (Exception ex)
		{
			WebException ex2 = ex as WebException;
			if (ex2 != null)
			{
				if (request.retry && --request.retries >= 0)
				{
					Log.WarningTag("Error: " + ex.ToString(), null, "NetworkQueue");
					Log.WarningTag("WData: " + ex2.Message, null, "NetworkQueue");
					Log.Warning("Connection error: {0}, will retry {1} times.", ex2.Status.ToString(), request.retries + 1);
					ProcessRequest(request, response);
					return;
				}
				response.error = "WebException: " + ex;
				if (ex2.Response != null)
				{
					using (Stream stream3 = ex2.Response.GetResponseStream())
					{
						using (StreamReader streamReader2 = new StreamReader(stream3))
						{
							string text6 = streamReader2.ReadToEnd();
							if (text6.Contains("clocksync"))
							{
								response.error = "WebException: clocksync";
							}
							Log.ErrorTag("WebException, Body content " + text6, null, "NetworkQueue");
							return;
						}
					}
				}
				Log.ErrorTag("WebException, Empty Response", null, "NetworkQueue");
			}
			else
			{
				response.error = "Exception: " + ex;
			}
		}
	}

	private float RandomFloat()
	{
		if (rand == null)
		{
			rand = new Random();
		}
		float num = (float)rand.NextDouble();
		Log.Debug("RandomFloat: " + num);
		return num;
	}
}
