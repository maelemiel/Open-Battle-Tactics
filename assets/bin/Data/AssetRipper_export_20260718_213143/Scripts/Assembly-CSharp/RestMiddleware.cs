using System;
using System.Collections.Generic;

public class RestMiddleware
{
	private List<Action<RestRequest>> _beforeRequestActions = new List<Action<RestRequest>>();

	private List<Action<RestResponse>> _afterResponseActions = new List<Action<RestResponse>>();

	private List<Action<RestResponse>> _afterParseActions = new List<Action<RestResponse>>();

	public void BeforeRequest(Action<RestRequest> action)
	{
		_beforeRequestActions.Add(action);
	}

	public void AfterResponse(Action<RestResponse> action)
	{
		_afterResponseActions.Add(action);
	}

	public void AfterResourceOfType<T>(Action<TypedRestResponse<T>> action)
	{
		_afterParseActions.Add(delegate(RestResponse response)
		{
			object obj = response.CachedResource();
			if (obj != null)
			{
				TypedRestResponse<T> obj2;
				try
				{
					obj2 = (TypedRestResponse<T>)response;
				}
				catch (InvalidCastException)
				{
					return;
				}
				if (obj.GetType() == typeof(T))
				{
					action(obj2);
				}
				else if (obj.GetType().BaseType == typeof(T))
				{
					action(obj2);
				}
			}
		});
	}

	public void ExecuteBeforeRequest(RestRequest request)
	{
		ExecuteMiddleware(_beforeRequestActions, request);
	}

	public void ExecuteAfterResponse(RestResponse response)
	{
		ExecuteMiddleware(_afterResponseActions, response);
	}

	public void ExecuteAfterParse(RestResponse response)
	{
		ExecuteMiddleware(_afterParseActions, response);
	}

	private void ExecuteMiddleware<T>(List<Action<T>> list, T obj)
	{
		for (int i = 0; i < list.Count; i++)
		{
			list[i](obj);
		}
	}

	public static void LogResponse(RestResponse response)
	{
		RestResponse.LogResponse(response);
	}

	public static void LogRequest(RestRequest request)
	{
		RestRequest.LogRequest(request);
	}
}
