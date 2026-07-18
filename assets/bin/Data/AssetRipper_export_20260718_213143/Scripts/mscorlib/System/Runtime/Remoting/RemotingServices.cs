using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Lifetime;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Services;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace System.Runtime.Remoting
{
	[ComVisible(true)]
	public sealed class RemotingServices
	{
		[Serializable]
		private class CACD
		{
			public object d;

			public object c;
		}

		private static Hashtable uri_hash;

		private static BinaryFormatter _serializationFormatter;

		private static BinaryFormatter _deserializationFormatter;

		internal static string app_id;

		private static int next_id;

		private static readonly BindingFlags methodBindings;

		private static readonly MethodInfo FieldSetterMethod;

		private static readonly MethodInfo FieldGetterMethod;

		private RemotingServices()
		{
		}

		static RemotingServices()
		{
			uri_hash = new Hashtable();
			next_id = 1;
			methodBindings = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			RemotingSurrogateSelector selector = new RemotingSurrogateSelector();
			StreamingContext context = new StreamingContext(StreamingContextStates.Remoting, null);
			_serializationFormatter = new BinaryFormatter(selector, context);
			_deserializationFormatter = new BinaryFormatter(null, context);
			_serializationFormatter.AssemblyFormat = FormatterAssemblyStyle.Full;
			_deserializationFormatter.AssemblyFormat = FormatterAssemblyStyle.Full;
			RegisterInternalChannels();
			app_id = Guid.NewGuid().ToString().Replace('-', '_') + "/";
			CreateWellKnownServerIdentity(typeof(RemoteActivator), "RemoteActivationService.rem", WellKnownObjectMode.Singleton);
			FieldSetterMethod = typeof(object).GetMethod("FieldSetter", BindingFlags.Instance | BindingFlags.NonPublic);
			FieldGetterMethod = typeof(object).GetMethod("FieldGetter", BindingFlags.Instance | BindingFlags.NonPublic);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern object InternalExecute(MethodBase method, object obj, object[] parameters, out object[] out_args);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern MethodBase GetVirtualMethod(Type type, MethodBase method);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static extern bool IsTransparentProxy(object proxy);

		internal static IMethodReturnMessage InternalExecuteMessage(MarshalByRefObject target, IMethodCallMessage reqMsg)
		{
			Type type = target.GetType();
			MethodBase methodBase;
			if (reqMsg.MethodBase.DeclaringType == type || reqMsg.MethodBase == FieldSetterMethod || reqMsg.MethodBase == FieldGetterMethod)
			{
				methodBase = reqMsg.MethodBase;
			}
			else
			{
				methodBase = GetVirtualMethod(type, reqMsg.MethodBase);
				if (methodBase == null)
				{
					throw new RemotingException(string.Format("Cannot resolve method {0}:{1}", type, reqMsg.MethodName));
				}
			}
			if (reqMsg.MethodBase.IsGenericMethod)
			{
				Type[] genericArguments = reqMsg.MethodBase.GetGenericArguments();
				methodBase = ((MethodInfo)methodBase).MakeGenericMethod(genericArguments);
			}
			object oldContext = CallContext.SetCurrentCallContext(reqMsg.LogicalCallContext);
			ReturnMessage result;
			try
			{
				object[] out_args;
				object ret = InternalExecute(methodBase, target, reqMsg.Args, out out_args);
				ParameterInfo[] parameters = methodBase.GetParameters();
				object[] array = new object[parameters.Length];
				int outArgsCount = 0;
				int num = 0;
				ParameterInfo[] array2 = parameters;
				foreach (ParameterInfo parameterInfo in array2)
				{
					if (parameterInfo.IsOut && !parameterInfo.ParameterType.IsByRef)
					{
						array[outArgsCount++] = reqMsg.GetArg(parameterInfo.Position);
					}
					else if (parameterInfo.ParameterType.IsByRef)
					{
						array[outArgsCount++] = out_args[num++];
					}
					else
					{
						array[outArgsCount++] = null;
					}
				}
				result = new ReturnMessage(ret, array, outArgsCount, CallContext.CreateLogicalCallContext(true), reqMsg);
			}
			catch (Exception e)
			{
				result = new ReturnMessage(e, reqMsg);
			}
			CallContext.RestoreCallContext(oldContext);
			return result;
		}

		public static IMethodReturnMessage ExecuteMessage(MarshalByRefObject target, IMethodCallMessage reqMsg)
		{
			if (IsTransparentProxy(target))
			{
				RealProxy realProxy = GetRealProxy(target);
				return (IMethodReturnMessage)realProxy.Invoke(reqMsg);
			}
			return InternalExecuteMessage(target, reqMsg);
		}

		[ComVisible(true)]
		public static object Connect(Type classToProxy, string url)
		{
			ObjRef objRef = new ObjRef(classToProxy, url, null);
			return GetRemoteObject(objRef, classToProxy);
		}

		[ComVisible(true)]
		public static object Connect(Type classToProxy, string url, object data)
		{
			ObjRef objRef = new ObjRef(classToProxy, url, data);
			return GetRemoteObject(objRef, classToProxy);
		}

		public static bool Disconnect(MarshalByRefObject obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			ServerIdentity serverIdentity;
			if (IsTransparentProxy(obj))
			{
				RealProxy realProxy = GetRealProxy(obj);
				if (!realProxy.GetProxiedType().IsContextful || !(realProxy.ObjectIdentity is ServerIdentity))
				{
					throw new ArgumentException("The obj parameter is a proxy.");
				}
				serverIdentity = realProxy.ObjectIdentity as ServerIdentity;
			}
			else
			{
				serverIdentity = obj.ObjectIdentity;
				obj.ObjectIdentity = null;
			}
			if (serverIdentity == null || !serverIdentity.IsConnected)
			{
				return false;
			}
			LifetimeServices.StopTrackingLifetime(serverIdentity);
			DisposeIdentity(serverIdentity);
			TrackingServices.NotifyDisconnectedObject(obj);
			return true;
		}

		public static Type GetServerTypeForUri(string URI)
		{
			ServerIdentity serverIdentity = GetIdentityForUri(URI) as ServerIdentity;
			if (serverIdentity == null)
			{
				return null;
			}
			return serverIdentity.ObjectType;
		}

		public static string GetObjectUri(MarshalByRefObject obj)
		{
			Identity objectIdentity = GetObjectIdentity(obj);
			if (objectIdentity is ClientIdentity)
			{
				return ((ClientIdentity)objectIdentity).TargetUri;
			}
			if (objectIdentity != null)
			{
				return objectIdentity.ObjectUri;
			}
			return null;
		}

		public static object Unmarshal(ObjRef objectRef)
		{
			return Unmarshal(objectRef, true);
		}

		public static object Unmarshal(ObjRef objectRef, bool fRefine)
		{
			Type type = ((!fRefine) ? typeof(MarshalByRefObject) : objectRef.ServerType);
			if (type == null)
			{
				type = typeof(MarshalByRefObject);
			}
			if (objectRef.IsReferenceToWellKnow)
			{
				object remoteObject = GetRemoteObject(objectRef, type);
				TrackingServices.NotifyUnmarshaledObject(remoteObject, objectRef);
				return remoteObject;
			}
			object transparentProxy;
			if (type.IsContextful)
			{
				ProxyAttribute proxyAttribute = (ProxyAttribute)Attribute.GetCustomAttribute(type, typeof(ProxyAttribute), true);
				if (proxyAttribute != null)
				{
					transparentProxy = proxyAttribute.CreateProxy(objectRef, type, null, null).GetTransparentProxy();
					TrackingServices.NotifyUnmarshaledObject(transparentProxy, objectRef);
					return transparentProxy;
				}
			}
			transparentProxy = GetProxyForRemoteObject(objectRef, type);
			TrackingServices.NotifyUnmarshaledObject(transparentProxy, objectRef);
			return transparentProxy;
		}

		public static ObjRef Marshal(MarshalByRefObject Obj)
		{
			return Marshal(Obj, null, null);
		}

		public static ObjRef Marshal(MarshalByRefObject Obj, string URI)
		{
			return Marshal(Obj, URI, null);
		}

		public static ObjRef Marshal(MarshalByRefObject Obj, string ObjURI, Type RequestedType)
		{
			if (IsTransparentProxy(Obj))
			{
				RealProxy realProxy = GetRealProxy(Obj);
				Identity objectIdentity = realProxy.ObjectIdentity;
				if (objectIdentity != null)
				{
					if (realProxy.GetProxiedType().IsContextful && !objectIdentity.IsConnected)
					{
						ClientActivatedIdentity clientActivatedIdentity = (ClientActivatedIdentity)objectIdentity;
						if (ObjURI == null)
						{
							ObjURI = NewUri();
						}
						clientActivatedIdentity.ObjectUri = ObjURI;
						RegisterServerIdentity(clientActivatedIdentity);
						clientActivatedIdentity.StartTrackingLifetime((ILease)Obj.InitializeLifetimeService());
						return clientActivatedIdentity.CreateObjRef(RequestedType);
					}
					if (ObjURI != null)
					{
						throw new RemotingException("It is not possible marshal a proxy of a remote object.");
					}
					ObjRef objRef = realProxy.ObjectIdentity.CreateObjRef(RequestedType);
					TrackingServices.NotifyMarshaledObject(Obj, objRef);
					return objRef;
				}
			}
			if (RequestedType == null)
			{
				RequestedType = Obj.GetType();
			}
			if (ObjURI == null)
			{
				if (Obj.ObjectIdentity == null)
				{
					ObjURI = NewUri();
					CreateClientActivatedServerIdentity(Obj, RequestedType, ObjURI);
				}
			}
			else
			{
				ClientActivatedIdentity clientActivatedIdentity2 = GetIdentityForUri("/" + ObjURI) as ClientActivatedIdentity;
				if (clientActivatedIdentity2 == null || Obj != clientActivatedIdentity2.GetServerObject())
				{
					CreateClientActivatedServerIdentity(Obj, RequestedType, ObjURI);
				}
			}
			ObjRef objRef2 = ((!IsTransparentProxy(Obj)) ? Obj.CreateObjRef(RequestedType) : GetRealProxy(Obj).ObjectIdentity.CreateObjRef(RequestedType));
			TrackingServices.NotifyMarshaledObject(Obj, objRef2);
			return objRef2;
		}

		private static string NewUri()
		{
			int num = Interlocked.Increment(ref next_id);
			return app_id + Environment.TickCount.ToString("x") + "_" + num + ".rem";
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static RealProxy GetRealProxy(object proxy)
		{
			if (!IsTransparentProxy(proxy))
			{
				throw new RemotingException("Cannot get the real proxy from an object that is not a transparent proxy.");
			}
			return ((TransparentProxy)proxy)._rp;
		}

		public static MethodBase GetMethodBaseFromMethodMessage(IMethodMessage msg)
		{
			Type type = Type.GetType(msg.TypeName);
			if (type == null)
			{
				throw new RemotingException("Type '" + msg.TypeName + "' not found.");
			}
			return GetMethodBaseFromName(type, msg.MethodName, (Type[])msg.MethodSignature);
		}

		internal static MethodBase GetMethodBaseFromName(Type type, string methodName, Type[] signature)
		{
			if (type.IsInterface)
			{
				return FindInterfaceMethod(type, methodName, signature);
			}
			MethodBase methodBase = null;
			methodBase = ((signature != null) ? type.GetMethod(methodName, methodBindings, null, signature, null) : type.GetMethod(methodName, methodBindings));
			if (methodBase != null)
			{
				return methodBase;
			}
			if (methodName == "FieldSetter")
			{
				return FieldSetterMethod;
			}
			if (methodName == "FieldGetter")
			{
				return FieldGetterMethod;
			}
			if (signature == null)
			{
				return type.GetConstructor(methodBindings, null, Type.EmptyTypes, null);
			}
			return type.GetConstructor(methodBindings, null, signature, null);
		}

		private static MethodBase FindInterfaceMethod(Type type, string methodName, Type[] signature)
		{
			MethodBase methodBase = null;
			methodBase = ((signature != null) ? type.GetMethod(methodName, methodBindings, null, signature, null) : type.GetMethod(methodName, methodBindings));
			if (methodBase != null)
			{
				return methodBase;
			}
			Type[] interfaces = type.GetInterfaces();
			foreach (Type type2 in interfaces)
			{
				methodBase = FindInterfaceMethod(type2, methodName, signature);
				if (methodBase != null)
				{
					return methodBase;
				}
			}
			return null;
		}

		public static void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			ObjRef objRef = Marshal((MarshalByRefObject)obj);
			objRef.GetObjectData(info, context);
		}

		public static ObjRef GetObjRefForProxy(MarshalByRefObject obj)
		{
			Identity objectIdentity = GetObjectIdentity(obj);
			if (objectIdentity == null)
			{
				return null;
			}
			return objectIdentity.CreateObjRef(null);
		}

		public static object GetLifetimeService(MarshalByRefObject obj)
		{
			if (obj == null)
			{
				return null;
			}
			return obj.GetLifetimeService();
		}

		public static IMessageSink GetEnvoyChainForProxy(MarshalByRefObject obj)
		{
			if (IsTransparentProxy(obj))
			{
				return ((ClientIdentity)GetRealProxy(obj).ObjectIdentity).EnvoySink;
			}
			throw new ArgumentException("obj must be a proxy.", "obj");
		}

		[Obsolete("It existed for only internal use in .NET and unimplemented in mono")]
		[Conditional("REMOTING_PERF")]
		[MonoTODO]
		public static void LogRemotingStage(int stage)
		{
			throw new NotImplementedException();
		}

		public static string GetSessionIdForMethodMessage(IMethodMessage msg)
		{
			return msg.Uri;
		}

		public static bool IsMethodOverloaded(IMethodMessage msg)
		{
			MonoType monoType = (MonoType)msg.MethodBase.DeclaringType;
			return monoType.GetMethodsByName(msg.MethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, false, monoType).Length > 1;
		}

		public static bool IsObjectOutOfAppDomain(object tp)
		{
			MarshalByRefObject marshalByRefObject = tp as MarshalByRefObject;
			if (marshalByRefObject == null)
			{
				return false;
			}
			Identity objectIdentity = GetObjectIdentity(marshalByRefObject);
			return objectIdentity is ClientIdentity;
		}

		public static bool IsObjectOutOfContext(object tp)
		{
			MarshalByRefObject marshalByRefObject = tp as MarshalByRefObject;
			if (marshalByRefObject == null)
			{
				return false;
			}
			Identity objectIdentity = GetObjectIdentity(marshalByRefObject);
			if (objectIdentity == null)
			{
				return false;
			}
			ServerIdentity serverIdentity = objectIdentity as ServerIdentity;
			if (serverIdentity != null)
			{
				return serverIdentity.Context != Thread.CurrentContext;
			}
			return true;
		}

		public static bool IsOneWay(MethodBase method)
		{
			return method.IsDefined(typeof(OneWayAttribute), false);
		}

		internal static bool IsAsyncMessage(IMessage msg)
		{
			if (!(msg is MonoMethodMessage))
			{
				return false;
			}
			if (((MonoMethodMessage)msg).IsAsync)
			{
				return true;
			}
			if (IsOneWay(((MonoMethodMessage)msg).MethodBase))
			{
				return true;
			}
			return false;
		}

		public static void SetObjectUriForMarshal(MarshalByRefObject obj, string uri)
		{
			if (IsTransparentProxy(obj))
			{
				RealProxy realProxy = GetRealProxy(obj);
				Identity objectIdentity = realProxy.ObjectIdentity;
				if (objectIdentity != null && !(objectIdentity is ServerIdentity) && !realProxy.GetProxiedType().IsContextful)
				{
					throw new RemotingException("SetObjectUriForMarshal method should only be called for MarshalByRefObjects that exist in the current AppDomain.");
				}
			}
			Marshal(obj, uri);
		}

		internal static object CreateClientProxy(ActivatedClientTypeEntry entry, object[] activationAttributes)
		{
			if (entry.ContextAttributes != null || activationAttributes != null)
			{
				ArrayList arrayList = new ArrayList();
				if (entry.ContextAttributes != null)
				{
					arrayList.AddRange(entry.ContextAttributes);
				}
				if (activationAttributes != null)
				{
					arrayList.AddRange(activationAttributes);
				}
				return CreateClientProxy(entry.ObjectType, entry.ApplicationUrl, arrayList.ToArray());
			}
			return CreateClientProxy(entry.ObjectType, entry.ApplicationUrl, null);
		}

		internal static object CreateClientProxy(Type objectType, string url, object[] activationAttributes)
		{
			string text = url;
			if (!text.EndsWith("/"))
			{
				text += "/";
			}
			text += "RemoteActivationService.rem";
			string objectUri;
			GetClientChannelSinkChain(text, null, out objectUri);
			RemotingProxy remotingProxy = new RemotingProxy(objectType, text, activationAttributes);
			return remotingProxy.GetTransparentProxy();
		}

		internal static object CreateClientProxy(WellKnownClientTypeEntry entry)
		{
			return Connect(entry.ObjectType, entry.ObjectUrl, null);
		}

		internal static object CreateClientProxyForContextBound(Type type, object[] activationAttributes)
		{
			if (type.IsContextful)
			{
				ProxyAttribute proxyAttribute = (ProxyAttribute)Attribute.GetCustomAttribute(type, typeof(ProxyAttribute), true);
				if (proxyAttribute != null)
				{
					return proxyAttribute.CreateInstance(type);
				}
			}
			RemotingProxy remotingProxy = new RemotingProxy(type, ChannelServices.CrossContextUrl, activationAttributes);
			return remotingProxy.GetTransparentProxy();
		}

		internal static Identity GetIdentityForUri(string uri)
		{
			string normalizedUri = GetNormalizedUri(uri);
			lock (uri_hash)
			{
				Identity identity = (Identity)uri_hash[normalizedUri];
				if (identity == null)
				{
					normalizedUri = RemoveAppNameFromUri(uri);
					if (normalizedUri != null)
					{
						identity = (Identity)uri_hash[normalizedUri];
					}
				}
				return identity;
			}
		}

		private static string RemoveAppNameFromUri(string uri)
		{
			string applicationName = RemotingConfiguration.ApplicationName;
			if (applicationName == null)
			{
				return null;
			}
			applicationName = "/" + applicationName + "/";
			if (uri.StartsWith(applicationName))
			{
				return uri.Substring(applicationName.Length);
			}
			return null;
		}

		internal static Identity GetObjectIdentity(MarshalByRefObject obj)
		{
			if (IsTransparentProxy(obj))
			{
				return GetRealProxy(obj).ObjectIdentity;
			}
			return obj.ObjectIdentity;
		}

		internal static ClientIdentity GetOrCreateClientIdentity(ObjRef objRef, Type proxyType, out object clientProxy)
		{
			object channelData = ((objRef.ChannelInfo == null) ? null : objRef.ChannelInfo.ChannelData);
			string objectUri;
			IMessageSink clientChannelSinkChain = GetClientChannelSinkChain(objRef.URI, channelData, out objectUri);
			if (objectUri == null)
			{
				objectUri = objRef.URI;
			}
			lock (uri_hash)
			{
				clientProxy = null;
				string normalizedUri = GetNormalizedUri(objRef.URI);
				ClientIdentity clientIdentity = uri_hash[normalizedUri] as ClientIdentity;
				if (clientIdentity != null)
				{
					clientProxy = clientIdentity.ClientProxy;
					if (clientProxy != null)
					{
						return clientIdentity;
					}
					DisposeIdentity(clientIdentity);
				}
				clientIdentity = new ClientIdentity(objectUri, objRef);
				clientIdentity.ChannelSink = clientChannelSinkChain;
				uri_hash[normalizedUri] = clientIdentity;
				if (proxyType != null)
				{
					RemotingProxy remotingProxy = new RemotingProxy(proxyType, clientIdentity);
					CrossAppDomainSink crossAppDomainSink = clientChannelSinkChain as CrossAppDomainSink;
					if (crossAppDomainSink != null)
					{
						remotingProxy.SetTargetDomain(crossAppDomainSink.TargetDomainId);
					}
					clientProxy = remotingProxy.GetTransparentProxy();
					clientIdentity.ClientProxy = (MarshalByRefObject)clientProxy;
				}
				return clientIdentity;
			}
		}

		private static IMessageSink GetClientChannelSinkChain(string url, object channelData, out string objectUri)
		{
			IMessageSink messageSink = ChannelServices.CreateClientChannelSinkChain(url, channelData, out objectUri);
			if (messageSink == null)
			{
				if (url != null)
				{
					string message = string.Format("Cannot create channel sink to connect to URL {0}. An appropriate channel has probably not been registered.", url);
					throw new RemotingException(message);
				}
				string message2 = string.Format("Cannot create channel sink to connect to the remote object. An appropriate channel has probably not been registered.", url);
				throw new RemotingException(message2);
			}
			return messageSink;
		}

		internal static ClientActivatedIdentity CreateContextBoundObjectIdentity(Type objectType)
		{
			ClientActivatedIdentity clientActivatedIdentity = new ClientActivatedIdentity(null, objectType);
			clientActivatedIdentity.ChannelSink = ChannelServices.CrossContextChannel;
			return clientActivatedIdentity;
		}

		internal static ClientActivatedIdentity CreateClientActivatedServerIdentity(MarshalByRefObject realObject, Type objectType, string objectUri)
		{
			ClientActivatedIdentity clientActivatedIdentity = new ClientActivatedIdentity(objectUri, objectType);
			clientActivatedIdentity.AttachServerObject(realObject, Context.DefaultContext);
			RegisterServerIdentity(clientActivatedIdentity);
			clientActivatedIdentity.StartTrackingLifetime((ILease)realObject.InitializeLifetimeService());
			return clientActivatedIdentity;
		}

		internal static ServerIdentity CreateWellKnownServerIdentity(Type objectType, string objectUri, WellKnownObjectMode mode)
		{
			ServerIdentity serverIdentity = ((mode != WellKnownObjectMode.SingleCall) ? ((ServerIdentity)new SingletonIdentity(objectUri, Context.DefaultContext, objectType)) : ((ServerIdentity)new SingleCallIdentity(objectUri, Context.DefaultContext, objectType)));
			RegisterServerIdentity(serverIdentity);
			return serverIdentity;
		}

		private static void RegisterServerIdentity(ServerIdentity identity)
		{
			lock (uri_hash)
			{
				if (uri_hash.ContainsKey(identity.ObjectUri))
				{
					throw new RemotingException("Uri already in use: " + identity.ObjectUri + ".");
				}
				uri_hash[identity.ObjectUri] = identity;
			}
		}

		internal static object GetProxyForRemoteObject(ObjRef objref, Type classToProxy)
		{
			ClientActivatedIdentity clientActivatedIdentity = GetIdentityForUri(objref.URI) as ClientActivatedIdentity;
			if (clientActivatedIdentity != null)
			{
				return clientActivatedIdentity.GetServerObject();
			}
			return GetRemoteObject(objref, classToProxy);
		}

		internal static object GetRemoteObject(ObjRef objRef, Type proxyType)
		{
			object clientProxy;
			GetOrCreateClientIdentity(objRef, proxyType, out clientProxy);
			return clientProxy;
		}

		internal static object GetServerObject(string uri)
		{
			ClientActivatedIdentity clientActivatedIdentity = GetIdentityForUri(uri) as ClientActivatedIdentity;
			if (clientActivatedIdentity == null)
			{
				throw new RemotingException("Server for uri '" + uri + "' not found");
			}
			return clientActivatedIdentity.GetServerObject();
		}

		internal static byte[] SerializeCallData(object obj)
		{
			LogicalCallContext logicalCallContext = CallContext.CreateLogicalCallContext(false);
			if (logicalCallContext != null)
			{
				CACD cACD = new CACD();
				cACD.d = obj;
				cACD.c = logicalCallContext;
				obj = cACD;
			}
			if (obj == null)
			{
				return null;
			}
			MemoryStream memoryStream = new MemoryStream();
			_serializationFormatter.Serialize(memoryStream, obj);
			return memoryStream.ToArray();
		}

		internal static object DeserializeCallData(byte[] array)
		{
			if (array == null)
			{
				return null;
			}
			MemoryStream serializationStream = new MemoryStream(array);
			object obj = _deserializationFormatter.Deserialize(serializationStream);
			if (obj is CACD)
			{
				CACD cACD = (CACD)obj;
				obj = cACD.d;
				CallContext.UpdateCurrentCallContext((LogicalCallContext)cACD.c);
			}
			return obj;
		}

		internal static byte[] SerializeExceptionData(Exception ex)
		{
			try
			{
				int num = 4;
				do
				{
					try
					{
						MemoryStream memoryStream = new MemoryStream();
						_serializationFormatter.Serialize(memoryStream, ex);
						return memoryStream.ToArray();
					}
					catch (Exception ex2)
					{
						if (ex2 is ThreadAbortException)
						{
							Thread.ResetAbort();
							num = 5;
							ex = ex2;
						}
						else if (num == 2)
						{
							ex = new Exception();
							ex.SetMessage(ex2.Message);
							ex.SetStackTrace(ex2.StackTrace);
						}
						else
						{
							ex = ex2;
						}
					}
					num--;
				}
				while (num > 0);
				return null;
			}
			catch (Exception ex3)
			{
				byte[] result = SerializeExceptionData(ex3);
				Thread.ResetAbort();
				return result;
			}
		}

		internal static object GetDomainProxy(AppDomain domain)
		{
			byte[] array = null;
			Context currentContext = Thread.CurrentContext;
			try
			{
				array = (byte[])AppDomain.InvokeInDomain(domain, typeof(AppDomain).GetMethod("GetMarshalledDomainObjRef", BindingFlags.Instance | BindingFlags.NonPublic), domain, null);
			}
			finally
			{
				AppDomain.InternalSetContext(currentContext);
			}
			byte[] array2 = new byte[array.Length];
			array.CopyTo(array2, 0);
			MemoryStream mem = new MemoryStream(array2);
			ObjRef objectRef = (ObjRef)CADSerializer.DeserializeObject(mem);
			return (AppDomain)Unmarshal(objectRef);
		}

		private static void RegisterInternalChannels()
		{
			CrossAppDomainChannel.RegisterCrossAppDomainChannel();
		}

		internal static void DisposeIdentity(Identity ident)
		{
			lock (uri_hash)
			{
				if (!ident.Disposed)
				{
					ClientIdentity clientIdentity = ident as ClientIdentity;
					if (clientIdentity != null)
					{
						uri_hash.Remove(GetNormalizedUri(clientIdentity.TargetUri));
					}
					else
					{
						uri_hash.Remove(ident.ObjectUri);
					}
					ident.Disposed = true;
				}
			}
		}

		internal static Identity GetMessageTargetIdentity(IMessage msg)
		{
			if (msg is IInternalMessage)
			{
				return ((IInternalMessage)msg).TargetIdentity;
			}
			lock (uri_hash)
			{
				string normalizedUri = GetNormalizedUri(((IMethodMessage)msg).Uri);
				return uri_hash[normalizedUri] as ServerIdentity;
			}
		}

		internal static void SetMessageTargetIdentity(IMessage msg, Identity ident)
		{
			if (msg is IInternalMessage)
			{
				((IInternalMessage)msg).TargetIdentity = ident;
			}
		}

		internal static bool UpdateOutArgObject(ParameterInfo pi, object local, object remote)
		{
			if (pi.ParameterType.IsArray && ((Array)local).Rank == 1)
			{
				Array array = (Array)local;
				if (array.Rank == 1)
				{
					Array.Copy((Array)remote, array, array.Length);
					return true;
				}
			}
			return false;
		}

		private static string GetNormalizedUri(string uri)
		{
			if (uri.StartsWith("/"))
			{
				return uri.Substring(1);
			}
			return uri;
		}
	}
}
