using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Serializable]
	[ComVisible(true)]
	public sealed class ReflectionPermission : CodeAccessPermission, IBuiltInPermission, IUnrestrictedPermission
	{
		private const int version = 1;

		private ReflectionPermissionFlag flags;

		public ReflectionPermissionFlag Flags
		{
			get
			{
				return flags;
			}
			set
			{
				if ((value & (ReflectionPermissionFlag.AllFlags | ReflectionPermissionFlag.RestrictedMemberAccess)) != value)
				{
					string message = string.Format(Locale.GetText("Invalid flags {0}"), value);
					throw new ArgumentException(message, "ReflectionPermissionFlag");
				}
				flags = value;
			}
		}

		public ReflectionPermission(PermissionState state)
		{
			if (CodeAccessPermission.CheckPermissionState(state, true) == PermissionState.Unrestricted)
			{
				flags = ReflectionPermissionFlag.AllFlags;
			}
			else
			{
				flags = ReflectionPermissionFlag.NoFlags;
			}
		}

		public ReflectionPermission(ReflectionPermissionFlag flag)
		{
			Flags = flag;
		}

		int IBuiltInPermission.GetTokenIndex()
		{
			return 4;
		}

		public override IPermission Copy()
		{
			return new ReflectionPermission(flags);
		}

		public override void FromXml(SecurityElement esd)
		{
			CodeAccessPermission.CheckSecurityElement(esd, "esd", 1, 1);
			if (CodeAccessPermission.IsUnrestricted(esd))
			{
				flags = ReflectionPermissionFlag.AllFlags;
				return;
			}
			flags = ReflectionPermissionFlag.NoFlags;
			string text = esd.Attributes["Flags"] as string;
			if (text.IndexOf("MemberAccess") >= 0)
			{
				flags |= ReflectionPermissionFlag.MemberAccess;
			}
			if (text.IndexOf("ReflectionEmit") >= 0)
			{
				flags |= ReflectionPermissionFlag.ReflectionEmit;
			}
			if (text.IndexOf("TypeInformation") >= 0)
			{
				flags |= ReflectionPermissionFlag.TypeInformation;
			}
		}

		public override IPermission Intersect(IPermission target)
		{
			ReflectionPermission reflectionPermission = Cast(target);
			if (reflectionPermission == null)
			{
				return null;
			}
			if (IsUnrestricted())
			{
				if (reflectionPermission.Flags == ReflectionPermissionFlag.NoFlags)
				{
					return null;
				}
				return reflectionPermission.Copy();
			}
			if (reflectionPermission.IsUnrestricted())
			{
				if (flags == ReflectionPermissionFlag.NoFlags)
				{
					return null;
				}
				return Copy();
			}
			ReflectionPermission reflectionPermission2 = (ReflectionPermission)reflectionPermission.Copy();
			reflectionPermission2.Flags &= flags;
			return (reflectionPermission2.Flags != ReflectionPermissionFlag.NoFlags) ? reflectionPermission2 : null;
		}

		public override bool IsSubsetOf(IPermission target)
		{
			ReflectionPermission reflectionPermission = Cast(target);
			if (reflectionPermission == null)
			{
				return flags == ReflectionPermissionFlag.NoFlags;
			}
			if (IsUnrestricted())
			{
				return reflectionPermission.IsUnrestricted();
			}
			if (reflectionPermission.IsUnrestricted())
			{
				return true;
			}
			return (flags & reflectionPermission.Flags) == flags;
		}

		public bool IsUnrestricted()
		{
			return flags == ReflectionPermissionFlag.AllFlags;
		}

		public override SecurityElement ToXml()
		{
			SecurityElement securityElement = Element(1);
			if (IsUnrestricted())
			{
				securityElement.AddAttribute("Unrestricted", "true");
			}
			else if (flags == ReflectionPermissionFlag.NoFlags)
			{
				securityElement.AddAttribute("Flags", "NoFlags");
			}
			else if ((flags & ReflectionPermissionFlag.AllFlags) == ReflectionPermissionFlag.AllFlags)
			{
				securityElement.AddAttribute("Flags", "AllFlags");
			}
			else
			{
				string text = string.Empty;
				if ((flags & ReflectionPermissionFlag.MemberAccess) == ReflectionPermissionFlag.MemberAccess)
				{
					text = "MemberAccess";
				}
				if ((flags & ReflectionPermissionFlag.ReflectionEmit) == ReflectionPermissionFlag.ReflectionEmit)
				{
					if (text.Length > 0)
					{
						text += ", ";
					}
					text += "ReflectionEmit";
				}
				if ((flags & ReflectionPermissionFlag.TypeInformation) == ReflectionPermissionFlag.TypeInformation)
				{
					if (text.Length > 0)
					{
						text += ", ";
					}
					text += "TypeInformation";
				}
				securityElement.AddAttribute("Flags", text);
			}
			return securityElement;
		}

		public override IPermission Union(IPermission other)
		{
			ReflectionPermission reflectionPermission = Cast(other);
			if (other == null)
			{
				return Copy();
			}
			if (IsUnrestricted() || reflectionPermission.IsUnrestricted())
			{
				return new ReflectionPermission(PermissionState.Unrestricted);
			}
			ReflectionPermission reflectionPermission2 = (ReflectionPermission)reflectionPermission.Copy();
			reflectionPermission2.Flags |= flags;
			return reflectionPermission2;
		}

		private ReflectionPermission Cast(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			ReflectionPermission reflectionPermission = target as ReflectionPermission;
			if (reflectionPermission == null)
			{
				CodeAccessPermission.ThrowInvalidPermission(target, typeof(ReflectionPermission));
			}
			return reflectionPermission;
		}
	}
}
