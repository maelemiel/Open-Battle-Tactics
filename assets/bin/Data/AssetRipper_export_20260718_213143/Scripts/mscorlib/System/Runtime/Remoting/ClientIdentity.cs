using System.Runtime.Remoting.Messaging;

namespace System.Runtime.Remoting
{
	internal class ClientIdentity : Identity
	{
		private WeakReference _proxyReference;

		public MarshalByRefObject ClientProxy
		{
			get
			{
				return (MarshalByRefObject)_proxyReference.Target;
			}
			set
			{
				_proxyReference = new WeakReference(value);
			}
		}

		public string TargetUri
		{
			get
			{
				return _objRef.URI;
			}
		}

		public ClientIdentity(string objectUri, ObjRef objRef)
			: base(objectUri)
		{
			_objRef = objRef;
			object envoySink;
			if (_objRef.EnvoyInfo != null)
			{
				IMessageSink envoySinks = _objRef.EnvoyInfo.EnvoySinks;
				envoySink = envoySinks;
			}
			else
			{
				envoySink = null;
			}
			_envoySink = (IMessageSink)envoySink;
		}

		public override ObjRef CreateObjRef(Type requestedType)
		{
			return _objRef;
		}
	}
}
