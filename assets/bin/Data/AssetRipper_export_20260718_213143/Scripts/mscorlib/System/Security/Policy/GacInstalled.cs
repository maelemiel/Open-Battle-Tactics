using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Security.Policy
{
	[Serializable]
	[ComVisible(true)]
	public sealed class GacInstalled : IBuiltInEvidence, IIdentityPermissionFactory
	{
		int IBuiltInEvidence.GetRequiredSize(bool verbose)
		{
			return 1;
		}

		int IBuiltInEvidence.InitFromBuffer(char[] buffer, int position)
		{
			return position;
		}

		int IBuiltInEvidence.OutputToBuffer(char[] buffer, int position, bool verbose)
		{
			buffer[position] = '\t';
			return position + 1;
		}

		public object Copy()
		{
			return new GacInstalled();
		}

		public IPermission CreateIdentityPermission(Evidence evidence)
		{
			return new GacIdentityPermission();
		}

		public override bool Equals(object o)
		{
			if (o == null)
			{
				return false;
			}
			return o is GacInstalled;
		}

		public override int GetHashCode()
		{
			return 0;
		}

		public override string ToString()
		{
			SecurityElement securityElement = new SecurityElement(GetType().FullName);
			securityElement.AddAttribute("version", "1");
			return securityElement.ToString();
		}
	}
}
