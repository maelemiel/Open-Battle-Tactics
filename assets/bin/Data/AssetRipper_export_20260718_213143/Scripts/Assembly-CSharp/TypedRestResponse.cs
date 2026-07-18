using System;
using System.Net;

public class TypedRestResponse<T> : RestResponse
{
	private Action<TypedRestResponse<T>> _callback;

	public TypedRestResponse(HttpWebResponse response, RestRequest request, WebException exception, Action<TypedRestResponse<T>> callback)
		: base(response, request, exception)
	{
		Resource<T>();
		_callback = callback;
		try
		{
			base.OriginalRequest.RestClient.Middleware().ExecuteAfterParse(this);
		}
		catch (Exception value)
		{
			Console.WriteLine(value);
		}
	}

	public T Resource()
	{
		return (T)_cachedResource;
	}

	public void Retry()
	{
		base.OriginalRequest.IncreaseRetryAttempts();
		base.OriginalRequest.Response(_callback);
	}
}
