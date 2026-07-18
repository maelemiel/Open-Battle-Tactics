using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Security.Policy
{
	[Serializable]
	[ComVisible(true)]
	public sealed class FileCodeGroup : CodeGroup
	{
		private FileIOPermissionAccess m_access;

		public override string MergeLogic
		{
			get
			{
				return "Union";
			}
		}

		public override string AttributeString
		{
			get
			{
				return null;
			}
		}

		public override string PermissionSetName
		{
			get
			{
				return "Same directory FileIO - " + m_access;
			}
		}

		public FileCodeGroup(IMembershipCondition membershipCondition, FileIOPermissionAccess access)
			: base(membershipCondition, null)
		{
			m_access = access;
		}

		internal FileCodeGroup(SecurityElement e, PolicyLevel level)
			: base(e, level)
		{
		}

		public override CodeGroup Copy()
		{
			FileCodeGroup fileCodeGroup = new FileCodeGroup(base.MembershipCondition, m_access);
			fileCodeGroup.Name = base.Name;
			fileCodeGroup.Description = base.Description;
			foreach (CodeGroup child in base.Children)
			{
				fileCodeGroup.AddChild(child.Copy());
			}
			return fileCodeGroup;
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
			PermissionSet permissionSet = null;
			permissionSet = ((base.PolicyStatement != null) ? base.PolicyStatement.PermissionSet.Copy() : new PermissionSet(PermissionState.None));
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
			PolicyStatement policyStatement2 = null;
			policyStatement2 = ((base.PolicyStatement == null) ? PolicyStatement.Empty() : base.PolicyStatement.Copy());
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
			FileCodeGroup fileCodeGroup = new FileCodeGroup(base.MembershipCondition, m_access);
			foreach (CodeGroup child in base.Children)
			{
				CodeGroup codeGroup2 = child.ResolveMatchingCodeGroups(evidence);
				if (codeGroup2 != null)
				{
					fileCodeGroup.AddChild(codeGroup2);
				}
			}
			return fileCodeGroup;
		}

		public override bool Equals(object o)
		{
			if (!(o is FileCodeGroup))
			{
				return false;
			}
			if (m_access != ((FileCodeGroup)o).m_access)
			{
				return false;
			}
			return Equals((CodeGroup)o, false);
		}

		public override int GetHashCode()
		{
			return m_access.GetHashCode();
		}

		protected override void ParseXml(SecurityElement e, PolicyLevel level)
		{
			string text = e.Attribute("Access");
			if (text != null)
			{
				m_access = (FileIOPermissionAccess)(int)Enum.Parse(typeof(FileIOPermissionAccess), text, true);
			}
			else
			{
				m_access = FileIOPermissionAccess.NoAccess;
			}
		}

		protected override void CreateXml(SecurityElement element, PolicyLevel level)
		{
			element.AddAttribute("Access", m_access.ToString());
		}
	}
}
