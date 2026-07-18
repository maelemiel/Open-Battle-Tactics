using System.Runtime.InteropServices;

namespace System.Security.Policy
{
	[Serializable]
	[ComVisible(true)]
	public sealed class FirstMatchCodeGroup : CodeGroup
	{
		public override string MergeLogic
		{
			get
			{
				return "First Match";
			}
		}

		public FirstMatchCodeGroup(IMembershipCondition membershipCondition, PolicyStatement policy)
			: base(membershipCondition, policy)
		{
		}

		internal FirstMatchCodeGroup(SecurityElement e, PolicyLevel level)
			: base(e, level)
		{
		}

		public override CodeGroup Copy()
		{
			FirstMatchCodeGroup firstMatchCodeGroup = CopyNoChildren();
			foreach (CodeGroup child in base.Children)
			{
				firstMatchCodeGroup.AddChild(child.Copy());
			}
			return firstMatchCodeGroup;
		}

		public override PolicyStatement Resolve(Evidence evidence)
		{
			if (evidence == null)
			{
				throw new ArgumentNullException("evidence");
			}
			if (!base.MembershipCondition.Check(evidence))
			{
				return null;
			}
			foreach (CodeGroup child in base.Children)
			{
				PolicyStatement policyStatement = child.Resolve(evidence);
				if (policyStatement != null)
				{
					return policyStatement;
				}
			}
			return base.PolicyStatement;
		}

		public override CodeGroup ResolveMatchingCodeGroups(Evidence evidence)
		{
			if (evidence == null)
			{
				throw new ArgumentNullException("evidence");
			}
			if (!base.MembershipCondition.Check(evidence))
			{
				return null;
			}
			foreach (CodeGroup child in base.Children)
			{
				if (child.Resolve(evidence) != null)
				{
					return child.Copy();
				}
			}
			return CopyNoChildren();
		}

		private FirstMatchCodeGroup CopyNoChildren()
		{
			FirstMatchCodeGroup firstMatchCodeGroup = new FirstMatchCodeGroup(base.MembershipCondition, base.PolicyStatement);
			firstMatchCodeGroup.Name = base.Name;
			firstMatchCodeGroup.Description = base.Description;
			return firstMatchCodeGroup;
		}
	}
}
