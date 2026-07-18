using System.Runtime.InteropServices;

namespace System.ComponentModel.Design
{
	[ComVisible(true)]
	public enum ViewTechnology
	{
		[Obsolete("Use ViewTechnology.Default.")]
		Passthrough = 0,
		[Obsolete("Use ViewTechnology.Default.")]
		WindowsForms = 1,
		Default = 2
	}
}
