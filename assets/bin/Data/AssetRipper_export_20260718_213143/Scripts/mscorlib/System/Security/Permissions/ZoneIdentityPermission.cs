using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Serializable]
	[ComVisible(true)]
	public sealed class ZoneIdentityPermission : CodeAccessPermission, IBuiltInPermission
	{
		private const int version = 1;

		private SecurityZone zone;

		public SecurityZone SecurityZone
		{
			get
			{
				return zone;
			}
			set
			{
				if (!Enum.IsDefined(typeof(SecurityZone), value))
				{
					string message = string.Format(Locale.GetText("Invalid enum {0}"), value);
					throw new ArgumentException(message, "SecurityZone");
				}
				zone = value;
			}
		}

		public ZoneIdentityPermission(PermissionState state)
		{
			CodeAccessPermission.CheckPermissionState(state, false);
			zone = SecurityZone.NoZone;
		}

		public ZoneIdentityPermission(SecurityZone zone)
		{
			SecurityZone = zone;
		}

		int IBuiltInPermission.GetTokenIndex()
		{
			return 14;
		}

		public override IPermission Copy()
		{
			return new ZoneIdentityPermission(zone);
		}

		public override bool IsSubsetOf(IPermission target)
		{
			ZoneIdentityPermission zoneIdentityPermission = Cast(target);
			if (zoneIdentityPermission == null)
			{
				return zone == SecurityZone.NoZone;
			}
			return zone == SecurityZone.NoZone || zone == zoneIdentityPermission.zone;
		}

		public override IPermission Union(IPermission target)
		{
			ZoneIdentityPermission zoneIdentityPermission = Cast(target);
			if (zoneIdentityPermission == null)
			{
				IPermission result;
				if (zone == SecurityZone.NoZone)
				{
					IPermission permission = null;
					result = permission;
				}
				else
				{
					result = Copy();
				}
				return result;
			}
			if (zone == zoneIdentityPermission.zone || zoneIdentityPermission.zone == SecurityZone.NoZone)
			{
				return Copy();
			}
			if (zone == SecurityZone.NoZone)
			{
				return zoneIdentityPermission.Copy();
			}
			throw new ArgumentException(Locale.GetText("Union impossible"));
		}

		public override IPermission Intersect(IPermission target)
		{
			ZoneIdentityPermission zoneIdentityPermission = Cast(target);
			if (zoneIdentityPermission == null || zone == SecurityZone.NoZone)
			{
				return null;
			}
			if (zone == zoneIdentityPermission.zone)
			{
				return Copy();
			}
			return null;
		}

		public override void FromXml(SecurityElement esd)
		{
			CodeAccessPermission.CheckSecurityElement(esd, "esd", 1, 1);
			string text = esd.Attribute("Zone");
			if (text == null)
			{
				zone = SecurityZone.NoZone;
			}
			else
			{
				zone = (SecurityZone)(int)Enum.Parse(typeof(SecurityZone), text);
			}
		}

		public override SecurityElement ToXml()
		{
			SecurityElement securityElement = Element(1);
			if (zone != SecurityZone.NoZone)
			{
				securityElement.AddAttribute("Zone", zone.ToString());
			}
			return securityElement;
		}

		private ZoneIdentityPermission Cast(IPermission target)
		{
			if (target == null)
			{
				return null;
			}
			ZoneIdentityPermission zoneIdentityPermission = target as ZoneIdentityPermission;
			if (zoneIdentityPermission == null)
			{
				CodeAccessPermission.ThrowInvalidPermission(target, typeof(ZoneIdentityPermission));
			}
			return zoneIdentityPermission;
		}
	}
}
