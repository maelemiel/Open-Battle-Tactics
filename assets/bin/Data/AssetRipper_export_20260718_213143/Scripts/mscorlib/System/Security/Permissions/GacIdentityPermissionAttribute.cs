using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	[ComVisible(true)]
	public sealed class GacIdentityPermissionAttribute : CodeAccessSecurityAttribute
	{
		public GacIdentityPermissionAttribute(SecurityAction action)
			: base(action)
		{
		}

		public override IPermission CreatePermission()
		{
			return new GacIdentityPermission();
		}
	}
}
