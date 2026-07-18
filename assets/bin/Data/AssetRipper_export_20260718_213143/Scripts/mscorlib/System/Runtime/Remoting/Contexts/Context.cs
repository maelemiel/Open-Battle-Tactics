using System.Collections;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Lifetime;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Threading;

namespace System.Runtime.Remoting.Contexts
{
	[ComVisible(true)]
	public class Context
	{
		private int domain_id;

		private int context_id;

		private UIntPtr static_data;

		private static IMessageSink default_server_context_sink;

		private IMessageSink server_context_sink_chain;

		private IMessageSink client_context_sink_chain;

		private object[] datastore;

		private ArrayList context_properties;

		private bool frozen;

		private static int global_count;

		private static Hashtable namedSlots = new Hashtable();

		private static DynamicPropertyCollection global_dynamic_properties;

		private DynamicPropertyCollection context_dynamic_properties;

		private ContextCallbackObject callback_object;

		public static Context DefaultContext
		{
			get
			{
				return AppDomain.InternalGetDefaultContext();
			}
		}

		public virtual int ContextID
		{
			get
			{
				return context_id;
			}
		}

		public virtual IContextProperty[] ContextProperties
		{
			get
			{
				if (context_properties == null)
				{
					return new IContextProperty[0];
				}
				return (IContextProperty[])context_properties.ToArray(typeof(IContextProperty[]));
			}
		}

		internal bool IsDefaultContext
		{
			get
			{
				return context_id == 0;
			}
		}

		internal bool NeedsContextSink
		{
			get
			{
				return context_id != 0 || (global_dynamic_properties != null && global_dynamic_properties.HasProperties) || (context_dynamic_properties != null && context_dynamic_properties.HasProperties);
			}
		}

		internal static bool HasGlobalDynamicSinks
		{
			get
			{
				return global_dynamic_properties != null && global_dynamic_properties.HasProperties;
			}
		}

		internal bool HasDynamicSinks
		{
			get
			{
				return context_dynamic_properties != null && context_dynamic_properties.HasProperties;
			}
		}

		internal bool HasExitSinks
		{
			get
			{
				return !(GetClientContextSinkChain() is ClientContextTerminatorSink) || HasDynamicSinks || HasGlobalDynamicSinks;
			}
		}

		public Context()
		{
			domain_id = Thread.GetDomainID();
			context_id = 1 + global_count++;
		}

		~Context()
		{
		}

		public static bool RegisterDynamicProperty(IDynamicProperty prop, ContextBoundObject obj, Context ctx)
		{
			DynamicPropertyCollection dynamicPropertyCollection = GetDynamicPropertyCollection(obj, ctx);
			return dynamicPropertyCollection.RegisterDynamicProperty(prop);
		}

		public static bool UnregisterDynamicProperty(string name, ContextBoundObject obj, Context ctx)
		{
			DynamicPropertyCollection dynamicPropertyCollection = GetDynamicPropertyCollection(obj, ctx);
			return dynamicPropertyCollection.UnregisterDynamicProperty(name);
		}

		private static DynamicPropertyCollection GetDynamicPropertyCollection(ContextBoundObject obj, Context ctx)
		{
			if (ctx == null && obj != null)
			{
				if (RemotingServices.IsTransparentProxy(obj))
				{
					RealProxy realProxy = RemotingServices.GetRealProxy(obj);
					return realProxy.ObjectIdentity.ClientDynamicProperties;
				}
				return obj.ObjectIdentity.ServerDynamicProperties;
			}
			if (ctx != null && obj == null)
			{
				if (ctx.context_dynamic_properties == null)
				{
					ctx.context_dynamic_properties = new DynamicPropertyCollection();
				}
				return ctx.context_dynamic_properties;
			}
			if (ctx == null && obj == null)
			{
				if (global_dynamic_properties == null)
				{
					global_dynamic_properties = new DynamicPropertyCollection();
				}
				return global_dynamic_properties;
			}
			throw new ArgumentException("Either obj or ctx must be null");
		}

		internal static void NotifyGlobalDynamicSinks(bool start, IMessage req_msg, bool client_site, bool async)
		{
			if (global_dynamic_properties != null && global_dynamic_properties.HasProperties)
			{
				global_dynamic_properties.NotifyMessage(start, req_msg, client_site, async);
			}
		}

		internal void NotifyDynamicSinks(bool start, IMessage req_msg, bool client_site, bool async)
		{
			if (context_dynamic_properties != null && context_dynamic_properties.HasProperties)
			{
				context_dynamic_properties.NotifyMessage(start, req_msg, client_site, async);
			}
		}

		public virtual IContextProperty GetProperty(string name)
		{
			if (context_properties == null)
			{
				return null;
			}
			foreach (IContextProperty context_property in context_properties)
			{
				if (context_property.Name == name)
				{
					return context_property;
				}
			}
			return null;
		}

		public virtual void SetProperty(IContextProperty prop)
		{
			if (prop == null)
			{
				throw new ArgumentNullException("IContextProperty");
			}
			if (this == DefaultContext)
			{
				throw new InvalidOperationException("Can not add properties to default context");
			}
			if (frozen)
			{
				throw new InvalidOperationException("Context is Frozen");
			}
			if (context_properties == null)
			{
				context_properties = new ArrayList();
			}
			context_properties.Add(prop);
		}

		public virtual void Freeze()
		{
			if (context_properties == null)
			{
				return;
			}
			foreach (IContextProperty context_property in context_properties)
			{
				context_property.Freeze(this);
			}
		}

		public override string ToString()
		{
			return "ContextID: " + context_id;
		}

