using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	[ComVisible(true)]
	public sealed class FileIOPermissionAttribute : CodeAccessSecurityAttribute
	{
		private string append;

		private string path;

		private string read;

		private string write;

		private FileIOPermissionAccess allFiles;

		private FileIOPermissionAccess allLocalFiles;

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
				append = value;
				path = value;
				read = value;
				write = value;
			}
		}

		public string Append
		{
			get
			{
				return append;
			}
			set
			{
				append = value;
			}
		}

		public string PathDiscovery
		{
			get
			{
				return path;
			}
			set
			{
				path = value;
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

		public FileIOPermissionAccess AllFiles
		{
			get
			{
				return allFiles;
			}
			set
			{
				allFiles = value;
			}
		}

		public FileIOPermissionAccess AllLocalFiles
		{
			get
			{
				return allLocalFiles;
			}
			set
			{
				allLocalFiles = value;
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
				append = value;
				path = value;
				read = value;
				write = value;
			}
		}

		public FileIOPermissionAttribute(SecurityAction action)
			: base(action)
		{
		}

		public override IPermission CreatePermission()
		{
			return null;
		}
	}
}
