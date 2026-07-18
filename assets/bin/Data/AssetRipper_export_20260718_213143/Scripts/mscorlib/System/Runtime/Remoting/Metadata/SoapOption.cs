using System.Runtime.InteropServices;

namespace System.Runtime.Remoting.Metadata
{
	[Serializable]
	[ComVisible(true)]
	[Flags]
	public enum SoapOption
	{
		None = 0,
		AlwaysIncludeTypes = 1,
		XsdString = 2,
		EmbedAll = 4,
		Option1 = 8,
		Option2 = 0x10
	}
}
