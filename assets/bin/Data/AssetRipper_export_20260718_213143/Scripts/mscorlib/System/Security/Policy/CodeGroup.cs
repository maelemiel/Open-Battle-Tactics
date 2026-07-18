using System.Collections;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Security.Policy
{
	[Serializable]
	[ComVisible(true)]
	public abstract class CodeGroup
	{
		private PolicyStatement m_policy;

		private IMembershipCondition m_membershipCondition;

		private string m_description;

		private string m_name;

		private ArrayList m_children = new ArrayList();

		public abstract string MergeLogic { get; }

		public PolicyStatement PolicyStatement
		{
			get
			{
				return m_policy;
			}
			set
			{
				m_policy = value;
			}
		}

		public string Description
		{
			get
			{
				return m_description;
			}
			set
			{
				m_description = value;
			}
		}

		public IMembershipCondition MembershipCondition
		{
			get
			{
				return m_membershipCondition;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentException("value");
				}
				m_membershipCondition = value;
			}
		}

		public string Name
		{
			get
			{
				return m_name;
			}
			set
			{
				m_name = value;
			}
		}

		public IList Children
		{
			get
			{
				return m_children;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				m_children = new ArrayList(value);
			}
		}

		public virtual string AttributeString
		{
			get
			{
				if (m_policy != null)
				{
					return m_policy.AttributeString;
				}
				return null;
			}
		}

		public virtual string PermissionSetName
		{
			get
			{
				if (m_policy == null)
				{
					return null;
				}
				if (m_policy.PermissionSet is NamedPermissionSet)
				{
					return ((NamedPermissionSet)m_policy.PermissionSet).Name;
				}
				return null;
			}
		}

		protected CodeGroup(IMembershipCondition membershipCondition, PolicyStatement policy)
		{
			if (membershipCondition == null)
			{
				throw new ArgumentNullException("membershipCondition");
			}
			if (policy != null)
			{
				m_policy = policy.Copy();
			}
			m_membershipCondition = membershipCondition.Copy();
		}

		internal CodeGroup(SecurityElement e, PolicyLevel level)
		{
			FromXml(e, level);
		}

		public abstract CodeGroup Copy();

		public abstract PolicyStatement Resolve(Evidence evidence);

		public abstract CodeGroup ResolveMatchingCodeGroups(Evidence evidence);

		public void AddChild(CodeGroup group)
		{
			if (group == null)
			{
				throw new ArgumentNullException("group");
			}
			m_children.Add(group.Copy());
		}

		public override bool Equals(object o)
		{
			CodeGroup codeGroup = o as CodeGroup;
			if (codeGroup == null)
			{
				return false;
			}
			return Equals(codeGroup, false);
		}

		public bool Equals(CodeGroup cg, bool compareChildren)
		{
			if (cg.Name != Name)
			{
				return false;
			}
			if (cg.Description != Description)
			{
				return false;
			}
			if (!cg.MembershipCondition.Equals(m_membershipCondition))
			{
				return false;
			}
			if (compareChildren)
			{
				int count = cg.Children.Count;
				if (Children.Count != count)
				{
					return false;
				}
				for (int i = 0; i < count; i++)
				{
					if (!((CodeGroup)Children[i]).Equals((CodeGroup)cg.Children[i], false))
					{
						return false;
					}
				}
			}
			return true;
		}

		public void RemoveChild(CodeGroup group)
		{
			if (group != null)
			{
				m_children.Remove(group);
			}
		}

		public override int GetHashCode()
		{
			int num = m_membershipCondition.GetHashCode();
			if (m_policy != null)
			{
				num += m_policy.GetHashCode();
			}
			return num;
		}

		public void FromXml(SecurityElement e)
		{
			FromXml(e, null);
		}

		public void FromXml(SecurityElement e, PolicyLevel level)
		{
			if (e == null)
			{
				throw new ArgumentNullException("e");
			}
			PermissionSet permissionSet = null;
			string text = e.Attribute("PermissionSetName");
			if (text != null && level != null)
			{
				permissionSet = level.GetNamedPermissionSet(text);
			}
			else
			{
				SecurityElement securityElement = e.SearchForChildByTag("PermissionSet");
				if (securityElement != null)
				{
					Type type = Type.GetType(securityElement.Attribute("class"));
					permissionSet = (PermissionSet)Activator.CreateInstance(type, true);
					permissionSet.FromXml(securityElement);
				}
				else
				{
					permissionSet = new PermissionSet(new PermissionSet(PermissionState.None));
				}
			}
			m_policy = new PolicyStatement(permissionSet);
			m_children.Clear();
			if (e.Children != null && e.Children.Count > 0)
			{
				foreach (SecurityElement child in e.Children)
				{
					if (child.Tag == "CodeGroup")
					{
						AddChild(CreateFromXml(child, level));
					}
				}
			}
			m_membershipCondition = null;
			SecurityElement securityElement3 = e.SearchForChildByTag("IMembershipCondition");
			if (securityElement3 != null)
			{
				string text2 = securityElement3.Attribute("class");
				Type type2 = Type.GetType(text2);
				if (type2 == null)
				{
					type2 = Type.GetType("System.Security.Policy." + text2);
				}
				m_membershipCondition = (IMembershipCondition)Activator.CreateInstance(type2, true);
				m_membershipCondition.FromXml(securityElement3, level);
			}
			m_name = e.Attribute("Name");
			m_description = e.Attribute("Description");
			ParseXml(e, level);
		}

		protected virtual void ParseXml(SecurityElement e, PolicyLevel level)
		{
		}

		public SecurityElement ToXml()
		{
			return ToXml(null);
		}

		public SecurityElement ToXml(PolicyLevel level)
		{
			SecurityElement securityElement = new SecurityElement("CodeGroup");
			securityElement.AddAttribute("class", GetType().AssemblyQualifiedName);
			securityElement.AddAttribute("version", "1");
			if (Name != null)
			{
				securityElement.AddAttribute("Name", Name);
			}
			if (Description != null)
			{
				securityElement.AddAttribute("Description", Description);
			}
			if (MembershipCondition != null)
			{
				securityElement.AddChild(MembershipCondition.ToXml());
			}
			if (PolicyStatement != null && PolicyStatement.PermissionSet != null)
			{
				securityElement.AddChild(PolicyStatement.PermissionSet.ToXml());
			}
			foreach (CodeGroup child in Children)
			{
				securityElement.AddChild(child.ToXml());
			}
			CreateXml(securityElement, level);
			return securityElement;
		}

		protected virtual void CreateXml(SecurityElement element, PolicyLevel level)
		{
		}

		internal static CodeGroup CreateFromXml(SecurityElement se, PolicyLevel level)
		{
			string text = se.Attribute("class");
			string text2 = text;
			int num = text2.IndexOf(",");
			if (num > 0)
			{
				text2 = text2.Substring(0, num);
			}
			num = text2.LastIndexOf(".");
			if (num > 0)
			{
				text2 = text2.Substring(num + 1);
			}
			switch (text2)
			{
			case "FileCodeGroup":
				return new FileCodeGroup(se, level);
			case "FirstMatchCodeGroup":
				return new FirstMatchCodeGroup(se, level);
			case "NetCodeGroup":
				return new NetCodeGroup(se, level);
			case "UnionCodeGroup":
				return new UnionCodeGroup(se, level);
			default:
			{
				Type type = Type.GetType(text);
				CodeGroup codeGroup = (CodeGroup)Activator.CreateInstance(type, true);
				codeGroup.FromXml(se, level);
				return codeGroup;
			}
			}
		}
	}
}
