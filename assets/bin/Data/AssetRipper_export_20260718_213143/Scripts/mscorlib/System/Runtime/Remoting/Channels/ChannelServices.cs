using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;

namespace System.Runtime.Remoting.Channels
{
	[ComVisible(true)]
	public sealed class ChannelServices
	{
		private static ArrayList registeredChannels = new ArrayList();

		private static ArrayList delayedClientChannels = new ArrayList();

		private static CrossContextChannel _crossContextSink = new CrossContextChannel();

		internal static string CrossContextUrl = "__CrossContext";

		private static IList oldStartModeTypes = new string[2] { "Novell.Zenworks.Zmd.Public.UnixServerChannel", "Novell.Zenworks.Zmd.Public.UnixChannel" };

		internal static CrossContextChannel CrossContextChannel
		{
			get
			{
				return _crossContextSink;
			}
		}

		public static IChannel[] RegisteredChannels
		{
			get
			{
				lock (registeredChannels.SyncRoot)
				{
					ArrayList arrayList = new ArrayList();
					for (int i = 0; i < registeredChannels.Count; i++)
					{
						IChannel channel = (IChannel)registeredChannels[i];
						if (!(channel is CrossAppDomainChannel))
						{
							arrayList.Add(channel);
						}
					}
					return (IChannel[])arrayList.ToArray(typeof(IChannel));
				}
			}
		}

		private ChannelServices()
		{
		}

		internal static IMessageSink CreateClientChannelSinkChain(string url, object remoteChannelData, out string objectUri)
		{
			object[] channelDataArray = (object[])remoteChannelData;
			lock (registeredChannels.SyncRoot)
			{
				foreach (IChannel registeredChannel in registeredChannels)
				{
					IChannelSender channelSender = registeredChannel as IChannelSender;
					if (channelSender != null)
					{
						IMessageSink messageSink = CreateClientChannelSinkChain(channelSender, url, channelDataArray, out objectUri);
						if (messageSink != null)
						{
							return messageSink;
						}
					}
				}
				RemotingConfiguration.LoadDefaultDelayedChannels();
				foreach (IChannelSender delayedClientChannel in delayedClientChannels)
				{
					IMessageSink messageSink2 = CreateClientChannelSinkChain(delayedClientChannel, url, channelDataArray, out objectUri);
					if (messageSink2 != null)
					{
						delayedClientChannels.Remove(delayedClientChannel);
						RegisterChannel(delayedClientChannel);
						return messageSink2;
					}
				}
			}
			objectUri = null;
			return null;
		}

		internal static IMessageSink CreateClientChannelSinkChain(IChannelSender sender, string url, object[] channelDataArray, out string objectUri)
		{
			objectUri = null;
			if (channelDataArray == null)
			{
				return sender.CreateMessageSink(url, null, out objectUri);
			}
			foreach (object obj in channelDataArray)
			{
				IMessageSink messageSink = ((!(obj is IChannelDataStore)) ? sender.CreateMessageSink(url, obj, out objectUri) : sender.CreateMessageSink(null, obj, out objectUri));
				if (messageSink != null)
				{
					return messageSink;
				}
			}
			return null;
		}

		public static IServerChannelSink CreateServerChannelSinkChain(IServerChannelSinkProvider provider, IChannelReceiver channel)
		{
			IServerChannelSinkProvider serverChannelSinkProvider = provider;
			while (serverChannelSinkProvider.Next != null)
			{
				serverChannelSinkProvider = serverChannelSinkProvider.Next;
			}
			serverChannelSinkProvider.Next = new ServerDispatchSinkProvider();
			return provider.CreateSink(channel);
		}

		public static ServerProcessing DispatchMessage(IServerChannelSinkStack sinkStack, IMessage msg, out IMessage replyMsg)
		{
			if (msg == null)
			{
				throw new ArgumentNullException("msg");
			}
			replyMsg = SyncDispatchMessage(msg);
			if (RemotingServices.IsOneWay(((IMethodMessage)msg).MethodBase))
			{
				return ServerProcessing.OneWay;
			}
			return ServerProcessing.Complete;
		}

		public static IChannel GetChannel(string name)
		{
			lock (registeredChannels.SyncRoot)
			{
				foreach (IChannel registeredChannel in registeredChannels)
				{
					if (registeredChannel.ChannelName == name && !(registeredChannel is CrossAppDomainChannel))
					{
						return registeredChannel;
					}
				}
				return null;
			}
		}

