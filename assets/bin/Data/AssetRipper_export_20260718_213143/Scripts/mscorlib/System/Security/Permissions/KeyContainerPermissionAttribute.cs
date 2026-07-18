using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Serializable]
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public sealed class KeyContainerPermissionAttribute : CodeAccessSecurityAttribute
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

		public KeyContainerPermissionAttribute(SecurityAction action)
			: base(action)
		{
			_spec = -1;
			_type = -1;
		}

		public override IPermission CreatePermission()
		{
			if (base.Unrestricted)
			{
				return new KeyContainerPermission(PermissionState.Unrestricted);
			}
			if (EmptyEntry())
			{
				return new KeyContainerPermission(_flags);
			}
			return new KeyContainerPermission(accessList: new KeyContainerPermissionAccessEntry[1]
			{
				new KeyContainerPermissionAccessEntry(_store, _providerName, _type, _containerName, _spec, _flags)
			}, flags: _flags);
		}

		private bool EmptyEntry()
		{
			if (_containerName != null)
			{
				return false;
			}
			if (_spec != 0)
			{
				return false;
			}
			if (_store != null)
			{
				return false;
			}
			if (_providerName != null)
			{
				return false;
			}
			if (_type != 0)
			{
				return false;
			}
			return true;
		}
	}
}
