using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Serializable]
	[ComVisible(true)]
	public sealed class FileDialogPermission : CodeAccessPermission, IBuiltInPermission, IUnrestrictedPermission
	{
		private const int version = 1;

		private FileDialogPermissionAccess _access;

		public FileDialogPermissionAccess Access
		{
			get
			{
				return _access;
			}
			set
			{
				if (!Enum.IsDefined(typeof(FileDialogPermissionAccess), value))
				{
					string message = string.Format(Locale.GetText("Invalid enum {0}"), value);
					throw new ArgumentException(message, "FileDialogPermissionAccess");
				}
				_access = value;
			}
		}

		public FileDialogPermission(PermissionState state)
		{
			if (CodeAccessPermission.CheckPermissionState(state, true) == PermissionState.Unrestricted)
			{
				_access = FileDialogPermissionAccess.OpenSave;
			}
			else
			{
				_access = FileDialogPermissionAccess.None;
			}
		}

		public FileDialogPermission(FileDialogPermissionAccess access)
		{
			Access = access;
		}

		int IBuiltInPermission.GetTokenIndex()
		{
			return 1;
		}

		public override IPermission Copy()
		{
			return new FileDialogPermission(_access);
		}

		public override void FromXml(SecurityElement esd)
		{
			CodeAccessPermission.CheckSecurityElement(esd, "esd", 1, 1);
			if (CodeAccessPermission.IsUnrestricted(esd))
			{
				_access = FileDialogPermissionAccess.OpenSave;
				return;
			}
			string text = esd.Attribute("Access");
			if (text == null)
			{
				_access = FileDialogPermissionAccess.None;
			}
			else
			{
				_access = (FileDialogPermissionAccess)(int)Enum.Parse(typeof(FileDialogPermissionAccess), text);
			}
		}

		public override IPermission Intersect(IPermission target)
		{
			FileDialogPermission fileDialogPermission = Cast(target);
			if (fileDialogPermission == null)
			{
				return null;
			}
			FileDialogPermissionAccess fileDialogPermissionAccess = _access & fileDialogPermission._access;
			return (fileDialogPermissionAccess != FileDialogPermissionAccess.None) ? new FileDialogPermission(fileDialogPermissionAccess) : null;
		}

		public override bool IsSubsetOf(IPermission target)
		{
			FileDialogPermission fileDialogPermission = Cast(target);
			if (fileDialogPermission == null)
			{
				return false;
			}
			return (_access & fileDialogPermission._access) == _access;
		}

		public bool IsUnrestricted()
		{
			return _access == FileDialogPermissionAccess.OpenSave;
		}

		public override SecurityElement ToXml()
		{
			SecurityElement securityElement = Element(1);
			switch (_access)
			{
			case FileDialogPermissionAccess.Open:
				securityElement.AddAttribute("Access", "Open");
				break;
			case FileDialogPermissionAccess.Save:
				securityElement.AddAttribute("Access", "Save");
				break;
			case FileDialogPermissionAccess.OpenSave:
				securityElement.AddAttribute("Unrestricted", "true");
				break;
			}
			return securityElement;
		}

		public override IPermission Union(IPermission target)
		{
			FileDialogPermission fileDialogPermission = Cast(target);
			if (fileDialogPermission == null)
			{
				return Copy();
			}
			if (IsUnrestricted() || fileDialogPermission.IsUnrestricted())
			{
				return new FileDialogPermission(PermissionState.Unrestricted);
			}
			return new FileDialogPermission(_access | fileDialogPermission._access);
		}

		private FileDialogPermission Cast(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			FileDialogPermission fileDialogPermission = target as FileDialogPermission;
			if (fileDialogPermission == null)
			{
				CodeAccessPermission.ThrowInvalidPermission(target, typeof(FileDialogPermission));
			}
			return fileDialogPermission;
		}
	}
}
