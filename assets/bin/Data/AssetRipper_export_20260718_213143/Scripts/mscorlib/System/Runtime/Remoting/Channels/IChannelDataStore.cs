using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Channels
{
	[ComVisible(true)]
	public interface IChannelDataStore
	{
		string[] ChannelUris { get; }

		object this[object key] { get; set; }
	}
}
