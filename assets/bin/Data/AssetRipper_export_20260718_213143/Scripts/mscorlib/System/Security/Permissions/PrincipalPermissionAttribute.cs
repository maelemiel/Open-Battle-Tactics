using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	[ComVisible(true)]
	public sealed class PrincipalPermissionAttribute : CodeAccessSecurityAttribute
	{
		private bool authenticated;

		private string name;

		private string role;

		public bool Authenticated
		{
			get
			{
				return authenticated;
			}
			set
			{
				authenticated = value;
			}
		}

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

		public string Role
		{
			get
			{
				return role;
			}
			set
			{
				role = value;
			}
		}

		public PrincipalPermissionAttribute(SecurityAction action)
			: base(action)
		{
			authenticated = true;
		}

		public override IPermission CreatePermission()
		{
			PrincipalPermission principalPermission = null;
			if (base.Unrestricted)
			{
				return new PrincipalPermission(PermissionState.Unrestricted);
			}
			return new PrincipalPermission(name, role, authenticated);
		}
	}
}
