using System.Runtime.InteropServices;
using System.Security.Policy;

namespace System.Security
{
	[ComVisible(true)]
	public interface ISecurityPolicyEncodable
	{
		void FromXml(SecurityElement e, PolicyLevel level);

		SecurityElement ToXml(PolicyLevel level);
	}
}
