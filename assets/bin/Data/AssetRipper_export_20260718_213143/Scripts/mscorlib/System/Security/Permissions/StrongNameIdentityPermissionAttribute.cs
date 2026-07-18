using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Serializable]
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public sealed class StrongNameIdentityPermissionAttribute : CodeAccessSecurityAttribute
	{
		private string name;

		private string key;

		private string version;

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		public string PublicKey
		{
			get
			{
				return key;
			}
			set
			{
				key = value;
			}
		}

		public string Version
		{
			get
			{
				return version;
			}
			set
			{
				version = value;
			}
		}

		public StrongNameIdentityPermissionAttribute(SecurityAction action)
			: base(action)
		{
		}

		public override IPermission CreatePermission()
		{
			return null;
		}
	}
}
