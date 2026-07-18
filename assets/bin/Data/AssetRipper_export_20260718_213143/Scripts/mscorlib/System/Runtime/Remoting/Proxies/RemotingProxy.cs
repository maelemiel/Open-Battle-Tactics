using System.Reflection;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Messaging;
using System.Threading;

namespace System.Runtime.Remoting.Proxies
{
	internal class RemotingProxy : RealProxy, IRemotingTypeInfo
	{
		private static MethodInfo _cache_GetTypeMethod = typeof(object).GetMethod("GetType");

		private static MethodInfo _cache_GetHashCodeMethod = typeof(object).GetMethod("GetHashCode");

		private IMessageSink _sink;

		private bool _hasEnvoySink;

		private ConstructionCall _ctorCall;

		public string TypeName
		{
			get
			{
				if (_objectIdentity is ClientIdentity)
				{
					ObjRef objRef = _objectIdentity.CreateObjRef(null);
					if (objRef.TypeInfo != null)
					{
						return objRef.TypeInfo.TypeName;
					}
				}
				return GetProxiedType().AssemblyQualifiedName;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		internal RemotingProxy(Type type, ClientIdentity identity)
			: base(type, identity)
		{
			_sink = identity.ChannelSink;
			_hasEnvoySink = false;
			_targetUri = identity.TargetUri;
		}

		internal RemotingProxy(Type type, string activationUrl, object[] activationAttributes)
			: base(type)
		{
			_hasEnvoySink = false;
			_ctorCall = ActivationServices.CreateConstructionCall(type, activationUrl, activationAttributes);
		}

		public override IMessage Invoke(IMessage request)
		{
			IMethodCallMessage methodCallMessage = request as IMethodCallMessage;
			if (methodCallMessage != null)
			{
				if (methodCallMessage.MethodBase == _cache_GetHashCodeMethod)
				{
					return new MethodResponse(base.ObjectIdentity.GetHashCode(), null, null, methodCallMessage);
				}
				if (methodCallMessage.MethodBase == _cache_GetTypeMethod)
				{
					return new MethodResponse(GetProxiedType(), null, null, methodCallMessage);
				}
			}
			IInternalMessage internalMessage = request as IInternalMessage;
			if (internalMessage != null)
			{
				if (internalMessage.Uri == null)
				{
					internalMessage.Uri = _targetUri;
				}
				internalMessage.TargetIdentity = _objectIdentity;
			}
			_objectIdentity.NotifyClientDynamicSinks(true, request, true, false);
			IMessageSink messageSink = ((!Thread.CurrentContext.HasExitSinks || _hasEnvoySink) ? _sink : Thread.CurrentContext.GetClientContextSinkChain());
			MonoMethodMessage monoMethodMessage = request as MonoMethodMessage;
			IMessage result;
			if (monoMethodMessage == null || monoMethodMessage.CallType == CallType.Sync)
			{
				result = messageSink.SyncProcessMessage(request);
			}
			else
			{
				AsyncResult asyncResult = monoMethodMessage.AsyncResult;
				IMessageCtrl messageCtrl = messageSink.AsyncProcessMessage(request, asyncResult);
				if (asyncResult != null)
				{
					asyncResult.SetMessageCtrl(messageCtrl);
				}
				result = new ReturnMessage(null, new object[0], 0, null, monoMethodMessage);
			}
			_objectIdentity.NotifyClientDynamicSinks(false, request, true, false);
			return result;
		}

		internal void AttachIdentity(Identity identity)
		{
			_objectIdentity = identity;
			if (identity is ClientActivatedIdentity)
			{
				ClientActivatedIdentity clientActivatedIdentity = (ClientActivatedIdentity)identity;
				_targetContext = clientActivatedIdentity.Context;
				AttachServer(clientActivatedIdentity.GetServerObject());
				clientActivatedIdentity.SetClientProxy((MarshalByRefObject)GetTransparentProxy());
			}
			if (identity is ClientIdentity)
			{
				((ClientIdentity)identity).ClientProxy = (MarshalByRefObject)GetTransparentProxy();
				_targetUri = ((ClientIdentity)identity).TargetUri;
			}
			else
			{
				_targetUri = identity.ObjectUri;
			}
			if (_objectIdentity.EnvoySink != null)
			{
				_sink = _objectIdentity.EnvoySink;
				_hasEnvoySink = true;
			}
			else
			{
				_sink = _objectIdentity.ChannelSink;
			}
			_ctorCall = null;
		}

		internal IMessage ActivateRemoteObject(IMethodMessage request)
		{
			if (_ctorCall == null)
			{
				return new ConstructionResponse(this, null, (IMethodCallMessage)request);
			}
			_ctorCall.CopyFrom(request);
			return ActivationServices.Activate(this, _ctorCall);
		}

		public bool CanCastTo(Type fromType, object o)
		{
			if (_objectIdentity is ClientIdentity)
			{
				ObjRef objRef = _objectIdentity.CreateObjRef(null);
				if (objRef.IsReferenceToWellKnow && (fromType.IsInterface || GetProxiedType() == typeof(MarshalByRefObject)))
				{
					return true;
				}
				if (objRef.TypeInfo != null)
				{
					return objRef.TypeInfo.CanCastTo(fromType, o);
				}
			}
			return fromType.IsAssignableFrom(GetProxiedType());
		}

		~RemotingProxy()
		{
			if (_objectIdentity != null && !(_objectIdentity is ClientActivatedIdentity))
			{
				RemotingServices.DisposeIdentity(_objectIdentity);
			}
		}
	}
}
