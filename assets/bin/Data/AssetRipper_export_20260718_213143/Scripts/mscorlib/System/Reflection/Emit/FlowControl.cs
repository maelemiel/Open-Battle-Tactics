using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[Serializable]
	[ComVisible(true)]
	public enum FlowControl
	{
		Branch = 0,
		Break = 1,
		Call = 2,
		Cond_Branch = 3,
		Meta = 4,
		Next = 5,
		[Obsolete("This API has been deprecated.")]
		Phi = 6,
		Return = 7,
		Throw = 8
	}
}
