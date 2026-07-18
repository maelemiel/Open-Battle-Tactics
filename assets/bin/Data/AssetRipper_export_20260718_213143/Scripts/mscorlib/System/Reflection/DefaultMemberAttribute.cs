using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Serializable]
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
	public sealed class DefaultMemberAttribute : Attribute
	{
		private string member_name;

		public string MemberName
		{
			get
			{
				return member_name;
			}
		}

		public DefaultMemberAttribute(string memberName)
		{
			member_name = memberName;
		}
	}
}
