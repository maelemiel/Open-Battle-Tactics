using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;

namespace System.Runtime.Remoting.Proxies
{
	[ComVisible(true)]
	public abstract class RealProxy
	{
		private Type class_to_proxy;

		internal Context _targetContext;

		private MarshalByRefObject _server;

		private int _targetDomainId = -1;

		internal string _targetUri;

		internal Identity _objectIdentity;

		private object _objTP;

		private object _stubData;

		internal Identity ObjectIdentity
		{
			get
			{
				return _objectIdentity;
			}
			set
			{
				_objectIdentity = value;
			}
		}

		protected RealProxy()
		{
		}

		protected RealProxy(Type classToProxy)
			: this(classToProxy, IntPtr.Zero, null)
		{
		}

		internal RealProxy(Type classToProxy, ClientIdentity identity)
			: this(classToProxy, IntPtr.Zero, null)
		{
			_objectIdentity = identity;
		}

		protected RealProxy(Type classToProxy, IntPtr stub, object stubData)
		{
			if (!classToProxy.IsMarshalByRef && !classToProxy.IsInterface)
			{
				throw new ArgumentException("object must be MarshalByRef");
			}
			class_to_proxy = classToProxy;
			if (stub != IntPtr.Zero)
			{
				throw new NotSupportedException("stub is not used in Mono");
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Type InternalGetProxyType(object transparentProxy);

		public Type GetProxiedType()
		{
			if (_objTP == null)
			{
				if (class_to_proxy.IsInterface)
				{
					return typeof(MarshalByRefObject);
				}
				return class_to_proxy;
			}
			return InternalGetProxyType(_objTP);
		}

		public virtual ObjRef CreateObjRef(Type requestedType)
		{
			return RemotingServices.Marshal((MarshalByRefObject)GetTransparentProxy(), null, requestedType);
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			object transparentProxy = GetTransparentProxy();
			RemotingServices.GetObjectData(transparentProxy, info, context);
		}

		[MonoTODO]
		public virtual IntPtr GetCOMIUnknown(bool fIsMarshalled)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public virtual void SetCOMIUnknown(IntPtr i)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public virtual IntPtr SupportsInterface(ref Guid iid)
		{
			throw new NotImplementedException();
		}

		public static object GetStubData(RealProxy rp)
		{
			return rp._stubData;
		}

		public static void SetStubData(RealProxy rp, object stubData)
		{
			rp._stubData = stubData;
		}

		public abstract IMessage Invoke(IMessage msg);

		internal static object PrivateInvoke(RealProxy rp, IMessage msg, out Exception exc, out object[] out_args)
		{
			MonoMethodMessage monoMethodMessage = (MonoMethodMessage)msg;
			monoMethodMessage.LogicalCallContext = CallContext.CreateLogicalCallContext(true);
			CallType callType = monoMethodMessage.CallType;
			bool flag = rp is RemotingProxy;
			out_args = null;
			IMethodReturnMessage methodReturnMessage = null;
			if (callType == CallType.BeginInvoke)
			{
				monoMethodMessage.AsyncResult.CallMessage = monoMethodMessage;
			}
			if (callType == CallType.EndInvoke)
			{
				methodReturnMessage = (IMethodReturnMessage)monoMethodMessage.AsyncResult.EndInvoke();
			}
			if (monoMethodMessage.MethodBase.IsConstructor)
			{
				if (flag)
				{
					methodReturnMessage = (IMethodReturnMessage)(rp as RemotingProxy).ActivateRemoteObject((IMethodMessage)msg);
				}
				else
				{
					msg = new ConstructionCall(rp.GetProxiedType());
				}
			}
			if (methodReturnMessage == null)
			{
				bool flag2 = false;
				try
				{
					methodReturnMessage = (IMethodReturnMessage)rp.Invoke(msg);
				}
				catch (Exception e)
				{
					flag2 = true;
					if (callType != CallType.BeginInvoke)
					{
						throw;
					}
					monoMethodMessage.AsyncResult.SyncProcessMessage(new ReturnMessage(e, msg as IMethodCallMessage));
					methodReturnMessage = new ReturnMessage(null, null, 0, null, msg as IMethodCallMessage);
				}
				if (!flag && callType == CallType.BeginInvoke && !flag2)
				{
					IMessage message = null;
					message = monoMethodMessage.AsyncResult.SyncProcessMessage(methodReturnMessage);
					out_args = methodReturnMessage.OutArgs;
					methodReturnMessage = new ReturnMessage(message, null, 0, null, methodReturnMessage as IMethodCallMessage);
				}
			}
			if (methodReturnMessage.LogicalCallContext != null && methodReturnMessage.LogicalCallContext.HasInfo)
			{
				CallContext.UpdateCurrentCallContext(methodReturnMessage.LogicalCallContext);
			}
			exc = methodReturnMessage.Exception;
			if (exc != null)
			{
				out_args = null;
				throw exc.FixRemotingException();
			}
			if (methodReturnMessage is IConstructionReturnMessage)
			{
				if (out_args == null)
				{
					out_args = methodReturnMessage.OutArgs;
				}
			}
			else if (monoMethodMessage.CallType != CallType.BeginInvoke)
			{
				if (monoMethodMessage.CallType == CallType.Sync)
				{
					out_args = ProcessResponse(methodReturnMessage, monoMethodMessage);
				}
				else if (monoMethodMessage.CallType == CallType.EndInvoke)
				{
					out_args = ProcessResponse(methodReturnMessage, monoMethodMessage.AsyncResult.CallMessage);
				}
				else if (out_args == null)
				{
					out_args = methodReturnMessage.OutArgs;
				}
			}
			return methodReturnMessage.ReturnValue;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal virtual extern object InternalGetTransparentProxy(string className);

		public virtual object GetTransparentProxy()
		{
			if (_objTP == null)
			{
				IRemotingTypeInfo remotingTypeInfo = this as IRemotingTypeInfo;
				string text;
				if (remotingTypeInfo != null)
				{
					text = remotingTypeInfo.TypeName;
					if (text == null || text == typeof(MarshalByRefObject).AssemblyQualifiedName)
					{
						text = class_to_proxy.AssemblyQualifiedName;
					}
				}
				else
				{
					text = class_to_proxy.AssemblyQualifiedName;
				}
				_objTP = InternalGetTransparentProxy(text);
			}
			return _objTP;
		}

		[ComVisible(true)]
		[MonoTODO]
		public IConstructionReturnMessage InitializeServerObject(IConstructionCallMessage ctorMsg)
		{
			throw new NotImplementedException();
		}

		protected void AttachServer(MarshalByRefObject s)
		{
			_server = s;
		}

		protected MarshalByRefObject DetachServer()
		{
			MarshalByRefObject server = _server;
			_server = null;
			return server;
		}

		protected MarshalByRefObject GetUnwrappedServer()
		{
			return _server;
		}

		internal void SetTargetDomain(int domainId)
		{
			_targetDomainId = domainId;
		}

		internal object GetAppDomainTarget()
		{
			if (_server == null)
			{
				ClientActivatedIdentity clientActivatedIdentity = RemotingServices.GetIdentityForUri(_targetUri) as ClientActivatedIdentity;
				if (clientActivatedIdentity == null)
				{
					throw new RemotingException("Server for uri '" + _targetUri + "' not found");
				}
				_server = clientActivatedIdentity.GetServerObject();
			}
			return _server;
		}

		private static object[] ProcessResponse(IMethodReturnMessage mrm, MonoMethodMessage call)
		{
			MethodInfo methodInfo = (MethodInfo)call.MethodBase;
			if (mrm.ReturnValue != null && !methodInfo.ReturnType.IsInstanceOfType(mrm.ReturnValue))
			{
				throw new InvalidCastException("Return value has an invalid type");
			}
			int outCount;
			if (call.NeedsOutProcessing(out outCount))
			{
				ParameterInfo[] parameters = methodInfo.GetParameters();
				object[] array = new object[outCount];
				int num = 0;
				ParameterInfo[] array2 = parameters;
				foreach (ParameterInfo parameterInfo in array2)
				{
					if (parameterInfo.IsOut && !parameterInfo.ParameterType.IsByRef)
					{
						object obj = ((parameterInfo.Position >= mrm.ArgCount) ? null : mrm.GetArg(parameterInfo.Position));
						if (obj != null)
						{
							object arg = call.GetArg(parameterInfo.Position);
							if (arg == null)
							{
								throw new RemotingException("Unexpected null value in local out parameter '" + parameterInfo.Name + "'");
							}
							RemotingServices.UpdateOutArgObject(parameterInfo, arg, obj);
						}
					}
					else if (parameterInfo.ParameterType.IsByRef)
					{
						object obj2 = ((parameterInfo.Position >= mrm.ArgCount) ? null : mrm.GetArg(parameterInfo.Position));
						if (obj2 != null && !parameterInfo.ParameterType.GetElementType().IsInstanceOfType(obj2))
						{
							throw new InvalidCastException("Return argument '" + parameterInfo.Name + "' has an invalid type");
						}
						array[num++] = obj2;
					}
				}
				return array;
			}
			return new object[0];
		}
	}
}
