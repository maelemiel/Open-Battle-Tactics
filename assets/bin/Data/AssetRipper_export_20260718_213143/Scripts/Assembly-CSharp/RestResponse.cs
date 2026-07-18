using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using ICSharpCode.SharpZipLib.GZip;

public class RestResponse
{
	private const string GZIP_CONTENT_ENCODING = "gzip";

	private Dictionary<string, object> _headers = new Dictionary<string, object>();

	private RestRequest _request;

	private string _responseUrl;

	private Stream _stream;

	private string _characterSet;

	private string _contentType;

	private object _error;

	private object _body;

	private int _statusCode;

	private string _statusDescription;

	private MediaType _mediaType;

	private WebException _exception;

	protected object _cachedResource;

	public object Error
	{
		get
		{
			return _error;
		}
		set
		{
			_error = value;
		}
	}

	public RestRequest OriginalRequest
	{
		get
		{
			return _request;
		}
	}

	public ThreadPoolManager ThreadPoolManager
	{
		get
		{
			return _request.RestClient.ThreadPoolManager();
		}
	}

	public object Body
	{
		get
		{
			return _body;
		}
	}

	public Dictionary<string, object> Headers
	{
		get
		{
			return _headers;
		}
	}

	public string CharacterSet
	{
		get
		{
			return _characterSet;
		}
	}

	public Stream Stream
	{
		get
		{
			return _stream;
		}
		set
		{
			_stream = value;
		}
	}

	public string StatusDescription
	{
		get
		{
			return _statusDescription;
		}
	}

	public int StatusCode
	{
		get
		{
			return _statusCode;
		}
		set
		{
			_statusCode = value;
		}
	}

	public WebException Exception
	{
		get
		{
			return _exception;
		}
	}

	public RestResponse(HttpWebResponse response, RestRequest request, WebException exception)
	{
		_request = request;
		_exception = exception;
		if (response == null)
		{
			Error = request.RestClient.GetCommunicationErrorObject();
			_statusCode = 0;
		}
		else
		{
			ValueOfWebResponse(response);
		}
		if (_mediaType != null)
		{
			_body = _mediaType.ParseResponseStream(this);
		}
		ExecuteResponseMiddleware();
	}

	public RestResponse(RestRequest request)
	{
		_request = request;
	}

	private void ValueOfWebResponse(HttpWebResponse httpResponse)
	{
		_characterSet = httpResponse.CharacterSet;
		_responseUrl = httpResponse.ResponseUri.ToString();
		_statusCode = (int)httpResponse.StatusCode;
		_statusDescription = httpResponse.StatusDescription;
		_stream = httpResponse.GetResponseStream();
		if (httpResponse.ContentType != null)
		{
			_contentType = OriginalRequest.RestClient.MediaTypes.ContentTypeFromHTTPFormat(httpResponse.ContentType);
			_mediaType = OriginalRequest.RestClient.MediaTypes.valueOf(_contentType);
			if (httpResponse.ContentEncoding != null && "gzip" == httpResponse.ContentEncoding.ToLower())
			{
				_stream = new GZipInputStream(_stream);
			}
		}
		else
		{
			Console.WriteLine("HttpWebResponse Content type is null");
		}
		_headers["Content-Type"] = httpResponse.ContentType;
		_headers["Content-Length"] = httpResponse.ContentLength;
		_headers["Content-Encoding"] = httpResponse.ContentEncoding;
		for (int i = 0; i < httpResponse.Headers.Count; i++)
		{
			string key = httpResponse.Headers.Keys[i];
			string value = httpResponse.Headers[i];
			_headers[key] = value;
		}
	}

	public object CachedResource()
	{
		return _cachedResource;
	}

	public T Resource<T>()
	{
		if (_mediaType == null)
		{
			Log.ErrorTag("No mediatype support for content type " + _contentType, null, "Networklayer");
			return default(T);
		}
		if (_cachedResource == null)
		{
			_cachedResource = _mediaType.Unmarshall<T>(this);
		}
		return (T)_cachedResource;
	}

	public void ExecuteResponseMiddleware()
	{
		_request.RestClient.Middleware().ExecuteAfterResponse(this);
	}

	public static void LogResponse(RestResponse response)
	{
		Log.DebugTag("Response from {0} \n {1}", null, "Networklayer_Response", response._responseUrl, Environment.StackTrace);
		List<string> list = new List<string>(response._headers.Keys);
		for (int i = 0; i < list.Count; i++)
		{
			Log.DebugTag(list[i] + ": " + response.Headers[list[i]], null, "Networklayer_Response");
		}
		if (response._body != null && response._body is string)
		{
			Log.DebugTag("Body: " + response._body, null, "Networklayer_Response");
		}
	}
}
