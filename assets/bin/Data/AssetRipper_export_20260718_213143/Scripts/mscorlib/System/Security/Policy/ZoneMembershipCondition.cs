using System.Collections;
using System.Runtime.InteropServices;

namespace System.Security.Policy
{
	[Serializable]
	[ComVisible(true)]
	public sealed class ZoneMembershipCondition : IConstantMembershipCondition, IMembershipCondition, ISecurityEncodable, ISecurityPolicyEncodable
	{
		private readonly int version = 1;

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
					throw new ArgumentException(Locale.GetText("invalid zone"));
				}
				if (value == SecurityZone.NoZone)
				{
					throw new ArgumentException(Locale.GetText("NoZone isn't valid for membership condition"));
				}
				zone = value;
			}
		}

		internal ZoneMembershipCondition()
		{
		}

		public ZoneMembershipCondition(SecurityZone zone)
		{
			SecurityZone = zone;
		}

		public bool Check(Evidence evidence)
		{
			if (evidence == null)
			{
				return false;
			}
			IEnumerator hostEnumerator = evidence.GetHostEnumerator();
			while (hostEnumerator.MoveNext())
			{
				Zone zone = hostEnumerator.Current as Zone;
				if (zone != null && zone.SecurityZone == this.zone)
				{
					return true;
				}
			}
			return false;
		}

		public IMembershipCondition Copy()
		{
			return new ZoneMembershipCondition(zone);
		}

		public override bool Equals(object o)
		{
			ZoneMembershipCondition zoneMembershipCondition = o as ZoneMembershipCondition;
			if (zoneMembershipCondition == null)
			{
				return false;
			}
			return zoneMembershipCondition.SecurityZone == zone;
		}

		public void FromXml(SecurityElement e)
		{
			FromXml(e, null);
		}

		public void FromXml(SecurityElement e, PolicyLevel level)
		{
			MembershipConditionHelper.CheckSecurityElement(e, "e", version, version);
			string text = e.Attribute("Zone");
			if (text != null)
			{
				zone = (SecurityZone)(int)Enum.Parse(typeof(SecurityZone), text);
			}
		}

		public override int GetHashCode()
		{
			return zone.GetHashCode();
		}

		public override string ToString()
		{
			return "Zone - " + zone;
		}

		public SecurityElement ToXml()
		{
			return ToXml(null);
		}

		public SecurityElement ToXml(PolicyLevel level)
		{
			SecurityElement securityElement = MembershipConditionHelper.Element(typeof(ZoneMembershipCondition), version);
			securityElement.AddAttribute("Zone", zone.ToString());
			return securityElement;
		}
	}
}
