using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Serializable]
	[ComVisible(true)]
	public delegate bool MemberFilter(MemberInfo m, object filterCriteria);
}
