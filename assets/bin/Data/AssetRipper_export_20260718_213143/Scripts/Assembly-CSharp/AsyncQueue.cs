using System;
using System.Collections.Generic;
using System.Threading;

public class AsyncQueue<AsyncQueueType, RequestType, ResponseType> : Singleton<AsyncQueueType> where AsyncQueueType : class where RequestType : AsyncQueue<AsyncQueueType, RequestType, ResponseType>.BaseRequest where ResponseType : AsyncQueue<AsyncQueueType, RequestType, ResponseType>.BaseResponse, new()
{
	public class BaseRequest
	{
		public bool cancelled;

		public bool queued;

		public bool prioritized;

		public Callback callback;

		public Action onCancel;

		public Func<bool> waitForCondition;
	}

	public class BaseResponse
	{
		public RequestType request;
	}

	public delegate void Callback(ResponseType response);

	private Queue<RequestType> requestQueue = new Queue<RequestType>();

	private Queue<RequestType> priorityRequestQueue = new Queue<RequestType>();

	private Queue<ResponseType> responseQueue = new Queue<ResponseType>();

	private Thread thread;

	private int outstandingCount;

	private bool forceKill;

	public virtual void Start()
	{
		thread = new Thread(ThreadFunc);
		thread.Priority = ThreadPriority.BelowNormal;
		thread.Name = GetType().Name + "Thr";
		Log.InfoTag("New Thread Created with name: {0} and id: {1}", null, "AsyncQueue", thread.Name, thread.ManagedThreadId);
		thread.Start();
	}

	private void OnDestroy()
	{
		forceKill = true;
		thread.Abort();
	}

	public void Reset()
	{
		CancelAll();
		requestQueue.Clear();
		priorityRequestQueue.Clear();
		responseQueue.Clear();
		forceKill = true;
	}

	private void Update()
	{
		if (outstandingCount <= 0)
		{
			return;
		}
		ResponseType[] array;
		lock (responseQueue)
		{
			int count = responseQueue.Count;
			if (count == 0)
			{
				return;
			}
			array = new ResponseType[count];
			responseQueue.CopyTo(array, 0);
			responseQueue.Clear();
		}
		foreach (ResponseType val in array)
		{
			if (val.request.callback != null && !val.request.cancelled)
			{
				val.request.callback(val);
			}
		}
		Interlocked.Add(ref outstandingCount, -array.Length);
	}

	public void Enqueue(RequestType request)
	{
		if (request.queued)
		{
			throw new Exception("This request has already been queued");
		}
		request.queued = true;
		lock (requestQueue)
		{
			if (request.prioritized)
			{
				priorityRequestQueue.Enqueue(request);
			}
			else
			{
				requestQueue.Enqueue(request);
			}
			Monitor.Pulse(requestQueue);
		}
	}

	public bool Contains(RequestType req)
	{
		lock (requestQueue)
		{
			if (req.prioritized)
			{
				return priorityRequestQueue.Contains(req);
			}
			return requestQueue.Contains(req);
		}
	}

	public void Cancel(RequestType request)
	{
		request.cancelled = true;
	}

	public void CancelAll()
	{
		lock (requestQueue)
		{
			foreach (RequestType item in requestQueue)
			{
				item.cancelled = true;
			}
		}
	}

	private void ThreadFunc()
	{
		while (true)
		{
			bool flag = false;
			RequestType val;
			lock (requestQueue)
			{
				while (requestQueue.Count == 0 && priorityRequestQueue.Count == 0)
				{
					Monitor.Wait(requestQueue);
				}
				val = ((priorityRequestQueue.Count <= 0) ? requestQueue.Dequeue() : priorityRequestQueue.Dequeue());
			}
			ResponseType val2 = new ResponseType
			{
				request = val
			};
			if (val.waitForCondition != null)
			{
				do
				{
					if (forceKill)
					{
						forceKill = false;
						val.cancelled = true;
						break;
					}
				}
				while (!val.cancelled && val.waitForCondition != null && !val.waitForCondition());
			}
			if (!val.cancelled)
			{
				try
				{
					ProcessRequest(val, val2);
				}
				catch (Exception ex)
				{
					Log.ErrorTag("Unhandled Exception ProcessRequest, no callback is going to be called!. Error: " + ex.ToString(), null, "AsyncQueue");
					flag = true;
				}
				if (!flag)
				{
					lock (responseQueue)
					{
						responseQueue.Enqueue(val2);
					}
					Interlocked.Increment(ref outstandingCount);
				}
			}
			else if (val.onCancel != null)
			{
				val.onCancel();
			}
		}
	}

	protected virtual void ProcessRequest(RequestType request, ResponseType response)
	{
	}
}
