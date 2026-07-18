using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Security.Policy
{
	[Serializable]
	[ComVisible(true)]
	public sealed class SiteMembershipCondition : IConstantMembershipCondition, IMembershipCondition, ISecurityEncodable, ISecurityPolicyEncodable
	{
		private readonly int version = 1;

		private string _site;

		public string Site
		{
			get
			{
				return _site;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("site");
				}
				if (!System.Security.Policy.Site.IsValid(value))
				{
					throw new ArgumentException("invalid site");
				}
				_site = value;
			}
		}

		internal SiteMembershipCondition()
		{
		}

		public SiteMembershipCondition(string site)
		{
			Site = site;
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
				if (!(hostEnumerator.Current is Site))
				{
					continue;
				}
				string[] array = _site.Split('.');
				string[] array2 = (hostEnumerator.Current as Site).origin_site.Split('.');
				int num = array.Length - 1;
				int num2 = array2.Length - 1;
				while (num >= 0)
				{
					if (num == 0)
					{
						return string.Compare(array[0], "*", true, CultureInfo.InvariantCulture) == 0;
					}
					if (string.Compare(array[num], array2[num2], true, CultureInfo.InvariantCulture) != 0)
					{
						return false;
					}
					num--;
					num2--;
				}
				return true;
			}
			return false;
		}

		public IMembershipCondition Copy()
		{
			return new SiteMembershipCondition(_site);
		}

		public override bool Equals(object o)
		{
			if (o == null)
			{
				return false;
			}
			if (o is SiteMembershipCondition)
			{
				Site site = new Site((o as SiteMembershipCondition)._site);
				return site.Equals(new Site(_site));
			}
			return false;
		}

		public void FromXml(SecurityElement e)
		{
			FromXml(e, null);
		}

		public void FromXml(SecurityElement e, PolicyLevel level)
		{
			MembershipConditionHelper.CheckSecurityElement(e, "e", version, version);
			_site = e.Attribute("Site");
		}

		public override int GetHashCode()
		{
			return _site.GetHashCode();
		}

		public override string ToString()
		{
			return "Site - " + _site;
		}

		public SecurityElement ToXml()
		{
			return ToXml(null);
		}

		public SecurityElement ToXml(PolicyLevel level)
		{
			SecurityElement securityElement = MembershipConditionHelper.Element(typeof(SiteMembershipCondition), version);
			securityElement.AddAttribute("Site", _site);
			return securityElement;
		}
	}
}
