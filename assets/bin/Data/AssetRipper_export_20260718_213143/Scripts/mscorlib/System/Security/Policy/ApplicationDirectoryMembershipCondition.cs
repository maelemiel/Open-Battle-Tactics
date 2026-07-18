using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using Mono.Security;

namespace System.Security.Policy
{
	[Serializable]
	[ComVisible(true)]
	public sealed class ApplicationDirectoryMembershipCondition : IConstantMembershipCondition, IMembershipCondition, ISecurityEncodable, ISecurityPolicyEncodable
	{
		private readonly int version = 1;

		public bool Check(Evidence evidence)
		{
			if (evidence == null)
			{
				return false;
			}
			string codeBase = Assembly.GetCallingAssembly().CodeBase;
			Uri uri = new Uri(codeBase);
			Url url = new Url(codeBase);
			bool flag = false;
			bool flag2 = false;
			IEnumerator hostEnumerator = evidence.GetHostEnumerator();
			while (hostEnumerator.MoveNext())
			{
				object current = hostEnumerator.Current;
				if (!flag && current is ApplicationDirectory)
				{
					ApplicationDirectory applicationDirectory = current as ApplicationDirectory;
					string directory = applicationDirectory.Directory;
					flag = string.Compare(directory, 0, uri.ToString(), 0, directory.Length, true, CultureInfo.InvariantCulture) == 0;
				}
				else if (!flag2 && current is Url)
				{
					flag2 = url.Equals(current);
				}
				if (flag && flag2)
				{
					return true;
				}
			}
			return false;
		}

		public IMembershipCondition Copy()
		{
			return new ApplicationDirectoryMembershipCondition();
		}

		public override bool Equals(object o)
		{
			return o is ApplicationDirectoryMembershipCondition;
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
			return typeof(ApplicationDirectoryMembershipCondition).GetHashCode();
		}

		public override string ToString()
		{
			return "ApplicationDirectory";
		}

		public SecurityElement ToXml()
		{
			return ToXml(null);
		}

		public SecurityElement ToXml(PolicyLevel level)
		{
			return MembershipConditionHelper.Element(typeof(ApplicationDirectoryMembershipCondition), version);
		}
	}
}
