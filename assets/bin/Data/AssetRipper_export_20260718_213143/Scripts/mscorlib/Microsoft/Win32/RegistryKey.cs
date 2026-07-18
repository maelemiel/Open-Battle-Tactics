using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;

namespace Microsoft.Win32
{
	[ComVisible(true)]
	public sealed class RegistryKey : MarshalByRefObject, IDisposable
	{
		private object handle;

		private object hive;

		private readonly string qname;

		private readonly bool isRemoteRoot;

		private readonly bool isWritable;

		private static readonly IRegistryApi RegistryApi;

		public string Name
		{
			get
			{
				return qname;
			}
		}

		public int SubKeyCount
		{
			get
			{
				AssertKeyStillValid();
				return RegistryApi.SubKeyCount(this);
			}
		}

		public int ValueCount
		{
			get
			{
				AssertKeyStillValid();
				return RegistryApi.ValueCount(this);
			}
		}

		internal bool IsRoot
		{
			get
			{
				return hive != null;
			}
		}

		private bool IsWritable
		{
			get
			{
				return isWritable;
			}
		}

		internal RegistryHive Hive
		{
			get
			{
				if (!IsRoot)
				{
					throw new NotSupportedException();
				}
				return (RegistryHive)(int)hive;
			}
		}

		internal object Handle
		{
			get
			{
				return handle;
			}
		}

		internal RegistryKey(RegistryHive hiveId)
			: this(hiveId, new IntPtr((int)hiveId), false)
		{
		}

		internal RegistryKey(RegistryHive hiveId, IntPtr keyHandle, bool remoteRoot)
		{
			hive = hiveId;
			handle = keyHandle;
			qname = GetHiveName(hiveId);
			isRemoteRoot = remoteRoot;
			isWritable = true;
		}

		internal RegistryKey(object data, string keyName, bool writable)
		{
			handle = data;
			qname = keyName;
			isWritable = writable;
		}

		static RegistryKey()
		{
			if (Path.DirectorySeparatorChar == '\\')
			{
				RegistryApi = new Win32RegistryApi();
			}
			else
			{
				RegistryApi = new UnixRegistryApi();
			}
		}

		void IDisposable.Dispose()
		{
			GC.SuppressFinalize(this);
			Close();
		}

		~RegistryKey()
		{
			Close();
		}

		public void Flush()
		{
			RegistryApi.Flush(this);
		}

		public void Close()
		{
			Flush();
			if (isRemoteRoot || !IsRoot)
			{
				RegistryApi.Close(this);
				handle = null;
			}
		}

		public void SetValue(string name, object value)
		{
			AssertKeyStillValid();
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (name != null)
			{
				AssertKeyNameLength(name);
			}
			if (!IsWritable)
			{
				throw new UnauthorizedAccessException("Cannot write to the registry key.");
			}
			RegistryApi.SetValue(this, name, value);
		}

		[ComVisible(false)]
		public void SetValue(string name, object value, RegistryValueKind valueKind)
		{
			AssertKeyStillValid();
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (name != null)
			{
				AssertKeyNameLength(name);
			}
			if (!IsWritable)
			{
				throw new UnauthorizedAccessException("Cannot write to the registry key.");
			}
			RegistryApi.SetValue(this, name, value, valueKind);
		}

		public RegistryKey OpenSubKey(string name)
		{
			return OpenSubKey(name, false);
		}

		public RegistryKey OpenSubKey(string name, bool writable)
		{
			AssertKeyStillValid();
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			AssertKeyNameLength(name);
			return RegistryApi.OpenSubKey(this, name, writable);
		}

		public object GetValue(string name)
		{
			return GetValue(name, null);
		}

		public object GetValue(string name, object defaultValue)
		{
			AssertKeyStillValid();
			return RegistryApi.GetValue(this, name, defaultValue, RegistryValueOptions.None);
		}

		[ComVisible(false)]
		public object GetValue(string name, object defaultValue, RegistryValueOptions options)
		{
			AssertKeyStillValid();
			return RegistryApi.GetValue(this, name, defaultValue, options);
		}

		[ComVisible(false)]
		public RegistryValueKind GetValueKind(string name)
		{
			throw new NotImplementedException();
		}

		public RegistryKey CreateSubKey(string subkey)
		{
			AssertKeyStillValid();
			AssertKeyNameNotNull(subkey);
			AssertKeyNameLength(subkey);
			if (!IsWritable)
			{
				throw new UnauthorizedAccessException("Cannot write to the registry key.");
			}
			return RegistryApi.CreateSubKey(this, subkey);
		}

		[ComVisible(false)]
		public RegistryKey CreateSubKey(string subkey, RegistryKeyPermissionCheck permissionCheck)
		{
			throw new NotImplementedException();
		}

		[ComVisible(false)]
		public RegistryKey CreateSubKey(string subkey, RegistryKeyPermissionCheck permissionCheck, RegistrySecurity registrySecurity)
		{
			throw new NotImplementedException();
		}

		public void DeleteSubKey(string subkey)
		{
			DeleteSubKey(subkey, true);
		}

