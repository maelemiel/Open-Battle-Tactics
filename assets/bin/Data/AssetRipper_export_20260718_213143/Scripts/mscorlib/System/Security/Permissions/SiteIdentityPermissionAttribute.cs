using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	[ComVisible(true)]
	public sealed class SiteIdentityPermissionAttribute : CodeAccessSecurityAttribute
	{
		private string site;

		public string Site
		{
			get
			{
				return site;
			}
			set
			{
				site = value;
			}
		}

		public SiteIdentityPermissionAttribute(SecurityAction action)
			: base(action)
		{
		}

		public override IPermission CreatePermission()
		{
			SiteIdentityPermission siteIdentityPermission = null;
			if (base.Unrestricted)
			{
				return new SiteIdentityPermission(PermissionState.Unrestricted);
			}
			if (site == null)
			{
				return new SiteIdentityPermission(PermissionState.None);
			}
			return new SiteIdentityPermission(site);
		}
	}
}
