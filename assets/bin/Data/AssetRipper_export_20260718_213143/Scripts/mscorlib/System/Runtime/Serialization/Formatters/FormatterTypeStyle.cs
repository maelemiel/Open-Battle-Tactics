using System.Runtime.InteropServices;

namespace System.Runtime.Serialization.Formatters
{
	[Serializable]
	[ComVisible(true)]
	public enum FormatterTypeStyle
	{
		TypesWhenNeeded = 0,
		TypesAlways = 1,
		XsdString = 2
	}
}
