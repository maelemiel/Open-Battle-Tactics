using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Serializable]
	[ComVisible(true)]
	public struct CustomAttributeNamedArgument
	{
		private CustomAttributeTypedArgument typedArgument;

		private MemberInfo memberInfo;

		public MemberInfo MemberInfo
		{
			get
			{
				return memberInfo;
			}
		}

		public CustomAttributeTypedArgument TypedValue
		{
			get
			{
				return typedArgument;
			}
		}

		internal CustomAttributeNamedArgument(MemberInfo memberInfo, object typedArgument)
		{
			this.memberInfo = memberInfo;
			this.typedArgument = (CustomAttributeTypedArgument)typedArgument;
		}

		public override string ToString()
		{
			return memberInfo.Name + " = " + typedArgument.ToString();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is CustomAttributeNamedArgument))
			{
				return false;
			}
			CustomAttributeNamedArgument customAttributeNamedArgument = (CustomAttributeNamedArgument)obj;
			return customAttributeNamedArgument.memberInfo == memberInfo && typedArgument.Equals(customAttributeNamedArgument.typedArgument);
		}

		public override int GetHashCode()
		{
			return (memberInfo.GetHashCode() << 16) + typedArgument.GetHashCode();
		}

		public static bool operator ==(CustomAttributeNamedArgument left, CustomAttributeNamedArgument right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(CustomAttributeNamedArgument left, CustomAttributeNamedArgument right)
		{
			return !left.Equals(right);
		}
	}
}
