using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Serializable]
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public sealed class UIPermissionAttribute : CodeAccessSecurityAttribute
	{
		private UIPermissionClipboard clipboard;

		private UIPermissionWindow window;

		public UIPermissionClipboard Clipboard
		{
			get
			{
				return clipboard;
			}
			set
			{
				clipboard = value;
			}
		}

		public UIPermissionWindow Window
		{
			get
			{
				return window;
			}
			set
			{
				window = value;
			}
		}

		public UIPermissionAttribute(SecurityAction action)
			: base(action)
		{
		}

		public override IPermission CreatePermission()
		{
			UIPermission uIPermission = null;
			if (base.Unrestricted)
			{
				return new UIPermission(PermissionState.Unrestricted);
			}
			return new UIPermission(window, clipboard);
		}
	}
}
