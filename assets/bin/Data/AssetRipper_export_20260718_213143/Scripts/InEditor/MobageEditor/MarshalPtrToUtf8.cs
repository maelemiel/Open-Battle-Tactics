using System;
using System.Runtime.InteropServices;
using System.Text;

namespace MobageEditor
{
	public class MarshalPtrToUtf8 : ICustomMarshaler
	{
		private static MarshalPtrToUtf8 marshaler = new MarshalPtrToUtf8();

		public void CleanUpManagedData(object ManagedObj)
		{
		}

		public void CleanUpNativeData(IntPtr pNativeData)
		{
			Marshal.Release(pNativeData);
		}

		public int GetNativeDataSize()
		{
			return Marshal.SizeOf(typeof(byte));
		}

		public int GetNativeDataSize(IntPtr ptr)
		{
			int num = 0;
			for (num = 0; Marshal.ReadByte(ptr, num) > 0; num++)
			{
			}
			return num;
		}

		public IntPtr MarshalManagedToNative(object ManagedObj)
		{
			if (ManagedObj == null)
			{
				return IntPtr.Zero;
			}
			if (ManagedObj.GetType() != typeof(string))
			{
				throw new ArgumentException("ManagedObj", "Can only marshal type of System.String");
			}
			byte[] bytes = Encoding.UTF8.GetBytes((string)ManagedObj);
			int num = Marshal.SizeOf(bytes[0]) * bytes.Length + Marshal.SizeOf(bytes[0]);
			IntPtr intPtr = Marshal.AllocHGlobal(num);
			Marshal.Copy(bytes, 0, intPtr, bytes.Length);
			Marshal.WriteByte(intPtr, num - 1, 0);
			return intPtr;
		}

		public object MarshalNativeToManaged(IntPtr pNativeData)
		{
			if (pNativeData == IntPtr.Zero)
			{
				return null;
			}
			int nativeDataSize = GetNativeDataSize(pNativeData);
			byte[] array = new byte[nativeDataSize - 1];
			Marshal.Copy(pNativeData, array, 0, nativeDataSize - 1);
			return Encoding.UTF8.GetString(array);
		}

		public static ICustomMarshaler GetInstance(string cookie)
		{
			return marshaler;
		}
	}
}