		public static IDictionary GetChannelSinkProperties(object obj)
		{
			if (!RemotingServices.IsTransparentProxy(obj))
			{
				throw new ArgumentException("obj must be a proxy", "obj");
			}
			ClientIdentity clientIdentity = (ClientIdentity)RemotingServices.GetRealProxy(obj).ObjectIdentity;
			IMessageSink messageSink = clientIdentity.ChannelSink;
			ArrayList arrayList = new ArrayList();
			while (messageSink != null && !(messageSink is IClientChannelSink))
			{
				messageSink = messageSink.NextSink;
			}
			if (messageSink == null)
			{
				return new Hashtable();
			}
			for (IClientChannelSink clientChannelSink = messageSink as IClientChannelSink; clientChannelSink != null; clientChannelSink = clientChannelSink.NextChannelSink)
			{
				arrayList.Add(clientChannelSink.Properties);
			}
			IDictionary[] dics = (IDictionary[])arrayList.ToArray(typeof(IDictionary[]));
			return new AggregateDictionary(dics);
		}

		public static string[] GetUrlsForObject(MarshalByRefObject obj)
		{
			string objectUri = RemotingServices.GetObjectUri(obj);
			if (objectUri == null)
			{
				return new string[0];
			}
			ArrayList arrayList = new ArrayList();
			lock (registeredChannels.SyncRoot)
			{
				foreach (object registeredChannel in registeredChannels)
				{
					if (!(registeredChannel is CrossAppDomainChannel))
					{
						IChannelReceiver channelReceiver = registeredChannel as IChannelReceiver;
						if (channelReceiver != null)
						{
							arrayList.AddRange(channelReceiver.GetUrlsForUri(objectUri));
						}
					}
				}
			}
			return (string[])arrayList.ToArray(typeof(string));
		}

		[Obsolete("Use RegisterChannel(IChannel,Boolean)")]
		public static void RegisterChannel(IChannel chnl)
		{
			RegisterChannel(chnl, false);
		}

		public static void RegisterChannel(IChannel chnl, bool ensureSecurity)
		{
			if (chnl == null)
			{
				throw new ArgumentNullException("chnl");
			}
			if (ensureSecurity)
			{
				ISecurableChannel securableChannel = chnl as ISecurableChannel;
				if (securableChannel == null)
				{
					throw new RemotingException(string.Format("Channel {0} is not securable while ensureSecurity is specified as true", chnl.ChannelName));
				}
				securableChannel.IsSecured = true;
			}
			lock (registeredChannels.SyncRoot)
			{
				int num = -1;
				for (int i = 0; i < registeredChannels.Count; i++)
				{
					IChannel channel = (IChannel)registeredChannels[i];
					if (channel.ChannelName == chnl.ChannelName && chnl.ChannelName != string.Empty)
					{
						throw new RemotingException("Channel " + channel.ChannelName + " already registered");
					}
					if (channel.ChannelPriority < chnl.ChannelPriority && num == -1)
					{
						num = i;
					}
				}
				if (num != -1)
				{
					registeredChannels.Insert(num, chnl);
				}
				else
				{
					registeredChannels.Add(chnl);
				}
				IChannelReceiver channelReceiver = chnl as IChannelReceiver;
				if (channelReceiver != null && oldStartModeTypes.Contains(chnl.GetType().ToString()))
				{
					channelReceiver.StartListening(null);
				}
			}
		}

