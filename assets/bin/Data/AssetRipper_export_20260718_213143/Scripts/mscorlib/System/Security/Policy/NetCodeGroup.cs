using System.Collections;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Security.Policy
{
	[Serializable]
	[ComVisible(true)]
	public sealed class NetCodeGroup : CodeGroup
	{
		public static readonly string AbsentOriginScheme = string.Empty;

		public static readonly string AnyOtherOriginScheme = "*";

		private Hashtable _rules = new Hashtable();

		private int _hashcode;

		public override string AttributeString
		{
			get
			{
				return null;
			}
		}

		public override string MergeLogic
		{
			get
			{
				return "Union";
			}
		}

		public override string PermissionSetName
		{
			get
			{
				return "Same site Web";
			}
		}

		public NetCodeGroup(IMembershipCondition membershipCondition)
			: base(membershipCondition, null)
		{
		}

		internal NetCodeGroup(SecurityElement e, PolicyLevel level)
			: base(e, level)
		{
		}

		[MonoTODO("(2.0) missing validations")]
		public void AddConnectAccess(string originScheme, CodeConnectAccess connectAccess)
		{
			if (originScheme == null)
			{
				throw new ArgumentException("originScheme");
			}
			if (originScheme == AbsentOriginScheme && connectAccess.Scheme == CodeConnectAccess.OriginScheme)
			{
				throw new ArgumentOutOfRangeException("connectAccess", Locale.GetText("Schema == CodeConnectAccess.OriginScheme"));
			}
			if (_rules.ContainsKey(originScheme))
			{
				if (connectAccess != null)
				{
					CodeConnectAccess[] array = (CodeConnectAccess[])_rules[originScheme];
					CodeConnectAccess[] array2 = new CodeConnectAccess[array.Length + 1];
					Array.Copy(array, 0, array2, 0, array.Length);
					array2[array.Length] = connectAccess;
					_rules[originScheme] = array2;
				}
			}
			else
			{
				CodeConnectAccess[] value = new CodeConnectAccess[1] { connectAccess };
				_rules.Add(originScheme, value);
			}
		}

		public override CodeGroup Copy()
		{
			NetCodeGroup netCodeGroup = new NetCodeGroup(base.MembershipCondition);
			netCodeGroup.Name = base.Name;
			netCodeGroup.Description = base.Description;
			netCodeGroup.PolicyStatement = base.PolicyStatement;
			foreach (CodeGroup child in base.Children)
			{
				netCodeGroup.AddChild(child.Copy());
			}
			return netCodeGroup;
		}

		private bool Equals(CodeConnectAccess[] rules1, CodeConnectAccess[] rules2)
		{
			for (int i = 0; i < rules1.Length; i++)
			{
				bool flag = false;
				for (int j = 0; j < rules2.Length; j++)
				{
					if (rules1[i].Equals(rules2[j]))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		public override bool Equals(object o)
		{
			if (!base.Equals(o))
			{
				return false;
			}
			NetCodeGroup netCodeGroup = o as NetCodeGroup;
			if (netCodeGroup == null)
			{
				return false;
			}
			foreach (DictionaryEntry rule in _rules)
			{
				bool flag = false;
				CodeConnectAccess[] array = (CodeConnectAccess[])netCodeGroup._rules[rule.Key];
				if (!((array == null) ? (rule.Value == null) : Equals((CodeConnectAccess[])rule.Value, array)))
				{
					return false;
				}
			}
			return true;
		}

		public DictionaryEntry[] GetConnectAccessRules()
		{
			DictionaryEntry[] array = new DictionaryEntry[_rules.Count];
			_rules.CopyTo(array, 0);
			return array;
		}

		public override int GetHashCode()
		{
			if (_hashcode == 0)
			{
				_hashcode = base.GetHashCode();
				foreach (DictionaryEntry rule in _rules)
				{
					CodeConnectAccess[] array = (CodeConnectAccess[])rule.Value;
					if (array != null)
					{
						CodeConnectAccess[] array2 = array;
						foreach (CodeConnectAccess codeConnectAccess in array2)
						{
							_hashcode ^= codeConnectAccess.GetHashCode();
						}
					}
				}
			}
			return _hashcode;
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
			PolicyStatement policyStatement2 = base.PolicyStatement.Copy();
			policyStatement2.PermissionSet = permissionSet;
			return policyStatement2;
		}

		public void ResetConnectAccess()
		{
			_rules.Clear();
		}

		public override CodeGroup ResolveMatchingCodeGroups(Evidence evidence)
		{
			if (evidence == null)
			{
				throw new ArgumentNullException("evidence");
			}
			CodeGroup codeGroup = null;
			if (base.MembershipCondition.Check(evidence))
			{
				codeGroup = Copy();
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

		[MonoTODO("(2.0) Add new stuff (CodeConnectAccess) into XML")]
		protected override void CreateXml(SecurityElement element, PolicyLevel level)
		{
			base.CreateXml(element, level);
		}

		[MonoTODO("(2.0) Parse new stuff (CodeConnectAccess) from XML")]
		protected override void ParseXml(SecurityElement e, PolicyLevel level)
		{
			base.ParseXml(e, level);
		}
	}
}
