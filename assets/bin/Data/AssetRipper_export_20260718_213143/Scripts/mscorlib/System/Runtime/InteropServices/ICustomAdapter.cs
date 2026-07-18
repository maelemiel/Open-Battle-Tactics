namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	public interface ICustomAdapter
	{
		[return: MarshalAs(UnmanagedType.IUnknown)]
		object GetUnderlyingObject();
	}
}
