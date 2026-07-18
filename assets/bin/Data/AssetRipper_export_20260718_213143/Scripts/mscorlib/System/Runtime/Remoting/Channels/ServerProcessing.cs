using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Channels
{
	[Serializable]
	[ComVisible(true)]
	public enum ServerProcessing
	{
		Complete = 0,
		OneWay = 1,
		Async = 2
	}
}
