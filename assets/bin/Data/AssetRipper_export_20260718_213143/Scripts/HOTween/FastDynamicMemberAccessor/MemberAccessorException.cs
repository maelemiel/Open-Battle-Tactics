using System;

namespace FastDynamicMemberAccessor
{
	internal class MemberAccessorException : Exception
	{
		internal MemberAccessorException(string message)
			: base(message)
		{
		}
	}
}
