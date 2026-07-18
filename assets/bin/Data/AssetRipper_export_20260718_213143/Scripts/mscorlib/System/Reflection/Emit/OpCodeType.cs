using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[Serializable]
	[ComVisible(true)]
	public enum OpCodeType
	{
		[Obsolete("This API has been deprecated.")]
		Annotation = 0,
		Macro = 1,
		Nternal = 2,
		Objmodel = 3,
		Prefix = 4,
		Primitive = 5
	}
}
