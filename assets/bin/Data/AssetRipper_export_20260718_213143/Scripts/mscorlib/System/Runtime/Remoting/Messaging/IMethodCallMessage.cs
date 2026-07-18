using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Messaging
{
	[ComVisible(true)]
	public interface IMethodCallMessage : IMessage, IMethodMessage
	{
		int InArgCount { get; }

		object[] InArgs { get; }

		object GetInArg(int argNum);

		string GetInArgName(int index);
	}
}
