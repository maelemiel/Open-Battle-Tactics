using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Serializable]
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public sealed class ZoneIdentityPermissionAttribute : CodeAccessSecurityAttribute
	{
		private SecurityZone zone;

		public SecurityZone Zone
		{
			get
			{
				return zone;
			}
			set
			{
				zone = value;
			}
		}

		public ZoneIdentityPermissionAttribute(SecurityAction action)
			: base(action)
		{
			zone = SecurityZone.NoZone;
		}

		public override IPermission CreatePermission()
		{
			if (base.Unrestricted)
			{
				return new ZoneIdentityPermission(PermissionState.Unrestricted);
			}
			return new ZoneIdentityPermission(zone);
		}
	}
}
