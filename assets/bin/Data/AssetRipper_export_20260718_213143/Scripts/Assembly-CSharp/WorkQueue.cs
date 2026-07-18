using System;

public class WorkQueue : AsyncQueue<WorkQueue, WorkQueue.Request, WorkQueue.Response>
{
	public class Request : BaseRequest
	{
		public Func<object> backgrundCallback;

		public Action backgrundNoReturnCallback;
	}

	public class Response : BaseResponse
	{
		public object retVal;

		public string error;
	}

	public static void Do(Func<object> backgroundCallback, Callback foregroundCallback)
	{
		Singleton<WorkQueue>.instance.Enqueue(new Request
		{
			backgrundCallback = backgroundCallback,
			callback = foregroundCallback
		});
	}

	public static void Do(Action backgroundNoReturnCallback)
	{
		Singleton<WorkQueue>.instance.Enqueue(new Request
		{
			backgrundNoReturnCallback = backgroundNoReturnCallback
		});
	}

	protected override void ProcessRequest(Request request, Response response)
	{
		if (request.backgrundNoReturnCallback != null)
		{
			try
			{
				request.backgrundNoReturnCallback();
				return;
			}
			catch (Exception ex)
			{
				Log.Error("Error executing non returning callback. Error: " + ex.ToString());
				return;
			}
		}
		try
		{
			response.retVal = request.backgrundCallback();
		}
		catch (Exception ex2)
		{
			response.error = ex2.ToString();
		}
	}
}
