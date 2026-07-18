using System.Runtime.InteropServices;

namespace System.Security.Policy
{
	[Serializable]
	[ComVisible(true)]
	public sealed class UnionCodeGroup : CodeGroup
	{
		public override string MergeLogic
		{
			get
			{
				return "Union";
			}
		}

		public UnionCodeGroup(IMembershipCondition membershipCondition, PolicyStatement policy)
			: base(membershipCondition, policy)
		{
		}

		internal UnionCodeGroup(SecurityElement e, PolicyLevel level)
			: base(e, level)
		{
		}

		public override CodeGroup Copy()
		{
			return Copy(true);
		}

		internal CodeGroup Copy(bool childs)
		{
			UnionCodeGroup unionCodeGroup = new UnionCodeGroup(base.MembershipCondition, base.PolicyStatement);
			unionCodeGroup.Name = base.Name;
			unionCodeGroup.Description = base.Description;
			if (childs)
			{
				foreach (CodeGroup child in base.Children)
				{
					unionCodeGroup.AddChild(child.Copy());
				}
			}
			return unionCodeGroup;
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
			PermissionSet permissionSet = base.PolicyStatement.PermissionSet.Copy();
			if (base.Children.Count > 0)
			{
				foreach (CodeGroup child in base.Children)
				{
					PolicyStatement policyStatement = child.Resolve(evidence);
					if (policyStatement != null)
					{
						permissionSet = permissionSet.Union(policyStatement.PermissionSet);
					}
				}
			}
			PolicyStatement policyStatement2 = base.PolicyStatement.Copy();
			policyStatement2.PermissionSet = permissionSet;
			return policyStatement2;
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
			CodeGroup codeGroup = Copy(false);
			if (base.Children.Count > 0)
			{
				foreach (CodeGroup child in base.Children)
				{
					CodeGroup codeGroup3 = child.ResolveMatchingCodeGroups(evidence);
					if (codeGroup3 != null)
					{
						codeGroup.AddChild(codeGroup3);
					}
				}
			}
			return codeGroup;
		}
	}
}
