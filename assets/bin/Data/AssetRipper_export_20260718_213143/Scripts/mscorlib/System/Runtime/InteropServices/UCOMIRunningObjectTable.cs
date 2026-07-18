namespace System.Runtime.InteropServices
{
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("00000010-0000-0000-c000-000000000046")]
	[Obsolete]
	public interface UCOMIRunningObjectTable
	{
		void Register(int grfFlags, [MarshalAs(UnmanagedType.Interface)] object punkObject, UCOMIMoniker pmkObjectName, out int pdwRegister);

		void Revoke(int dwRegister);

		void IsRunning(UCOMIMoniker pmkObjectName);

		void GetObject(UCOMIMoniker pmkObjectName, [MarshalAs(UnmanagedType.Interface)] out object ppunkObject);

		void NoteChangeTime(int dwRegister, ref FILETIME pfiletime);

		void GetTimeOfLastChange(UCOMIMoniker pmkObjectName, out FILETIME pfiletime);

		void EnumRunning(out UCOMIEnumMoniker ppenumMoniker);
	}
}