		internal IMessageSink GetServerContextSinkChain()
		{
			if (server_context_sink_chain == null)
			{
				if (default_server_context_sink == null)
				{
					default_server_context_sink = new ServerContextTerminatorSink();
				}
				server_context_sink_chain = default_server_context_sink;
				if (context_properties != null)
				{
					for (int num = context_properties.Count - 1; num >= 0; num--)
					{
						IContributeServerContextSink contributeServerContextSink = context_properties[num] as IContributeServerContextSink;
						if (contributeServerContextSink != null)
						{
							server_context_sink_chain = contributeServerContextSink.GetServerContextSink(server_context_sink_chain);
						}
					}
				}
			}
			return server_context_sink_chain;
		}

		internal IMessageSink GetClientContextSinkChain()
		{
			if (client_context_sink_chain == null)
			{
				client_context_sink_chain = new ClientContextTerminatorSink(this);
				if (context_properties != null)
				{
					foreach (IContextProperty context_property in context_properties)
					{
						IContributeClientContextSink contributeClientContextSink = context_property as IContributeClientContextSink;
						if (contributeClientContextSink != null)
						{
							client_context_sink_chain = contributeClientContextSink.GetClientContextSink(client_context_sink_chain);
						}
					}
				}
			}
			return client_context_sink_chain;
		}

		internal IMessageSink CreateServerObjectSinkChain(MarshalByRefObject obj, bool forceInternalExecute)
		{
			IMessageSink nextSink = new StackBuilderSink(obj, forceInternalExecute);
			nextSink = new ServerObjectTerminatorSink(nextSink);
			nextSink = new LeaseSink(nextSink);
			if (context_properties != null)
			{
				for (int num = context_properties.Count - 1; num >= 0; num--)
				{
					IContextProperty contextProperty = (IContextProperty)context_properties[num];
					IContributeObjectSink contributeObjectSink = contextProperty as IContributeObjectSink;
					if (contributeObjectSink != null)
					{
						nextSink = contributeObjectSink.GetObjectSink(obj, nextSink);
					}
				}
			}
			return nextSink;
		}

		internal IMessageSink CreateEnvoySink(MarshalByRefObject serverObject)
		{
			IMessageSink messageSink = EnvoyTerminatorSink.Instance;
			if (context_properties != null)
			{
				foreach (IContextProperty context_property in context_properties)
				{
					IContributeEnvoySink contributeEnvoySink = context_property as IContributeEnvoySink;
					if (contributeEnvoySink != null)
					{
						messageSink = contributeEnvoySink.GetEnvoySink(serverObject, messageSink);
					}
				}
			}
			return messageSink;
		}

		internal static Context SwitchToContext(Context newContext)
		{
			return AppDomain.InternalSetContext(newContext);
		}

		internal static Context CreateNewContext(IConstructionCallMessage msg)
		{
			Context context = new Context();
			foreach (IContextProperty contextProperty3 in msg.ContextProperties)
			{
				if (context.GetProperty(contextProperty3.Name) == null)
				{
					context.SetProperty(contextProperty3);
				}
			}
			context.Freeze();
			foreach (IContextProperty contextProperty4 in msg.ContextProperties)
			{
				if (!contextProperty4.IsNewContextOK(context))
				{
					throw new RemotingException("A context property did not approve the candidate context for activating the object");
				}
			}
			return context;
		}

		public void DoCallBack(CrossContextDelegate deleg)
		{
			lock (this)
			{
				if (callback_object == null)
				{
					Context newContext = SwitchToContext(this);
					callback_object = new ContextCallbackObject();
					SwitchToContext(newContext);
				}
			}
			callback_object.DoCallBack(deleg);
		}

		public static LocalDataStoreSlot AllocateDataSlot()
		{
			return new LocalDataStoreSlot(false);
		}

		public static LocalDataStoreSlot AllocateNamedDataSlot(string name)
		{
			lock (namedSlots.SyncRoot)
			{
				LocalDataStoreSlot localDataStoreSlot = AllocateDataSlot();
				namedSlots.Add(name, localDataStoreSlot);
				return localDataStoreSlot;
			}
		}

		public static void FreeNamedDataSlot(string name)
		{
			lock (namedSlots.SyncRoot)
			{
				namedSlots.Remove(name);
			}
		}

		public static object GetData(LocalDataStoreSlot slot)
		{
			Context currentContext = Thread.CurrentContext;
			lock (currentContext)
			{
				if (currentContext.datastore != null && slot.slot < currentContext.datastore.Length)
				{
					return currentContext.datastore[slot.slot];
				}
				return null;
			}
		}

		public static LocalDataStoreSlot GetNamedDataSlot(string name)
		{
			lock (namedSlots.SyncRoot)
			{
				LocalDataStoreSlot localDataStoreSlot = namedSlots[name] as LocalDataStoreSlot;
				if (localDataStoreSlot == null)
				{
					return AllocateNamedDataSlot(name);
				}
				return localDataStoreSlot;
			}
		}

		public static void SetData(LocalDataStoreSlot slot, object data)
		{
			Context currentContext = Thread.CurrentContext;
			lock (currentContext)
			{
				if (currentContext.datastore == null)
				{
					currentContext.datastore = new object[slot.slot + 2];
				}
				else if (slot.slot >= currentContext.datastore.Length)
				{
					object[] array = new object[slot.slot + 2];
					currentContext.datastore.CopyTo(array, 0);
					currentContext.datastore = array;
				}
				currentContext.datastore[slot.slot] = data;
			}
		}
	}
}