		public void DeleteSubKey(string subkey, bool throwOnMissingSubKey)
		{
			AssertKeyStillValid();
			AssertKeyNameNotNull(subkey);
			AssertKeyNameLength(subkey);
			if (!IsWritable)
			{
				throw new UnauthorizedAccessException("Cannot write to the registry key.");
			}
			RegistryKey registryKey = OpenSubKey(subkey);
			if (registryKey == null)
			{
				if (throwOnMissingSubKey)
				{
					throw new ArgumentException("Cannot delete a subkey tree because the subkey does not exist.");
				}
				return;
			}
			if (registryKey.SubKeyCount > 0)
			{
				throw new InvalidOperationException("Registry key has subkeys and recursive removes are not supported by this method.");
			}
			registryKey.Close();
			RegistryApi.DeleteKey(this, subkey, throwOnMissingSubKey);
		}

		public void DeleteSubKeyTree(string subkey)
		{
			AssertKeyStillValid();
			AssertKeyNameNotNull(subkey);
			AssertKeyNameLength(subkey);
			RegistryKey registryKey = OpenSubKey(subkey, true);
			if (registryKey == null)
			{
				throw new ArgumentException("Cannot delete a subkey tree because the subkey does not exist.");
			}
			registryKey.DeleteChildKeysAndValues();
			registryKey.Close();
			DeleteSubKey(subkey, false);
		}

		public void DeleteValue(string name)
		{
			DeleteValue(name, true);
		}

		public void DeleteValue(string name, bool throwOnMissingValue)
		{
			AssertKeyStillValid();
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (!IsWritable)
			{
				throw new UnauthorizedAccessException("Cannot write to the registry key.");
			}
			RegistryApi.DeleteValue(this, name, throwOnMissingValue);
		}

		public RegistrySecurity GetAccessControl()
		{
			throw new NotImplementedException();
		}

		public RegistrySecurity GetAccessControl(AccessControlSections includeSections)
		{
			throw new NotImplementedException();
		}

		public string[] GetSubKeyNames()
		{
			AssertKeyStillValid();
			return RegistryApi.GetSubKeyNames(this);
		}

		public string[] GetValueNames()
		{
			AssertKeyStillValid();
			return RegistryApi.GetValueNames(this);
		}

		[MonoTODO("Not implemented on unix")]
		public static RegistryKey OpenRemoteBaseKey(RegistryHive hKey, string machineName)
		{
			if (machineName == null)
			{
				throw new ArgumentNullException("machineName");
			}
			return RegistryApi.OpenRemoteBaseKey(hKey, machineName);
		}

		[ComVisible(false)]
		public RegistryKey OpenSubKey(string name, RegistryKeyPermissionCheck permissionCheck)
		{
			throw new NotImplementedException();
		}

		[ComVisible(false)]
		public RegistryKey OpenSubKey(string name, RegistryKeyPermissionCheck permissionCheck, RegistryRights rights)
		{
			throw new NotImplementedException();
		}

		public void SetAccessControl(RegistrySecurity registrySecurity)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			AssertKeyStillValid();
			return RegistryApi.ToString(this);
		}

		private void AssertKeyStillValid()
		{
			if (handle == null)
			{
				throw new ObjectDisposedException("Microsoft.Win32.RegistryKey");
			}
		}

		private void AssertKeyNameNotNull(string subKeyName)
		{
			if (subKeyName == null)
			{
				throw new ArgumentNullException("name");
			}
		}

		private void AssertKeyNameLength(string name)
		{
			if (name.Length > 255)
			{
				throw new ArgumentException("Name of registry key cannot be greater than 255 characters");
			}
		}

		private void DeleteChildKeysAndValues()
		{
			if (!IsRoot)
			{
				string[] subKeyNames = GetSubKeyNames();
				string[] array = subKeyNames;
				foreach (string text in array)
				{
					RegistryKey registryKey = OpenSubKey(text, true);
					registryKey.DeleteChildKeysAndValues();
					registryKey.Close();
					DeleteSubKey(text, false);
				}
				string[] valueNames = GetValueNames();
				string[] array2 = valueNames;
				foreach (string name in array2)
				{
					DeleteValue(name, false);
				}
			}
		}

		internal static string DecodeString(byte[] data)
		{
			string text = Encoding.Unicode.GetString(data);
			int num = text.IndexOf('\0');
			if (num != -1)
			{
				text = text.TrimEnd(default(char));
			}
			return text;
		}

		internal static IOException CreateMarkedForDeletionException()
		{
			throw new IOException("Illegal operation attempted on a registry key that has been marked for deletion.");
		}

		private static string GetHiveName(RegistryHive hive)
		{
			switch (hive)
			{
			case RegistryHive.ClassesRoot:
				return "HKEY_CLASSES_ROOT";
			case RegistryHive.CurrentConfig:
				return "HKEY_CURRENT_CONFIG";
			case RegistryHive.CurrentUser:
				return "HKEY_CURRENT_USER";
			case RegistryHive.DynData:
				return "HKEY_DYN_DATA";
			case RegistryHive.LocalMachine:
				return "HKEY_LOCAL_MACHINE";
			case RegistryHive.PerformanceData:
				return "HKEY_PERFORMANCE_DATA";
			case RegistryHive.Users:
				return "HKEY_USERS";
			default:
				throw new NotImplementedException(string.Format("Registry hive '{0}' is not implemented.", hive.ToString()));
			}
		}
	}
}
