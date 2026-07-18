using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	[ComVisible(true)]
	public sealed class ReflectionPermissionAttribute : CodeAccessSecurityAttribute
	{
		private ReflectionPermissionFlag flags;

		private bool memberAccess;

		private bool reflectionEmit;

		private bool typeInfo;

		public ReflectionPermissionFlag Flags
		{
			get
			{
				return flags;
			}
			set
			{
				flags = value;
				memberAccess = (flags & ReflectionPermissionFlag.MemberAccess) == ReflectionPermissionFlag.MemberAccess;
				reflectionEmit = (flags & ReflectionPermissionFlag.ReflectionEmit) == ReflectionPermissionFlag.ReflectionEmit;
				typeInfo = (flags & ReflectionPermissionFlag.TypeInformation) == ReflectionPermissionFlag.TypeInformation;
			}
		}

		public bool MemberAccess
		{
			get
			{
				return memberAccess;
			}
			set
			{
				if (value)
				{
					flags |= ReflectionPermissionFlag.MemberAccess;
				}
				else
				{
					flags -= 2;
				}
				memberAccess = value;
			}
		}

		public bool ReflectionEmit
		{
			get
			{
				return reflectionEmit;
			}
			set
			{
				if (value)
				{
					flags |= ReflectionPermissionFlag.ReflectionEmit;
				}
				else
				{
					flags -= 4;
				}
				reflectionEmit = value;
			}
		}

		public bool RestrictedMemberAccess
		{
			get
			{
				return (flags & ReflectionPermissionFlag.RestrictedMemberAccess) == ReflectionPermissionFlag.RestrictedMemberAccess;
			}
			set
			{
				if (value)
				{
					flags |= ReflectionPermissionFlag.RestrictedMemberAccess;
				}
				else
				{
					flags -= 8;
				}
			}
		}

		[Obsolete("not enforced in 2.0+")]
		public bool TypeInformation
		{
			get
			{
				return typeInfo;
			}
			set
			{
				if (value)
				{
					flags |= ReflectionPermissionFlag.TypeInformation;
				}
				else
				{
					flags--;
				}
				typeInfo = value;
			}
		}

		public ReflectionPermissionAttribute(SecurityAction action)
			: base(action)
		{
		}

		public override IPermission CreatePermission()
		{
			return null;
		}
	}
}
