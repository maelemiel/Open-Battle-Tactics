using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public class RestClient
{
	private string _host;

	private ThreadPoolManager _threadPoolManager;

	private RestMiddleware _restMiddleware;

	private int _defaultRetries = 3;

	private MediaTypes _mediaTypes;

	private DeNetworkErrorHandler _errorHandler;

	private object _communicationError = "communicationError";

	private string _defaultContentType = "application/json";

	public MediaTypes MediaTypes
	{
		get
		{
			return _mediaTypes;
		}
	}

	public int DefaultRetries
	{
		get
		{
			return _defaultRetries;
		}
	}

	public RestClient(ThreadPoolManager threadPoolManager)
	{
		_threadPoolManager = threadPoolManager;
		_restMiddleware = new RestMiddleware();
		_mediaTypes = new MediaTypes();
		ServerCertificateValidationCallback((object o, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) => true);
	}

	public RestClient Host(string host)
	{
		_host = host;
		return this;
	}

	public RestRequest At(string path)
	{
		return new RestRequest(this).At(_host + path).As(_defaultContentType).ErrorHandler(_errorHandler);
	}

	public RestClient SetDefaultContentType(string contentType)
	{
		_defaultContentType = contentType;
		return this;
	}

	public RestClient SetErrorHandler(DeNetworkErrorHandler errorHandler)
	{
		_errorHandler = errorHandler;
		return this;
	}

	public RestClient BeforeRequest(Action<RestRequest> action)
	{
		_restMiddleware.BeforeRequest(action);
		return this;
	}

	public RestClient AfterResponse(Action<RestResponse> action)
	{
		_restMiddleware.AfterResponse(action);
		return this;
	}

	public RestClient AfterResourceOfType<T>(Action<TypedRestResponse<T>> action)
	{
		_restMiddleware.AfterResourceOfType(action);
		return this;
	}

	public RestClient UseLogMiddleware()
	{
		BeforeRequest(RestMiddleware.LogRequest);
		AfterResponse(RestMiddleware.LogResponse);
		return this;
	}

	public RestClient OnNetworkLost(Action action)
	{
		_threadPoolManager.LostNetworkConnectionCallback = action;
		return this;
	}

	public RestClient OnNetworkRecovered(Action action)
	{
		_threadPoolManager.RecoveredNetworkConnectionCallback = action;
		return this;
	}

	public ThreadPoolManager ThreadPoolManager()
	{
		return _threadPoolManager;
	}

	public RestMiddleware Middleware()
	{
		return _restMiddleware;
	}

	public object GetCommunicationErrorObject()
	{
		return _communicationError;
	}

	public void SetDefaultRetries(int retries)
	{
		_defaultRetries = retries;
	}

	public RestClient SetMediaType(string contentType, MediaType mediaType)
	{
		_mediaTypes.SetMediaType(contentType, mediaType);
		return this;
	}

	public void ServerCertificateValidationCallback(RemoteCertificateValidationCallback validationCallback)
	{
		ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, validationCallback);
	}
}
