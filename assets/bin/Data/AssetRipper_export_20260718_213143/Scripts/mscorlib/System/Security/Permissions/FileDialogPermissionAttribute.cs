using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Serializable]
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public sealed class FileDialogPermissionAttribute : CodeAccessSecurityAttribute
	{
		private bool canOpen;

		private bool canSave;

		public bool Open
		{
			get
			{
				return canOpen;
			}
			set
			{
				canOpen = value;
			}
		}

		public bool Save
		{
			get
			{
				return canSave;
			}
			set
			{
				canSave = value;
			}
		}

		public FileDialogPermissionAttribute(SecurityAction action)
			: base(action)
		{
		}

		public override IPermission CreatePermission()
		{
			FileDialogPermission fileDialogPermission = null;
			if (base.Unrestricted)
			{
				return new FileDialogPermission(PermissionState.Unrestricted);
			}
			FileDialogPermissionAccess fileDialogPermissionAccess = FileDialogPermissionAccess.None;
			if (canOpen)
			{
				fileDialogPermissionAccess |= FileDialogPermissionAccess.Open;
			}
			if (canSave)
			{
				fileDialogPermissionAccess |= FileDialogPermissionAccess.Save;
			}
			return new FileDialogPermission(fileDialogPermissionAccess);
		}
	}
}