		internal static void RegisterChannelConfig(ChannelData channel)
		{
			IServerChannelSinkProvider serverChannelSinkProvider = null;
			IClientChannelSinkProvider clientChannelSinkProvider = null;
			for (int num = channel.ServerProviders.Count - 1; num >= 0; num--)
			{
				ProviderData prov = channel.ServerProviders[num] as ProviderData;
				IServerChannelSinkProvider serverChannelSinkProvider2 = (IServerChannelSinkProvider)CreateProvider(prov);
				serverChannelSinkProvider2.Next = serverChannelSinkProvider;
				serverChannelSinkProvider = serverChannelSinkProvider2;
			}
			for (int num2 = channel.ClientProviders.Count - 1; num2 >= 0; num2--)
			{
				ProviderData prov2 = channel.ClientProviders[num2] as ProviderData;
				IClientChannelSinkProvider clientChannelSinkProvider2 = (IClientChannelSinkProvider)CreateProvider(prov2);
				clientChannelSinkProvider2.Next = clientChannelSinkProvider;
				clientChannelSinkProvider = clientChannelSinkProvider2;
			}
			Type type = Type.GetType(channel.Type);
			if (type == null)
			{
				throw new RemotingException("Type '" + channel.Type + "' not found");
			}
			bool flag = typeof(IChannelSender).IsAssignableFrom(type);
			bool flag2 = typeof(IChannelReceiver).IsAssignableFrom(type);
			Type[] types;
			object[] parameters;
			if (flag && flag2)
			{
				types = new Type[3]
				{
					typeof(IDictionary),
					typeof(IClientChannelSinkProvider),
					typeof(IServerChannelSinkProvider)
				};
				parameters = new object[3] { channel.CustomProperties, clientChannelSinkProvider, serverChannelSinkProvider };
			}
			else if (flag)
			{
				types = new Type[2]
				{
					typeof(IDictionary),
					typeof(IClientChannelSinkProvider)
				};
				parameters = new object[2] { channel.CustomProperties, clientChannelSinkProvider };
			}
			else
			{
				if (!flag2)
				{
					throw new RemotingException(string.Concat(type, " is not a valid channel type"));
				}
				types = new Type[2]
				{
					typeof(IDictionary),
					typeof(IServerChannelSinkProvider)
				};
				parameters = new object[2] { channel.CustomProperties, serverChannelSinkProvider };
			}
			ConstructorInfo constructor = type.GetConstructor(types);
			if (constructor == null)
			{
				throw new RemotingException(string.Concat(type, " does not have a valid constructor"));
			}
			IChannel channel2;
			try
			{
				channel2 = (IChannel)constructor.Invoke(parameters);
			}
			catch (TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
			lock (registeredChannels.SyncRoot)
			{
				if (channel.DelayLoadAsClientChannel == "true" && !(channel2 is IChannelReceiver))
				{
					delayedClientChannels.Add(channel2);
				}
				else
				{
					RegisterChannel(channel2);
				}
			}
		}

		private static object CreateProvider(ProviderData prov)
		{
			Type type = Type.GetType(prov.Type);
			if (type == null)
			{
				throw new RemotingException("Type '" + prov.Type + "' not found");
			}
			object[] args = new object[2] { prov.CustomProperties, prov.CustomData };
			try
			{
				return Activator.CreateInstance(type, args);
			}
			catch (Exception innerException)
			{
				if (innerException is TargetInvocationException)
				{
					innerException = ((TargetInvocationException)innerException).InnerException;
				}
				throw new RemotingException(string.Concat("An instance of provider '", type, "' could not be created: ", innerException.Message));
			}
		}

		public static IMessage SyncDispatchMessage(IMessage msg)
		{
			IMessage message = CheckIncomingMessage(msg);
			if (message != null)
			{
				return CheckReturnMessage(msg, message);
			}
			message = _crossContextSink.SyncProcessMessage(msg);
			return CheckReturnMessage(msg, message);
		}

		public static IMessageCtrl AsyncDispatchMessage(IMessage msg, IMessageSink replySink)
		{
			IMessage message = CheckIncomingMessage(msg);
			if (message != null)
			{
				replySink.SyncProcessMessage(CheckReturnMessage(msg, message));
				return null;
			}
			if (RemotingConfiguration.CustomErrorsEnabled(IsLocalCall(msg)))
			{
				replySink = new ExceptionFilterSink(msg, replySink);
			}
			return _crossContextSink.AsyncProcessMessage(msg, replySink);
		}

		private static ReturnMessage CheckIncomingMessage(IMessage msg)
		{
			IMethodMessage methodMessage = (IMethodMessage)msg;
			ServerIdentity serverIdentity = RemotingServices.GetIdentityForUri(methodMessage.Uri) as ServerIdentity;
			if (serverIdentity == null)
			{
				return new ReturnMessage(new RemotingException("No receiver for uri " + methodMessage.Uri), (IMethodCallMessage)msg);
			}
			RemotingServices.SetMessageTargetIdentity(msg, serverIdentity);
			return null;
		}

		internal static IMessage CheckReturnMessage(IMessage callMsg, IMessage retMsg)
		{
			IMethodReturnMessage methodReturnMessage = retMsg as IMethodReturnMessage;
			if (methodReturnMessage != null && methodReturnMessage.Exception != null && RemotingConfiguration.CustomErrorsEnabled(IsLocalCall(callMsg)))
			{
				Exception e = new Exception("Server encountered an internal error. For more information, turn off customErrors in the server's .config file.");
				retMsg = new MethodResponse(e, (IMethodCallMessage)callMsg);
			}
			return retMsg;
		}

		private static bool IsLocalCall(IMessage callMsg)
		{
			return true;
		}

		public static void UnregisterChannel(IChannel chnl)
		{
			if (chnl == null)
			{
				throw new ArgumentNullException();
			}
			lock (registeredChannels.SyncRoot)
			{
				for (int i = 0; i < registeredChannels.Count; i++)
				{
					if (registeredChannels[i] == chnl)
					{
						registeredChannels.RemoveAt(i);
						IChannelReceiver channelReceiver = chnl as IChannelReceiver;
						if (channelReceiver != null)
						{
							channelReceiver.StopListening(null);
						}
						return;
					}
				}
				throw new RemotingException("Channel not registered");
			}
		}

		internal static object[] GetCurrentChannelInfo()
		{
			ArrayList arrayList = new ArrayList();
			lock (registeredChannels.SyncRoot)
			{
				foreach (object registeredChannel in registeredChannels)
				{
					IChannelReceiver channelReceiver = registeredChannel as IChannelReceiver;
					if (channelReceiver != null)
					{
						object channelData = channelReceiver.ChannelData;
						if (channelData != null)
						{
							arrayList.Add(channelData);
						}
					}
				}
			}
			return arrayList.ToArray();
		}
	}
}
