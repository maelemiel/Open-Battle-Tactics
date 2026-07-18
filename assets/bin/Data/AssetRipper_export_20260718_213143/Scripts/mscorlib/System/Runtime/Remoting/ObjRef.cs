using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Channels;
using System.Runtime.Serialization;

namespace System.Runtime.Remoting
{
	[Serializable]
	[ComVisible(true)]
	public class ObjRef : ISerializable, IObjectReference
	{
		private IChannelInfo channel_info;

		private string uri;

		private IRemotingTypeInfo typeInfo;

		private IEnvoyInfo envoyInfo;

		private int flags;

		private Type _serverType;

		private static int MarshalledObjectRef = 1;

		private static int WellKnowObjectRef = 2;

		internal bool IsReferenceToWellKnow
		{
			get
			{
				return (flags & WellKnowObjectRef) > 0;
			}
		}

		public virtual IChannelInfo ChannelInfo
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get
			{
				return channel_info;
			}
			set
			{
				channel_info = value;
			}
		}

		public virtual IEnvoyInfo EnvoyInfo
		{
			get
			{
				return envoyInfo;
			}
			set
			{
				envoyInfo = value;
			}
		}

		public virtual IRemotingTypeInfo TypeInfo
		{
			get
			{
				return typeInfo;
			}
			set
			{
				typeInfo = value;
			}
		}

		public virtual string URI
		{
			get
			{
				return uri;
			}
			set
			{
				uri = value;
			}
		}

		internal Type ServerType
		{
			get
			{
				if (_serverType == null)
				{
					_serverType = Type.GetType(typeInfo.TypeName);
				}
				return _serverType;
			}
		}

		public ObjRef()
		{
			UpdateChannelInfo();
		}

		internal ObjRef(string typeName, string uri, IChannelInfo cinfo)
		{
			this.uri = uri;
			channel_info = cinfo;
			typeInfo = new TypeInfo(Type.GetType(typeName, true));
		}

		internal ObjRef(ObjRef o, bool unmarshalAsProxy)
		{
			channel_info = o.channel_info;
			uri = o.uri;
			typeInfo = o.typeInfo;
			envoyInfo = o.envoyInfo;
			flags = o.flags;
			if (unmarshalAsProxy)
			{
				flags |= MarshalledObjectRef;
			}
		}

		public ObjRef(MarshalByRefObject o, Type requestedType)
		{
			if (o == null)
			{
				throw new ArgumentNullException("o");
			}
			if (requestedType == null)
			{
				throw new ArgumentNullException("requestedType");
			}
			uri = RemotingServices.GetObjectUri(o);
			typeInfo = new TypeInfo(requestedType);
			if (!requestedType.IsInstanceOfType(o))
			{
				throw new RemotingException("The server object type cannot be cast to the requested type " + requestedType.FullName);
			}
			UpdateChannelInfo();
		}

		internal ObjRef(Type type, string url, object remoteChannelData)
		{
			uri = url;
			typeInfo = new TypeInfo(type);
			if (remoteChannelData != null)
			{
				channel_info = new ChannelInfo(remoteChannelData);
			}
			flags |= WellKnowObjectRef;
		}

		protected ObjRef(SerializationInfo info, StreamingContext context)
		{
			SerializationInfoEnumerator enumerator = info.GetEnumerator();
			bool flag = true;
			while (enumerator.MoveNext())
			{
				switch (enumerator.Name)
				{
				case "uri":
					uri = (string)enumerator.Value;
					break;
				case "typeInfo":
					typeInfo = (IRemotingTypeInfo)enumerator.Value;
					break;
				case "channelInfo":
					channel_info = (IChannelInfo)enumerator.Value;
					break;
				case "envoyInfo":
					envoyInfo = (IEnvoyInfo)enumerator.Value;
					break;
				case "fIsMarshalled":
				{
					object value = enumerator.Value;
					if (((!(value is string)) ? ((int)value) : ((IConvertible)value).ToInt32(null)) == 0)
					{
						flag = false;
					}
					break;
				}
				case "objrefFlags":
					flags = Convert.ToInt32(enumerator.Value);
					break;
				default:
					throw new NotSupportedException();
				}
			}
			if (flag)
			{
				flags |= MarshalledObjectRef;
			}
		}

		internal bool IsPossibleToCAD()
		{
			return false;
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.SetType(GetType());
			info.AddValue("uri", uri);
			info.AddValue("typeInfo", typeInfo, typeof(IRemotingTypeInfo));
			info.AddValue("envoyInfo", envoyInfo, typeof(IEnvoyInfo));
			info.AddValue("channelInfo", channel_info, typeof(IChannelInfo));
			info.AddValue("objrefFlags", flags);
		}

		public virtual object GetRealObject(StreamingContext context)
		{
			if ((flags & MarshalledObjectRef) > 0)
			{
				return RemotingServices.Unmarshal(this);
			}
			return this;
		}

		public bool IsFromThisAppDomain()
		{
			Identity identityForUri = RemotingServices.GetIdentityForUri(uri);
			if (identityForUri == null)
			{
				return false;
			}
			return identityForUri.IsFromThisAppDomain;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public bool IsFromThisProcess()
		{
			object[] channelData = channel_info.ChannelData;
			foreach (object obj in channelData)
			{
				if (obj is CrossAppDomainData)
				{
					string processID = ((CrossAppDomainData)obj).ProcessID;
					return processID == RemotingConfiguration.ProcessId;
				}
			}
			return true;
		}

		internal void UpdateChannelInfo()
		{
			channel_info = new ChannelInfo();
		}
	}
}
