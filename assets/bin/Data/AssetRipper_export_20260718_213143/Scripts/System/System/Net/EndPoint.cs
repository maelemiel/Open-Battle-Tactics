using System.Net.Sockets;

namespace System.Net
{
	[Serializable]
	public abstract class EndPoint
	{
		public virtual AddressFamily AddressFamily
		{
			get
			{
				throw NotImplemented();
			}
		}

		public virtual EndPoint Create(SocketAddress address)
		{
			throw NotImplemented();
		}

		public virtual SocketAddress Serialize()
		{
			throw NotImplemented();
		}

		private static Exception NotImplemented()
		{
			return new NotImplementedException();
		}
	}
}
