using System;
using System.IO;

namespace Microsoft.Win32.SafeHandles
{
	public sealed class SafeFileHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		public SafeFileHandle(IntPtr preexistingHandle, bool ownsHandle)
			: base(ownsHandle)
		{
			SetHandle(preexistingHandle);
		}

		internal SafeFileHandle()
			: base(true)
		{
		}

		protected override bool ReleaseHandle()
		{
			MonoIOError error;
			MonoIO.Close(handle, out error);
			return error == MonoIOError.ERROR_SUCCESS;
		}
	}
}
