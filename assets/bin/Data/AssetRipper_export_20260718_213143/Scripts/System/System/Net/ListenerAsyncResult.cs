using System.Threading;

namespace System.Net
{
	internal class ListenerAsyncResult : IAsyncResult
	{
		private ManualResetEvent handle;

		private bool synch;

		private bool completed;

		private AsyncCallback cb;

		private object state;

		private Exception exception;

		private HttpListenerContext context;

		private object locker = new object();

		private System.Net.ListenerAsyncResult forward;

		public object AsyncState
		{
			get
			{
				if (forward != null)
				{
					return forward.AsyncState;
				}
				return state;
			}
		}

		public WaitHandle AsyncWaitHandle
		{
			get
			{
				if (forward != null)
				{
					return forward.AsyncWaitHandle;
				}
				lock (locker)
				{
					if (handle == null)
					{
						handle = new ManualResetEvent(completed);
					}
				}
				return handle;
			}
		}

		public bool CompletedSynchronously
		{
			get
			{
				if (forward != null)
				{
					return forward.CompletedSynchronously;
				}
				return synch;
			}
		}

		public bool IsCompleted
		{
			get
			{
				if (forward != null)
				{
					return forward.IsCompleted;
				}
				lock (locker)
				{
					return completed;
				}
			}
		}

		public ListenerAsyncResult(AsyncCallback cb, object state)
		{
			this.cb = cb;
			this.state = state;
		}

		internal void Complete(string error)
		{
			if (forward != null)
			{
				forward.Complete(error);
				return;
			}
			exception = new HttpListenerException(0, error);
			lock (locker)
			{
				completed = true;
				if (handle != null)
				{
					handle.Set();
				}
				if (cb != null)
				{
					ThreadPool.QueueUserWorkItem(InvokeCallback, this);
				}
			}
		}

		private static void InvokeCallback(object o)
		{
			System.Net.ListenerAsyncResult listenerAsyncResult = (System.Net.ListenerAsyncResult)o;
			if (listenerAsyncResult.forward != null)
			{
				listenerAsyncResult.forward.cb(listenerAsyncResult);
			}
			else
			{
				listenerAsyncResult.cb(listenerAsyncResult);
			}
		}

		internal void Complete(HttpListenerContext context)
		{
			Complete(context, false);
		}

		internal void Complete(HttpListenerContext context, bool synch)
		{
			if (forward != null)
			{
				forward.Complete(context, synch);
				return;
			}
			this.synch = synch;
			this.context = context;
			lock (locker)
			{
				AuthenticationSchemes authenticationSchemes = context.Listener.SelectAuthenticationScheme(context);
				if ((authenticationSchemes == AuthenticationSchemes.Basic || context.Listener.AuthenticationSchemes == AuthenticationSchemes.Negotiate) && context.Request.Headers["Authorization"] == null)
				{
					context.Response.StatusCode = 401;
					context.Response.Headers["WWW-Authenticate"] = string.Concat(authenticationSchemes, " realm=\"", context.Listener.Realm, "\"");
					context.Response.OutputStream.Close();
					IAsyncResult asyncResult = context.Listener.BeginGetContext(cb, state);
					forward = (System.Net.ListenerAsyncResult)asyncResult;
					lock (forward.locker)
					{
						if (handle != null)
						{
							forward.handle = handle;
						}
					}
					System.Net.ListenerAsyncResult listenerAsyncResult = forward;
					int num = 0;
					while (listenerAsyncResult.forward != null)
					{
						if (num > 20)
						{
							Complete("Too many authentication errors");
						}
						listenerAsyncResult = listenerAsyncResult.forward;
						num++;
					}
				}
				else
				{
					completed = true;
					if (handle != null)
					{
						handle.Set();
					}
					if (cb != null)
					{
						ThreadPool.QueueUserWorkItem(InvokeCallback, this);
					}
				}
			}
		}

		internal HttpListenerContext GetContext()
		{
			if (forward != null)
			{
				return forward.GetContext();
			}
			if (exception != null)
			{
				throw exception;
			}
			return context;
		}
	}
}
