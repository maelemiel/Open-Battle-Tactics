using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Serializable]
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public sealed class UrlIdentityPermissionAttribute : CodeAccessSecurityAttribute
	{
		private string url;

		public string Url
		{
			get
			{
				return url;
			}
			set
			{
				url = value;
			}
		}

		public UrlIdentityPermissionAttribute(SecurityAction action)
			: base(action)
		{
		}

		public override IPermission CreatePermission()
		{
			if (base.Unrestricted)
			{
				return new UrlIdentityPermission(PermissionState.Unrestricted);
			}
			if (url == null)
			{
				return new UrlIdentityPermission(PermissionState.None);
			}
			return new UrlIdentityPermission(url);
		}
	}
}
