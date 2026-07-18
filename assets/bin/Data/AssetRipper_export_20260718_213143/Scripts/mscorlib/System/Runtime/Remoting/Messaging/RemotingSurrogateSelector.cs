using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Runtime.Remoting.Messaging
{
	[ComVisible(true)]
	public class RemotingSurrogateSelector : ISurrogateSelector
	{
		private static Type s_cachedTypeObjRef = typeof(ObjRef);

		private static ObjRefSurrogate _objRefSurrogate = new ObjRefSurrogate();

		private static RemotingSurrogate _objRemotingSurrogate = new RemotingSurrogate();

		private object _rootObj;

		private MessageSurrogateFilter _filter;

		private ISurrogateSelector _next;

		public MessageSurrogateFilter Filter
		{
			get
			{
				return _filter;
			}
			set
			{
				_filter = value;
			}
		}

		public virtual void ChainSelector(ISurrogateSelector selector)
		{
			if (_next != null)
			{
				selector.ChainSelector(_next);
			}
			_next = selector;
		}

		public virtual ISurrogateSelector GetNextSelector()
		{
			return _next;
		}

		public object GetRootObject()
		{
			return _rootObj;
		}

		public virtual ISerializationSurrogate GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector ssout)
		{
			if (type.IsMarshalByRef)
			{
				ssout = this;
				return _objRemotingSurrogate;
			}
			if (s_cachedTypeObjRef.IsAssignableFrom(type))
			{
				ssout = this;
				return _objRefSurrogate;
			}
			if (_next != null)
			{
				return _next.GetSurrogate(type, context, out ssout);
			}
			ssout = null;
			return null;
		}

		public void SetRootObject(object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException();
			}
			_rootObj = obj;
		}

		[MonoTODO]
		public virtual void UseSoapFormat()
		{
			throw new NotImplementedException();
		}
	}
}
