using System.Collections;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Services
{
	[ComVisible(true)]
	public class TrackingServices
	{
		private static ArrayList _handlers = new ArrayList();

		public static ITrackingHandler[] RegisteredHandlers
		{
			get
			{
				lock (_handlers.SyncRoot)
				{
					if (_handlers.Count == 0)
					{
						return new ITrackingHandler[0];
					}
					return (ITrackingHandler[])_handlers.ToArray(typeof(ITrackingHandler));
				}
			}
		}

		public static void RegisterTrackingHandler(ITrackingHandler handler)
		{
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			lock (_handlers.SyncRoot)
			{
				if (_handlers.IndexOf(handler) != -1)
				{
					throw new RemotingException("handler already registered");
				}
				_handlers.Add(handler);
			}
		}

		public static void UnregisterTrackingHandler(ITrackingHandler handler)
		{
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			lock (_handlers.SyncRoot)
			{
				int num = _handlers.IndexOf(handler);
				if (num == -1)
				{
					throw new RemotingException("handler is not registered");
				}
				_handlers.RemoveAt(num);
			}
		}

		internal static void NotifyMarshaledObject(object obj, ObjRef or)
		{
			ITrackingHandler[] array;
			lock (_handlers.SyncRoot)
			{
				if (_handlers.Count == 0)
				{
					return;
				}
				array = (ITrackingHandler[])_handlers.ToArray(typeof(ITrackingHandler));
			}
			for (int i = 0; i < array.Length; i++)
			{
				array[i].MarshaledObject(obj, or);
			}
		}

		internal static void NotifyUnmarshaledObject(object obj, ObjRef or)
		{
			ITrackingHandler[] array;
			lock (_handlers.SyncRoot)
			{
				if (_handlers.Count == 0)
				{
					return;
				}
				array = (ITrackingHandler[])_handlers.ToArray(typeof(ITrackingHandler));
			}
			for (int i = 0; i < array.Length; i++)
			{
				array[i].UnmarshaledObject(obj, or);
			}
		}

		internal static void NotifyDisconnectedObject(object obj)
		{
			ITrackingHandler[] array;
			lock (_handlers.SyncRoot)
			{
				if (_handlers.Count == 0)
				{
					return;
				}
				array = (ITrackingHandler[])_handlers.ToArray(typeof(ITrackingHandler));
			}
			for (int i = 0; i < array.Length; i++)
			{
				array[i].DisconnectedObject(obj);
			}
		}
	}
}
