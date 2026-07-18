using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Microsoft.Win32
{
	internal class Win32RegistryApi : IRegistryApi
	{
		private const int OpenRegKeyRead = 131097;

		private const int OpenRegKeyWrite = 131078;

		private const int Int32ByteSize = 4;

		private const int BufferMaxLength = 1024;

		private readonly int NativeBytesPerCharacter = Marshal.SystemDefaultCharSize;

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		private static extern int RegCreateKey(IntPtr keyBase, string keyName, out IntPtr keyHandle);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		private static extern int RegCloseKey(IntPtr keyHandle);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		private static extern int RegConnectRegistry(string machineName, IntPtr hKey, out IntPtr keyHandle);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		private static extern int RegFlushKey(IntPtr keyHandle);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		private static extern int RegOpenKeyEx(IntPtr keyBase, string keyName, IntPtr reserved, int access, out IntPtr keyHandle);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		private static extern int RegDeleteKey(IntPtr keyHandle, string valueName);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		private static extern int RegDeleteValue(IntPtr keyHandle, string valueName);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		private static extern int RegEnumKey(IntPtr keyBase, int index, StringBuilder nameBuffer, int bufferLength);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		private static extern int RegEnumValue(IntPtr keyBase, int index, StringBuilder nameBuffer, ref int nameLength, IntPtr reserved, ref RegistryValueKind type, IntPtr data, IntPtr dataLength);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		private static extern int RegSetValueEx(IntPtr keyBase, string valueName, IntPtr reserved, RegistryValueKind type, string data, int rawDataLength);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		private static extern int RegSetValueEx(IntPtr keyBase, string valueName, IntPtr reserved, RegistryValueKind type, byte[] rawData, int rawDataLength);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		private static extern int RegSetValueEx(IntPtr keyBase, string valueName, IntPtr reserved, RegistryValueKind type, ref int data, int rawDataLength);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		private static extern int RegQueryValueEx(IntPtr keyBase, string valueName, IntPtr reserved, ref RegistryValueKind type, IntPtr zero, ref int dataSize);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		private static extern int RegQueryValueEx(IntPtr keyBase, string valueName, IntPtr reserved, ref RegistryValueKind type, [Out] byte[] data, ref int dataSize);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		private static extern int RegQueryValueEx(IntPtr keyBase, string valueName, IntPtr reserved, ref RegistryValueKind type, ref int data, ref int dataSize);

		private static IntPtr GetHandle(RegistryKey key)
		{
			return (IntPtr)key.Handle;
		}

		private static bool IsHandleValid(RegistryKey key)
		{
			return key.Handle != null;
		}

		public object GetValue(RegistryKey rkey, string name, object defaultValue, RegistryValueOptions options)
		{
			RegistryValueKind type = RegistryValueKind.Unknown;
			int dataSize = 0;
			object obj = null;
			IntPtr handle = GetHandle(rkey);
			int num = RegQueryValueEx(handle, name, IntPtr.Zero, ref type, IntPtr.Zero, ref dataSize);
			switch (num)
			{
			case 2:
			case 1018:
				return defaultValue;
			default:
				GenerateException(num);
				break;
			case 0:
			case 234:
				break;
			}
			switch (type)
			{
			case RegistryValueKind.String:
			{
				byte[] data2;
				num = GetBinaryValue(rkey, name, type, out data2, dataSize);
				obj = RegistryKey.DecodeString(data2);
				break;
			}
			case RegistryValueKind.ExpandString:
			{
				byte[] data5;
				num = GetBinaryValue(rkey, name, type, out data5, dataSize);
				obj = RegistryKey.DecodeString(data5);
				if ((options & RegistryValueOptions.DoNotExpandEnvironmentNames) == 0)
				{
					obj = Environment.ExpandEnvironmentVariables((string)obj);
				}
				break;
			}
			case RegistryValueKind.DWord:
			{
				int data4 = 0;
				num = RegQueryValueEx(handle, name, IntPtr.Zero, ref type, ref data4, ref dataSize);
				obj = data4;
				break;
			}
			case RegistryValueKind.Binary:
			{
				byte[] data3;
				num = GetBinaryValue(rkey, name, type, out data3, dataSize);
				obj = data3;
				break;
			}
			case RegistryValueKind.MultiString:
			{
				obj = null;
				byte[] data;
				num = GetBinaryValue(rkey, name, type, out data, dataSize);
				if (num == 0)
				{
					obj = RegistryKey.DecodeString(data).Split(default(char));
				}
				break;
			}
			default:
				throw new SystemException();
			}
			if (num != 0)
			{
				GenerateException(num);
			}
			return obj;
		}

		public void SetValue(RegistryKey rkey, string name, object value, RegistryValueKind valueKind)
		{
			Type type = value.GetType();
			IntPtr handle = GetHandle(rkey);
			int num;
			if (valueKind == RegistryValueKind.DWord && type == typeof(int))
			{
				int data = (int)value;
				num = RegSetValueEx(handle, name, IntPtr.Zero, RegistryValueKind.DWord, ref data, 4);
			}
			else if (valueKind == RegistryValueKind.Binary && type == typeof(byte[]))
			{
				byte[] array = (byte[])value;
				num = RegSetValueEx(handle, name, IntPtr.Zero, RegistryValueKind.Binary, array, array.Length);
			}
			else if (valueKind == RegistryValueKind.MultiString && type == typeof(string[]))
			{
				string[] array2 = (string[])value;
				StringBuilder stringBuilder = new StringBuilder();
				string[] array3 = array2;
				foreach (string value2 in array3)
				{
					stringBuilder.Append(value2);
					stringBuilder.Append('\0');
				}
				stringBuilder.Append('\0');
				byte[] bytes = Encoding.Unicode.GetBytes(stringBuilder.ToString());
				num = RegSetValueEx(handle, name, IntPtr.Zero, RegistryValueKind.MultiString, bytes, bytes.Length);
			}
			else
			{
				if ((valueKind != RegistryValueKind.String && valueKind != RegistryValueKind.ExpandString) || type != typeof(string))
				{
					if (type.IsArray)
					{
						throw new ArgumentException("Only string and byte arrays can written as registry values");
					}
					throw new ArgumentException("Type does not match the valueKind");
				}
				string text = string.Format("{0}{1}", value, '\0');
				num = RegSetValueEx(handle, name, IntPtr.Zero, valueKind, text, text.Length * NativeBytesPerCharacter);
			}
			if (num != 0)
			{
				GenerateException(num);
			}
		}

		public void SetValue(RegistryKey rkey, string name, object value)
		{
			Type type = value.GetType();
			IntPtr handle = GetHandle(rkey);
			int num;
			if (type == typeof(int))
			{
				int data = (int)value;
				num = RegSetValueEx(handle, name, IntPtr.Zero, RegistryValueKind.DWord, ref data, 4);
			}
			else if (type == typeof(byte[]))
			{
				byte[] array = (byte[])value;
				num = RegSetValueEx(handle, name, IntPtr.Zero, RegistryValueKind.Binary, array, array.Length);
			}
			else if (type == typeof(string[]))
			{
				string[] array2 = (string[])value;
				StringBuilder stringBuilder = new StringBuilder();
				string[] array3 = array2;
				foreach (string value2 in array3)
				{
					stringBuilder.Append(value2);
					stringBuilder.Append('\0');
				}
				stringBuilder.Append('\0');
				byte[] bytes = Encoding.Unicode.GetBytes(stringBuilder.ToString());
				num = RegSetValueEx(handle, name, IntPtr.Zero, RegistryValueKind.MultiString, bytes, bytes.Length);
			}
			else
			{
				if (type.IsArray)
				{
					throw new ArgumentException("Only string and byte arrays can written as registry values");
				}
				string text = string.Format("{0}{1}", value, '\0');
				num = RegSetValueEx(handle, name, IntPtr.Zero, RegistryValueKind.String, text, text.Length * NativeBytesPerCharacter);
			}
			switch (num)
			{
			case 1018:
				throw RegistryKey.CreateMarkedForDeletionException();
			case 0:
				return;
			}
			GenerateException(num);
		}

		private int GetBinaryValue(RegistryKey rkey, string name, RegistryValueKind type, out byte[] data, int size)
		{
			byte[] array = new byte[size];
			IntPtr handle = GetHandle(rkey);
			int result = RegQueryValueEx(handle, name, IntPtr.Zero, ref type, array, ref size);
			data = array;
			return result;
		}

		public int SubKeyCount(RegistryKey rkey)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			IntPtr handle = GetHandle(rkey);
			int num = 0;
			while (true)
			{
				int num2 = RegEnumKey(handle, num, stringBuilder, stringBuilder.Capacity);
				switch (num2)
				{
				case 1018:
					throw RegistryKey.CreateMarkedForDeletionException();
				default:
					GenerateException(num2);
					break;
				case 0:
					break;
				case 259:
					return num;
				}
				num++;
			}
		}

		public int ValueCount(RegistryKey rkey)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			IntPtr handle = GetHandle(rkey);
			int num = 0;
			while (true)
			{
				RegistryValueKind type = RegistryValueKind.Unknown;
				int nameLength = stringBuilder.Capacity;
				int num2 = RegEnumValue(handle, num, stringBuilder, ref nameLength, IntPtr.Zero, ref type, IntPtr.Zero, IntPtr.Zero);
				switch (num2)
				{
				case 1018:
					throw RegistryKey.CreateMarkedForDeletionException();
				default:
					GenerateException(num2);
					break;
				case 0:
				case 234:
					break;
				case 259:
					return num;
				}
				num++;
			}
		}

		public RegistryKey OpenRemoteBaseKey(RegistryHive hKey, string machineName)
		{
			IntPtr hKey2 = new IntPtr((int)hKey);
			IntPtr keyHandle;
			int num = RegConnectRegistry(machineName, hKey2, out keyHandle);
			if (num != 0)
			{
				GenerateException(num);
			}
			return new RegistryKey(hKey, keyHandle, true);
		}

		public RegistryKey OpenSubKey(RegistryKey rkey, string keyName, bool writable)
		{
			int num = 131097;
			if (writable)
			{
				num |= 0x20006;
			}
			IntPtr handle = GetHandle(rkey);
			IntPtr keyHandle;
			int num2 = RegOpenKeyEx(handle, keyName, IntPtr.Zero, num, out keyHandle);
			switch (num2)
			{
			case 2:
			case 1018:
				return null;
			default:
				GenerateException(num2);
				break;
			case 0:
				break;
			}
			return new RegistryKey(keyHandle, CombineName(rkey, keyName), writable);
		}

		public void Flush(RegistryKey rkey)
		{
			if (IsHandleValid(rkey))
			{
				IntPtr handle = GetHandle(rkey);
				RegFlushKey(handle);
			}
		}

		public void Close(RegistryKey rkey)
		{
			if (IsHandleValid(rkey))
			{
				IntPtr handle = GetHandle(rkey);
				RegCloseKey(handle);
			}
		}

		public RegistryKey CreateSubKey(RegistryKey rkey, string keyName)
		{
			IntPtr handle = GetHandle(rkey);
			IntPtr keyHandle;
			int num = RegCreateKey(handle, keyName, out keyHandle);
			switch (num)
			{
			case 1018:
				throw RegistryKey.CreateMarkedForDeletionException();
			default:
				GenerateException(num);
				break;
			case 0:
				break;
			}
			return new RegistryKey(keyHandle, CombineName(rkey, keyName), true);
		}

		public void DeleteKey(RegistryKey rkey, string keyName, bool shouldThrowWhenKeyMissing)
		{
			IntPtr handle = GetHandle(rkey);
			int num = RegDeleteKey(handle, keyName);
			switch (num)
			{
			case 2:
				if (shouldThrowWhenKeyMissing)
				{
					throw new ArgumentException("key " + keyName);
				}
				break;
			default:
				GenerateException(num);
				break;
			case 0:
				break;
			}
		}

		public void DeleteValue(RegistryKey rkey, string value, bool shouldThrowWhenKeyMissing)
		{
			IntPtr handle = GetHandle(rkey);
			int num = RegDeleteValue(handle, value);
			switch (num)
			{
			case 0:
			case 1018:
				break;
			case 2:
				if (shouldThrowWhenKeyMissing)
				{
					throw new ArgumentException("value " + value);
				}
				break;
			default:
				GenerateException(num);
				break;
			}
		}

		public string[] GetSubKeyNames(RegistryKey rkey)
		{
			IntPtr handle = GetHandle(rkey);
			StringBuilder stringBuilder = new StringBuilder(1024);
			ArrayList arrayList = new ArrayList();
			int num = 0;
			while (true)
			{
				int num2 = RegEnumKey(handle, num, stringBuilder, stringBuilder.Capacity);
				switch (num2)
				{
				case 0:
					arrayList.Add(stringBuilder.ToString());
					stringBuilder.Length = 0;
					break;
				default:
					GenerateException(num2);
					break;
				case 259:
					return (string[])arrayList.ToArray(typeof(string));
				}
				num++;
			}
		}

		public string[] GetValueNames(RegistryKey rkey)
		{
			IntPtr handle = GetHandle(rkey);
			ArrayList arrayList = new ArrayList();
			int num = 0;
			while (true)
			{
				StringBuilder stringBuilder = new StringBuilder(1024);
				int nameLength = stringBuilder.Capacity;
				RegistryValueKind type = RegistryValueKind.Unknown;
				int num2 = RegEnumValue(handle, num, stringBuilder, ref nameLength, IntPtr.Zero, ref type, IntPtr.Zero, IntPtr.Zero);
				switch (num2)
				{
				case 0:
				case 234:
					arrayList.Add(stringBuilder.ToString());
					break;
				case 1018:
					throw RegistryKey.CreateMarkedForDeletionException();
				default:
					GenerateException(num2);
					break;
				case 259:
					return (string[])arrayList.ToArray(typeof(string));
				}
				num++;
			}
		}

		private void GenerateException(int errorCode)
		{
			switch (errorCode)
			{
			case 2:
			case 87:
				throw new ArgumentException();
			case 5:
				throw new SecurityException();
			case 53:
				throw new IOException("The network path was not found.");
			default:
				throw new SystemException();
			}
		}

		public string ToString(RegistryKey rkey)
		{
			return rkey.Name;
		}

		internal static string CombineName(RegistryKey rkey, string localName)
		{
			return rkey.Name + "\\" + localName;
		}
	}
}
