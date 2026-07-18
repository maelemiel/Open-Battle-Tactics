using System.Collections;
using System.Runtime.InteropServices;

namespace System.Security.Policy
{
	[Serializable]
	[ComVisible(true)]
	public sealed class GacMembershipCondition : IConstantMembershipCondition, IMembershipCondition, ISecurityEncodable, ISecurityPolicyEncodable
	{
		private readonly int version = 1;

		public bool Check(Evidence evidence)
		{
			if (evidence == null)
			{
				return false;
			}
			IEnumerator hostEnumerator = evidence.GetHostEnumerator();
			while (hostEnumerator.MoveNext())
			{
				if (hostEnumerator.Current is GacInstalled)
				{
					return true;
				}
			}
			return false;
		}

		public IMembershipCondition Copy()
		{
			return new GacMembershipCondition();
		}

		public override bool Equals(object o)
		{
			if (o == null)
			{
				return false;
			}
			return o is GacMembershipCondition;
		}

		public void FromXml(SecurityElement e)
		{
			FromXml(e, null);
		}

		public void FromXml(SecurityElement e, PolicyLevel level)
		{
			MembershipConditionHelper.CheckSecurityElement(e, "e", version, version);
		}

		public override int GetHashCode()
		{
			return 0;
		}

		public override string ToString()
		{
			return "GAC";
		}

		public SecurityElement ToXml()
		{
			return ToXml(null);
		}

		public SecurityElement ToXml(PolicyLevel level)
		{
			return MembershipConditionHelper.Element(typeof(GacMembershipCondition), version);
		}
	}
}
