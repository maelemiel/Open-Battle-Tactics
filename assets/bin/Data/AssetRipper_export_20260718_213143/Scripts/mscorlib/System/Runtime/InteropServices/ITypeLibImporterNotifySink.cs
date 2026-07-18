using System.Reflection;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[Guid("f1c3bf76-c3e4-11d3-88e7-00902754c43a")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface ITypeLibImporterNotifySink
	{
		void ReportEvent(ImporterEventKind eventKind, int eventCode, string eventMsg);

		Assembly ResolveRef([MarshalAs(UnmanagedType.Interface)] object typeLib);
	}
}
