using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Serializable]
	[Flags]
	[ComVisible(true)]
	public enum ResourceAttributes
	{
		Public = 1,
		Private = 2
	}
}
