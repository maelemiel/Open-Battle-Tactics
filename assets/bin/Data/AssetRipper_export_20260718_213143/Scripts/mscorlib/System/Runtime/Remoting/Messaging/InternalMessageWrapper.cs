using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Messaging
{
	[ComVisible(true)]
	public class InternalMessageWrapper
	{
		protected IMessage WrappedMessage;

		public InternalMessageWrapper(IMessage msg)
		{
			WrappedMessage = msg;
		}
	}
}
