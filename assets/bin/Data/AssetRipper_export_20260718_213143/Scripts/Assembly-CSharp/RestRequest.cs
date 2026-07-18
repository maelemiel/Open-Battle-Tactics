using System;
using System.Collections.Generic;
using System.Net;

public class RestRequest
{
	private class HTTPMethods
	{
		public const string HEAD = "HEAD";

		public const string GET = "GET";

		public const string POST = "POST";

		public const string PUT = "PUT";

		public const string DELETE = "DELETE";
	}

	public delegate void RestCallback(RestResponse response);

	private HTTPHandler _httpHandler;

	private string _contentType = "application/json";

	private string _path;

	private string _method;

	private object _body;

	private string _rawBody;

	private object _responseType;

	private Action<RestResponse> _callback;

	private RestClient _client;

	private DeNetworkErrorHandler _errorHandler;

	private Dictionary<string, object> _config = new Dictionary<string, object>();

	private int _attempt;

	public string ContentType
	{
		get
		{
			return _contentType;
		}
	}

	public string Body
	{
		get
		{
			return _rawBody;
		}
	}

	public Dictionary<string, string> Headers
	{
		get
		{
			return _httpHandler.Headers();
		}
	}

	public string Path
	{
		get
		{
			return _path;
		}
	}

	public RestClient RestClient
	{
		get
		{
			return _client;
		}
	}

	public string Method
	{
		get
		{
			return _method;
		}
	}

	public int RetryAttempts
	{
		get
		{
			return _attempt;
		}
	}

	public RestRequest(RestClient restClient)
	{
		_httpHandler = new DefaultHTTPHandler(restClient.ThreadPoolManager());
		_client = restClient;
		_httpHandler.Retries(_client.DefaultRetries);
	}

	public RestRequest(RestClient restClient, HTTPHandler httpHandler)
	{
		_httpHandler = httpHandler;
		_client = restClient;
		_httpHandler.Retries(_client.DefaultRetries);
	}

	public void IncreaseRetryAttempts()
	{
		_attempt++;
	}

	public RestRequest body(object body)
	{
		_body = body;
		return this;
	}

	public RestRequest At(string path)
	{
		_path = path;
		return this;
	}

	public RestRequest As(string contentType)
	{
		_contentType = contentType;
		return this;
	}

	public RestRequest ErrorHandler(DeNetworkErrorHandler errorHandler)
	{
		_errorHandler = errorHandler;
		return this;
	}

	public RestRequest Header(string key, string value)
	{
		_httpHandler.Header(key, value);
		return this;
	}

	public RestRequest Param(string key, string value)
	{
		_httpHandler.Param(key, value);
		return this;
	}

	public RestRequest Config(string key, object value)
	{
		_config[key] = value;
		return this;
	}

	public object Config(string key)
	{
		if (_config.ContainsKey(key))
		{
			return _config[key];
		}
		return null;
	}

	public void Call()
	{
		createAndRunRequest();
	}

	public void Call(Action<RestResponse> callback)
	{
		_callback = callback;
		createAndRunRequest();
	}

	public void Response(Action<RestResponse> callback)
	{
		_callback = callback;
		createAndRunRequest();
	}

	public void Response<T>(Action<TypedRestResponse<T>> callback)
	{
		createAndRunRequest(callback);
	}

	public void Resource<T>(Action<T> callback)
	{
		_callback = delegate(RestResponse response)
		{
			T obj = response.Resource<T>();
			callback(obj);
		};
		createAndRunRequest();
	}

	public RestRequest Get()
	{
		_method = "GET";
		return this;
	}

	public RestRequest Post(object o)
	{
		_body = o;
		_method = "POST";
		return this;
	}

	public RestRequest Post()
	{
		_method = "POST";
		return this;
	}

	public RestRequest Put(object o)
	{
		_body = o;
		_method = "PUT";
		return this;
	}

	public RestRequest Put()
	{
		_method = "PUT";
		return this;
	}

	public RestRequest Delete(object o)
	{
		_body = o;
		_method = "DELETE";
		return this;
	}

	public RestRequest Delete()
	{
		_method = "DELETE";
		return this;
	}

	public RestRequest Head()
	{
		_method = "HEAD";
		return this;
	}

	public RestRequest Json()
	{
		_contentType = "application/json";
		return this;
	}

	public RestRequest Form()
	{
		_contentType = "application/x-www-form-urlencoded";
		return this;
	}

	public RestRequest TimeoutSeconds(int timeout)
	{
		_httpHandler.Timeout(timeout * 1000);
		return this;
	}

	public RestRequest Retries(int retries)
	{
		_httpHandler.Retries(retries);
		return this;
	}

	public RestRequest HighPriority()
	{
		_httpHandler.HighPriority(true);
		return this;
	}

	public void Cancel()
	{
		_httpHandler.Cancel();
	}

	private void createAndRunRequest()
	{
		basicRequestSetup();
		_httpHandler.Exec(_method, delegate(object httpWebResponse, object exception)
		{
			RestResponse restResponse = new RestResponse((HttpWebResponse)httpWebResponse, this, (WebException)exception);
			if (_errorHandler == null || !_errorHandler.CheckError(restResponse))
			{
				_client.ThreadPoolManager().RunOnMainThread(delegate
				{
					_callback(restResponse);
				});
			}
		});
	}

	private void createAndRunRequest<T>(Action<TypedRestResponse<T>> callback)
	{
		basicRequestSetup();
		_httpHandler.Exec(_method, delegate(object httpWebResponse, object exception)
		{
			TypedRestResponse<T> restResponse = new TypedRestResponse<T>((HttpWebResponse)httpWebResponse, this, (WebException)exception, callback);
			if (_errorHandler == null || !_errorHandler.CheckError(restResponse))
			{
				_client.ThreadPoolManager().RunOnMainThread(delegate
				{
					callback(restResponse);
				});
			}
		});
	}

	private void basicRequestSetup()
	{
		parseBody();
		_client.Middleware().ExecuteBeforeRequest(this);
		_httpHandler.Path(_path);
	}

	private void parseBody()
	{
		if (_body != null)
		{
			MediaType mediaType = RestClient.MediaTypes.valueOf(_contentType);
			if (mediaType == null)
			{
				throw new SystemException("No mediatype support for content type " + _contentType);
			}
			_rawBody = (string)mediaType.Marshall(_body);
			_httpHandler.Header("Content-Type", _contentType);
			_httpHandler.Body(_rawBody);
		}
	}

	public static void LogRequest(RestRequest request)
	{
		Log.DebugTag("New request {0} at {1}", null, "Networklayer_Request", request._method, request._path);
		List<string> list = new List<string>(request.Headers.Keys);
		for (int i = 0; i < list.Count; i++)
		{
			Log.DebugTag(list[i] + ": " + request.Headers[list[i]], null, "Networklayer_Request");
		}
		if (request._rawBody != null)
		{
			Log.DebugTag("Body: " + request._rawBody, null, "Networklayer_Request");
		}
	}
}
