using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Serializable]
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public sealed class RegistryPermissionAttribute : CodeAccessSecurityAttribute
	{
		private string create;

		private string read;

		private string write;

		private string changeAccessControl;

		private string viewAccessControl;

		[Obsolete("use newer properties")]
		public string All
		{
			get
			{
				throw new NotSupportedException("All");
			}
			set
			{
				create = value;
				read = value;
				write = value;
			}
		}

		public string Create
		{
			get
			{
				return create;
			}
			set
			{
				create = value;
			}
		}

		public string Read
		{
			get
			{
				return read;
			}
			set
			{
				read = value;
			}
		}

		public string Write
		{
			get
			{
				return write;
			}
			set
			{
				write = value;
			}
		}

		public string ChangeAccessControl
		{
			get
			{
				return changeAccessControl;
			}
			set
			{
				changeAccessControl = value;
			}
		}

		public string ViewAccessControl
		{
			get
			{
				return viewAccessControl;
			}
			set
			{
				viewAccessControl = value;
			}
		}

		public string ViewAndModify
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				create = value;
				read = value;
				write = value;
			}
		}

		public RegistryPermissionAttribute(SecurityAction action)
			: base(action)
		{
		}

		public override IPermission CreatePermission()
		{
			RegistryPermission registryPermission = null;
			if (base.Unrestricted)
			{
				registryPermission = new RegistryPermission(PermissionState.Unrestricted);
			}
			else
			{
				registryPermission = new RegistryPermission(PermissionState.None);
				if (create != null)
				{
					registryPermission.AddPathList(RegistryPermissionAccess.Create, create);
				}
				if (read != null)
				{
					registryPermission.AddPathList(RegistryPermissionAccess.Read, read);
				}
				if (write != null)
				{
					registryPermission.AddPathList(RegistryPermissionAccess.Write, write);
				}
			}
			return registryPermission;
		}
	}
}
