using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace System.Security.Permissions
{
	[Serializable]
	[ComVisible(true)]
	public sealed class KeyContainerPermissionAccessEntry
	{
		private KeyContainerPermissionFlags _flags;

		private string _containerName;

		private int _spec;

		private string _store;

		private string _providerName;

		private int _type;

		public KeyContainerPermissionFlags Flags
		{
			get
			{
				return _flags;
			}
			set
			{
				if ((value & KeyContainerPermissionFlags.AllFlags) != KeyContainerPermissionFlags.NoFlags)
				{
					string message = string.Format(Locale.GetText("Invalid enum {0}"), value);
					throw new ArgumentException(message, "KeyContainerPermissionFlags");
				}
				_flags = value;
			}
		}

		public string KeyContainerName
		{
			get
			{
				return _containerName;
			}
			set
			{
				_containerName = value;
			}
		}

		public int KeySpec
		{
			get
			{
				return _spec;
			}
			set
			{
				_spec = value;
			}
		}

		public string KeyStore
		{
			get
			{
				return _store;
			}
			set
			{
				_store = value;
			}
		}

		public string ProviderName
		{
			get
			{
				return _providerName;
			}
			set
			{
				_providerName = value;
			}
		}

		public int ProviderType
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}

		public KeyContainerPermissionAccessEntry(CspParameters parameters, KeyContainerPermissionFlags flags)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException("parameters");
			}
			ProviderName = parameters.ProviderName;
			ProviderType = parameters.ProviderType;
			KeyContainerName = parameters.KeyContainerName;
			KeySpec = parameters.KeyNumber;
			Flags = flags;
		}

		public KeyContainerPermissionAccessEntry(string keyContainerName, KeyContainerPermissionFlags flags)
		{
			KeyContainerName = keyContainerName;
			Flags = flags;
		}

		public KeyContainerPermissionAccessEntry(string keyStore, string providerName, int providerType, string keyContainerName, int keySpec, KeyContainerPermissionFlags flags)
		{
			KeyStore = keyStore;
			ProviderName = providerName;
			ProviderType = providerType;
			KeyContainerName = keyContainerName;
			KeySpec = keySpec;
			Flags = flags;
		}

		public override bool Equals(object o)
		{
			if (o == null)
			{
				return false;
			}
			KeyContainerPermissionAccessEntry keyContainerPermissionAccessEntry = o as KeyContainerPermissionAccessEntry;
			if (keyContainerPermissionAccessEntry == null)
			{
				return false;
			}
			if (_flags != keyContainerPermissionAccessEntry._flags)
			{
				return false;
			}
			if (_containerName != keyContainerPermissionAccessEntry._containerName)
			{
				return false;
			}
			if (_store != keyContainerPermissionAccessEntry._store)
			{
				return false;
			}
			if (_providerName != keyContainerPermissionAccessEntry._providerName)
			{
				return false;
			}
			if (_type != keyContainerPermissionAccessEntry._type)
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = _type ^ _spec ^ (int)_flags;
			if (_containerName != null)
			{
				num ^= _containerName.GetHashCode();
			}
			if (_store != null)
			{
				num ^= _store.GetHashCode();
			}
			if (_providerName != null)
			{
				num ^= _providerName.GetHashCode();
			}
			return num;
		}
	}
}
