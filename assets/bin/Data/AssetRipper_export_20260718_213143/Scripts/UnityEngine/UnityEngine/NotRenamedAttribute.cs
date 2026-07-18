using System;

namespace UnityEngine
{
	[NotConverted]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Interface)]
	public sealed class NotRenamedAttribute : Attribute
	{
	}
}
