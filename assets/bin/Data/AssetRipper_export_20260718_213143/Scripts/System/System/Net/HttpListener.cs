using System.Collections;
using System.Threading;

namespace System.Net
{
	public sealed class HttpListener : IDisposable
	{
		private AuthenticationSchemes auth_schemes;

		private HttpListenerPrefixCollection prefixes;

		private AuthenticationSchemeSelector auth_selector;

		private string realm;

		private bool ignore_write_exceptions;

		private bool unsafe_ntlm_auth;

		private bool listening;

		private bool disposed;

		private Hashtable registry;

		private ArrayList ctx_queue;

		private ArrayList wait_queue;

		public AuthenticationSchemes AuthenticationSchemes
		{
			get
			{
				return auth_schemes;
			}
			set
			{
				CheckDisposed();
				auth_schemes = value;
			}
		}

		public AuthenticationSchemeSelector AuthenticationSchemeSelectorDelegate
		{
			get
			{
				return auth_selector;
			}
			set
			{
				CheckDisposed();
				auth_selector = value;
			}
		}

		public bool IgnoreWriteExceptions
		{
			get
			{
				return ignore_write_exceptions;
			}
			set
			{
				CheckDisposed();
				ignore_write_exceptions = value;
			}
		}

		public bool IsListening
		{
			get
			{
				return listening;
			}
		}

		public static bool IsSupported
		{
			get
			{
				return true;
			}
		}

		public HttpListenerPrefixCollection Prefixes
		{
			get
			{
				CheckDisposed();
				return prefixes;
			}
		}

		public string Realm
		{
			get
			{
				return realm;
			}
			set
			{
				CheckDisposed();
				realm = value;
			}
		}

		[System.MonoTODO("Support for NTLM needs some loving.")]
		public bool UnsafeConnectionNtlmAuthentication
		{
			get
			{
				return unsafe_ntlm_auth;
			}
			set
			{
				CheckDisposed();
				unsafe_ntlm_auth = value;
			}
		}

		public HttpListener()
		{
			prefixes = new HttpListenerPrefixCollection(this);
			registry = new Hashtable();
			ctx_queue = new ArrayList();
			wait_queue = new ArrayList();
			auth_schemes = AuthenticationSchemes.Anonymous;
		}

		void IDisposable.Dispose()
		{
			if (!disposed)
			{
				Close(true);
				disposed = true;
			}
		}

		public void Abort()
		{
			if (!disposed && listening)
			{
				Close(true);
			}
		}

		public void Close()
		{
			if (!disposed)
			{
				if (!listening)
				{
					disposed = true;
					return;
				}
				Close(false);
				disposed = true;
			}
		}

		private void Close(bool force)
		{
			CheckDisposed();
			System.Net.EndPointManager.RemoveListener(this);
			Cleanup(force);
		}

		private void Cleanup(bool close_existing)
		{
			lock (registry)
			{
				if (close_existing)
				{
					foreach (HttpListenerContext key in registry.Keys)
					{
						key.Connection.Close();
					}
					registry.Clear();
				}
				lock (ctx_queue)
				{
					foreach (HttpListenerContext item in ctx_queue)
					{
						item.Connection.Close();
					}
					ctx_queue.Clear();
				}
				lock (wait_queue)
				{
					foreach (System.Net.ListenerAsyncResult item2 in wait_queue)
					{
						item2.Complete("Listener was closed.");
					}
					wait_queue.Clear();
				}
			}
		}

		public IAsyncResult BeginGetContext(AsyncCallback callback, object state)
		{
			CheckDisposed();
			if (!listening)
			{
				throw new InvalidOperationException("Please, call Start before using this method.");
			}
			System.Net.ListenerAsyncResult listenerAsyncResult = new System.Net.ListenerAsyncResult(callback, state);
			lock (wait_queue)
			{
				lock (ctx_queue)
				{
					HttpListenerContext contextFromQueue = GetContextFromQueue();
					if (contextFromQueue != null)
					{
						listenerAsyncResult.Complete(contextFromQueue, true);
						return listenerAsyncResult;
					}
				}
				wait_queue.Add(listenerAsyncResult);
				return listenerAsyncResult;
			}
		}

		public HttpListenerContext EndGetContext(IAsyncResult asyncResult)
		{
			CheckDisposed();
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			System.Net.ListenerAsyncResult listenerAsyncResult = asyncResult as System.Net.ListenerAsyncResult;
			if (listenerAsyncResult == null)
			{
				throw new ArgumentException("Wrong IAsyncResult.", "asyncResult");
			}
			if (!listenerAsyncResult.IsCompleted)
			{
				listenerAsyncResult.AsyncWaitHandle.WaitOne();
			}
			lock (wait_queue)
			{
				int num = wait_queue.IndexOf(listenerAsyncResult);
				if (num >= 0)
				{
					wait_queue.RemoveAt(num);
				}
			}
			HttpListenerContext context = listenerAsyncResult.GetContext();
			if (auth_schemes != AuthenticationSchemes.Anonymous)
			{
				context.ParseAuthentication(auth_schemes);
			}
			return context;
		}

		internal AuthenticationSchemes SelectAuthenticationScheme(HttpListenerContext context)
		{
			if (AuthenticationSchemeSelectorDelegate != null)
			{
				return AuthenticationSchemeSelectorDelegate(context.Request);
			}
			return auth_schemes;
		}

		public HttpListenerContext GetContext()
		{
			if (prefixes.Count == 0)
			{
				throw new InvalidOperationException("Please, call AddPrefix before using this method.");
			}
			IAsyncResult asyncResult = BeginGetContext(null, null);
			return EndGetContext(asyncResult);
		}

		public void Start()
		{
			CheckDisposed();
			if (!listening)
			{
				System.Net.EndPointManager.AddListener(this);
				listening = true;
			}
		}

		public void Stop()
		{
			CheckDisposed();
			listening = false;
			Close(false);
		}

		internal void CheckDisposed()
		{
			if (disposed)
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
		}

		private HttpListenerContext GetContextFromQueue()
		{
			if (ctx_queue.Count == 0)
			{
				return null;
			}
			HttpListenerContext result = (HttpListenerContext)ctx_queue[0];
			ctx_queue.RemoveAt(0);
			return result;
		}

		internal void RegisterContext(HttpListenerContext context)
		{
			try
			{
				Monitor.Enter(registry);
				registry[context] = context;
				Monitor.Enter(wait_queue);
				Monitor.Enter(ctx_queue);
				if (wait_queue.Count == 0)
				{
					ctx_queue.Add(context);
					return;
				}
				System.Net.ListenerAsyncResult listenerAsyncResult = (System.Net.ListenerAsyncResult)wait_queue[0];
				wait_queue.RemoveAt(0);
				listenerAsyncResult.Complete(context);
			}
			finally
			{
				Monitor.Exit(ctx_queue);
				Monitor.Exit(wait_queue);
				Monitor.Exit(registry);
			}
		}

		internal void UnregisterContext(HttpListenerContext context)
		{
			try
			{
				Monitor.Enter(registry);
				Monitor.Enter(ctx_queue);
				int num = ctx_queue.IndexOf(context);
				if (num >= 0)
				{
					ctx_queue.RemoveAt(num);
				}
				registry.Remove(context);
			}
			finally
			{
				Monitor.Exit(ctx_queue);
				Monitor.Exit(registry);
			}
		}
	}
}
