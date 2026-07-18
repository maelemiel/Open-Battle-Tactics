using System.Runtime.InteropServices;

namespace System.Security.Permissions
{
	[Serializable]
	[ComVisible(true)]
	[Obsolete("CAS support is not available with Silverlight applications.")]
	public enum SecurityAction
	{
		Demand = 2,
		Assert = 3,
		Deny = 4,
		PermitOnly = 5,
		LinkDemand = 6,
		InheritanceDemand = 7,
		RequestMinimum = 8,
		RequestOptional = 9,
		RequestRefuse = 10
	}
}
