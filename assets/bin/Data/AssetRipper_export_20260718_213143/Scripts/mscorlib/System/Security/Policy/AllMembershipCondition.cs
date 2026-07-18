using System.Runtime.InteropServices;

namespace System.Security.Policy
{
	[Serializable]
	[ComVisible(true)]
	public sealed class AllMembershipCondition : IConstantMembershipCondition, IMembershipCondition, ISecurityEncodable, ISecurityPolicyEncodable
	{
		private readonly int version = 1;

		public bool Check(Evidence evidence)
		{
			return true;
		}

		public IMembershipCondition Copy()
		{
			return new AllMembershipCondition();
		}

		public override bool Equals(object o)
		{
			return o is AllMembershipCondition;
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
			return typeof(AllMembershipCondition).GetHashCode();
		}

		public override string ToString()
		{
			return "All code";
		}

		public SecurityElement ToXml()
		{
			return ToXml(null);
		}

		public SecurityElement ToXml(PolicyLevel level)
		{
			return MembershipConditionHelper.Element(typeof(AllMembershipCondition), version);
		}
	}
}
