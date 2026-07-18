using System.Runtime.InteropServices;

namespace System.Globalization
{
	[Serializable]
	[Flags]
	[ComVisible(true)]
	public enum CultureTypes
	{
		NeutralCultures = 1,
		SpecificCultures = 2,
		InstalledWin32Cultures = 4,
		AllCultures = 7,
		UserCustomCulture = 8,
		ReplacementCultures = 0x10,
		WindowsOnlyCultures = 0x20,
		FrameworkCultures = 0x40
	}
}
