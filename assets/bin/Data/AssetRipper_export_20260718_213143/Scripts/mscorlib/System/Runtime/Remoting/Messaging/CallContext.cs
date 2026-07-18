using System.Collections;
using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Messaging
{
	[Serializable]
	[ComVisible(true)]
	public sealed class CallContext
	{
		[ThreadStatic]
		private static Header[] Headers;

		[ThreadStatic]
		private static Hashtable datastore;

		public static object HostContext
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		private static Hashtable Datastore
		{
			get
			{
				Hashtable hashtable = datastore;
				if (hashtable == null)
				{
					return datastore = new Hashtable();
				}
				return hashtable;
			}
		}

		private CallContext()
		{
		}

		public static void FreeNamedDataSlot(string name)
		{
			Datastore.Remove(name);
		}

		public static object GetData(string name)
		{
			return Datastore[name];
		}

		public static void SetData(string name, object data)
		{
			Datastore[name] = data;
		}

		[MonoTODO]
		public static object LogicalGetData(string name)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static void LogicalSetData(string name, object data)
		{
			throw new NotImplementedException();
		}

		public static Header[] GetHeaders()
		{
			return Headers;
		}

		public static void SetHeaders(Header[] headers)
		{
			Headers = headers;
		}

		internal static LogicalCallContext CreateLogicalCallContext(bool createEmpty)
		{
			LogicalCallContext logicalCallContext = null;
			if (datastore != null)
			{
				foreach (DictionaryEntry item in datastore)
				{
					if (item.Value is ILogicalThreadAffinative)
					{
						if (logicalCallContext == null)
						{
							logicalCallContext = new LogicalCallContext();
						}
						logicalCallContext.SetData((string)item.Key, item.Value);
					}
				}
			}
			if (logicalCallContext == null && createEmpty)
			{
				return new LogicalCallContext();
			}
			return logicalCallContext;
		}

		internal static object SetCurrentCallContext(LogicalCallContext ctx)
		{
			object result = datastore;
			if (ctx != null && ctx.HasInfo)
			{
				datastore = (Hashtable)ctx.Datastore.Clone();
			}
			else
			{
				datastore = null;
			}
			return result;
		}

		internal static void UpdateCurrentCallContext(LogicalCallContext ctx)
		{
			Hashtable hashtable = ctx.Datastore;
			foreach (DictionaryEntry item in hashtable)
			{
				SetData((string)item.Key, item.Value);
			}
		}

		internal static void RestoreCallContext(object oldContext)
		{
			datastore = (Hashtable)oldContext;
		}
	}
}
