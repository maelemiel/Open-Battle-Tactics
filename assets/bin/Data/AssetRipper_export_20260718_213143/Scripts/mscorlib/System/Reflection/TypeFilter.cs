using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Serializable]
	[ComVisible(true)]
	public delegate bool TypeFilter(Type m, object filterCriteria);
}
