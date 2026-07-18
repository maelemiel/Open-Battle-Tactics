using System.Runtime.InteropServices;

namespace System
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("27FFF232-A7A8-40DD-8D4A-734AD59fCD41")]
	[ComVisible(true)]
	public interface IAppDomainSetup
	{
		string ApplicationBase { get; set; }

		string ApplicationName { get; set; }

		string CachePath { get; set; }

		string ConfigurationFile { get; set; }

		string DynamicBase { get; set; }

		string LicenseFile { get; set; }

		string PrivateBinPath { get; set; }

		string PrivateBinPathProbe { get; set; }

		string ShadowCopyDirectories { get; set; }

		string ShadowCopyFiles { get; set; }
	}
}
