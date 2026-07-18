using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Serializable]
	[ComVisible(true)]
	public sealed class GacIdentityPermission : CodeAccessPermission, IBuiltInPermission
	{
		private const int version = 1;

		public GacIdentityPermission()
		{
		}

		public GacIdentityPermission(PermissionState state)
		{
			CodeAccessPermission.CheckPermissionState(state, false);
		}

		int IBuiltInPermission.GetTokenIndex()
		{
			return 15;
		}

		public override IPermission Copy()
		{
			return new GacIdentityPermission();
		}

		public override IPermission Intersect(IPermission target)
		{
			GacIdentityPermission gacIdentityPermission = Cast(target);
			if (gacIdentityPermission == null)
			{
				return null;
			}
			return Copy();
		}

		public override bool IsSubsetOf(IPermission target)
		{
			GacIdentityPermission gacIdentityPermission = Cast(target);
			return gacIdentityPermission != null;
		}

		public override IPermission Union(IPermission target)
		{
			Cast(target);
			return Copy();
		}

		public override void FromXml(SecurityElement securityElement)
		{
			CodeAccessPermission.CheckSecurityElement(securityElement, "securityElement", 1, 1);
		}

		public override SecurityElement ToXml()
		{
			return Element(1);
		}

		private GacIdentityPermission Cast(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			GacIdentityPermission gacIdentityPermission = target as GacIdentityPermission;
			if (gacIdentityPermission == null)
			{
				CodeAccessPermission.ThrowInvalidPermission(target, typeof(GacIdentityPermission));
			}
			return gacIdentityPermission;
		}
	}
}
