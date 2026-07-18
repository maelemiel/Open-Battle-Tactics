using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;

namespace System.Runtime.Remoting
{
	internal abstract class Identity
	{
		protected string _objectUri;

		protected IMessageSink _channelSink;

		protected IMessageSink _envoySink;

		private DynamicPropertyCollection _clientDynamicProperties;

		private DynamicPropertyCollection _serverDynamicProperties;

		protected ObjRef _objRef;

		private bool _disposed;

		public bool IsFromThisAppDomain
		{
			get
			{
				return _channelSink == null;
			}
		}

		public IMessageSink ChannelSink
		{
			get
			{
				return _channelSink;
			}
			set
			{
				_channelSink = value;
			}
		}

		public IMessageSink EnvoySink
		{
			get
			{
				return _envoySink;
			}
		}

		public string ObjectUri
		{
			get
			{
				return _objectUri;
			}
			set
			{
				_objectUri = value;
			}
		}

		public bool IsConnected
		{
			get
			{
				return _objectUri != null;
			}
		}

		public bool Disposed
		{
			get
			{
				return _disposed;
			}
			set
			{
				_disposed = value;
			}
		}

		public DynamicPropertyCollection ClientDynamicProperties
		{
			get
			{
				if (_clientDynamicProperties == null)
				{
					_clientDynamicProperties = new DynamicPropertyCollection();
				}
				return _clientDynamicProperties;
			}
		}

		public DynamicPropertyCollection ServerDynamicProperties
		{
			get
			{
				if (_serverDynamicProperties == null)
				{
					_serverDynamicProperties = new DynamicPropertyCollection();
				}
				return _serverDynamicProperties;
			}
		}

		public bool HasClientDynamicSinks
		{
			get
			{
				return _clientDynamicProperties != null && _clientDynamicProperties.HasProperties;
			}
		}

		public bool HasServerDynamicSinks
		{
			get
			{
				return _serverDynamicProperties != null && _serverDynamicProperties.HasProperties;
			}
		}

		public Identity(string objectUri)
		{
			_objectUri = objectUri;
		}

		public abstract ObjRef CreateObjRef(Type requestedType);

		public void NotifyClientDynamicSinks(bool start, IMessage req_msg, bool client_site, bool async)
		{
			if (_clientDynamicProperties != null && _clientDynamicProperties.HasProperties)
			{
				_clientDynamicProperties.NotifyMessage(start, req_msg, client_site, async);
			}
		}

		public void NotifyServerDynamicSinks(bool start, IMessage req_msg, bool client_site, bool async)
		{
			if (_serverDynamicProperties != null && _serverDynamicProperties.HasProperties)
			{
				_serverDynamicProperties.NotifyMessage(start, req_msg, client_site, async);
			}
		}
	}
}
