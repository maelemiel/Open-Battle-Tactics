using System.Runtime.InteropServices;

namespace System.Security.Policy
{
	[Serializable]
	[ComVisible(true)]
	[Flags]
	public enum PolicyStatementAttribute
	{
		Nothing = 0,
		Exclusive = 1,
		LevelFinal = 2,
		All = 3
	}
}
