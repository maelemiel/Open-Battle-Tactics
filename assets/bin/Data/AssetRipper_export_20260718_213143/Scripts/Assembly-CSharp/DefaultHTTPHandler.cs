using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Timers;

public class DefaultHTTPHandler : HTTPHandler
{
	private ThreadPoolManager _threadPoolManager;

	private string _path;

	private string _body;

	private Dictionary<string, string> _headers = new Dictionary<string, string>();

	private Dictionary<string, string> _params = new Dictionary<string, string>();

	private int _max_retries = 3;

	private int _attempt;

	private int _timeout = 10000;

	private bool _highPriority;

	private HttpWebRequest _httpRequest;

	private bool _cancelled;

	private static Dictionary<string, string> HEADER_PROPERTIES_MAP = new Dictionary<string, string>
	{
		{ "Accept", null },
		{ "Content-Type", "ContentType" },
		{ "Content-Length", "ContentLength" },
		{ "Expect", null },
		{ "Date", null },
		{ "Connection", null },
		{ "Host", null },
		{ "If-Modified-Since", "IfModifiedSince" },
		{ "Referer", null },
		{ "Transfer-Encoding", "TransferEncoding" },
		{ "User-Agent", "UserAgent" }
	};

	private bool Cancelled
	{
		get
		{
			if (_cancelled)
			{
				Log.WarningTag("Request Cancelled: " + _path, null, "Networklayer");
				return true;
			}
			return false;
		}
	}

	public DefaultHTTPHandler(ThreadPoolManager threadPoolManager)
	{
		_threadPoolManager = threadPoolManager;
	}

	public void Body(string body)
	{
		_body = body;
	}

	public void Path(string path)
	{
		_path = path;
	}

	public void Header(string key, string value)
	{
		_headers[key] = value;
	}

	public void Param(string key, string value)
	{
		_params[key] = value;
	}

	public void Timeout(int timeout)
	{
		_timeout = timeout;
	}

	public void Retries(int retries)
	{
		_max_retries = retries;
	}

	public string CreateQueryString()
	{
		List<string> list = new List<string>(_params.Keys);
		if (list.Count == 0)
		{
			return string.Empty;
		}
		string text = (_path.Contains("?") ? "&" : "?");
		for (int i = 0; i < list.Count; i++)
		{
			string text2 = list[i];
			string arg = _params[text2];
			text += string.Format("{0}={1}", text2, arg);
			if (i < list.Count - 1)
			{
				text += "&";
			}
		}
		return text;
	}

	public void HighPriority(bool highPriority)
	{
		_highPriority = highPriority;
	}

	public void Cancel()
	{
		_cancelled = true;
		if (_httpRequest != null)
		{
			_httpRequest.Abort();
		}
	}

	public void Exec(string method, Action<object, object> callback)
	{
		Action<Action> httpAction = delegate(Action done)
		{
			if (Cancelled)
			{
				done();
				return;
			}
			_path += CreateQueryString();
			_httpRequest = WebRequest.Create(_path) as HttpWebRequest;
			_httpRequest.Timeout = _timeout;
			_httpRequest.Method = method;
			_attempt++;
			SetupHeaders(_httpRequest);
			try
			{
				if (!string.IsNullOrEmpty(_body))
				{
					byte[] bytes = Encoding.UTF8.GetBytes(_body);
					_httpRequest.ContentLength = bytes.Length;
					using (Stream stream = _httpRequest.GetRequestStream())
					{
						stream.Write(bytes, 0, bytes.Length);
					}
				}
				using (HttpWebResponse httpWebResponse = _httpRequest.GetResponse() as HttpWebResponse)
				{
					if (!Cancelled)
					{
						callback(httpWebResponse, null);
					}
					_threadPoolManager.InternetAvailable();
					httpWebResponse.Close();
					done();
				}
			}
			catch (WebException ex)
			{
				if (!Cancelled)
				{
					Log.WarningTag(string.Concat("Error calling ", _path, ":", ex.Message, ", STATUS: ", ex.Status, ", type: ", ex.GetType()), null, "Networklayer");
					HttpWebResponse httpWebResponse2 = ex.Response as HttpWebResponse;
					if (ConnectionNotAvailable(ex))
					{
						_threadPoolManager.NetworkNotAvailable();
						_attempt = 0;
						Timer timer = new Timer(1000.0);
						timer.Elapsed += delegate
						{
							httpAction(done);
						};
						timer.Enabled = true;
						timer.AutoReset = false;
						timer.Start();
						return;
					}
					if (httpWebResponse2 == null || httpWebResponse2.StatusCode == HttpStatusCode.RequestTimeout)
					{
						if (_attempt < _max_retries)
						{
							Log.DebugTag("Retrying... Attempt No " + _attempt, null, "Networklayer");
							_threadPoolManager.Run(ThreadPoolManager.ThreadType.HighPriority, httpAction);
						}
						else
						{
							Log.DebugTag("No more attempts...", null, "NetworkLayer");
							callback(httpWebResponse2, ex);
						}
					}
					else
					{
						callback(httpWebResponse2, ex);
					}
				}
				done();
			}
			catch (Exception ex2)
			{
				Log.ErrorTag("Unknown error: " + ex2.ToString(), null, "Networklayer");
				callback(null, null);
				done();
			}
		};
		if (!Cancelled)
		{
			if (_highPriority)
			{
				_threadPoolManager.Run(ThreadPoolManager.ThreadType.HighPriority, httpAction);
			}
			else
			{
				_threadPoolManager.Run(httpAction);
			}
		}
	}

	private bool ConnectionNotAvailable(WebException e)
	{
		return e.Status == WebExceptionStatus.ConnectFailure || e.Status == WebExceptionStatus.ConnectionClosed || e.Status == WebExceptionStatus.NameResolutionFailure || e.Status == WebExceptionStatus.SendFailure;
	}

	public Dictionary<string, string> Headers()
	{
		return _headers;
	}

	public Dictionary<string, string> Parameters()
	{
		return _params;
	}

	private void SetupHeaders(HttpWebRequest request)
	{
		foreach (KeyValuePair<string, string> header in _headers)
		{
			if (!HEADER_PROPERTIES_MAP.ContainsKey(header.Key))
			{
				request.Headers[header.Key] = header.Value;
				continue;
			}
			string text = HEADER_PROPERTIES_MAP[header.Key];
			if (text == null)
			{
				text = header.Key;
			}
			request.GetType().GetProperty(text).SetValue(request, _headers[header.Key], null);
		}
	}
}
