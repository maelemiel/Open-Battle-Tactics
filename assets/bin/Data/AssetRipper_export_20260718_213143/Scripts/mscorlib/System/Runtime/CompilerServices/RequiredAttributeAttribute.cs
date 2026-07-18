using System.Runtime.InteropServices;

namespace System.Runtime.CompilerServices
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
	[ComVisible(true)]
	public sealed class RequiredAttributeAttribute : Attribute
	{
		public Type RequiredContract
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public RequiredAttributeAttribute(Type requiredContract)
		{
		}
	}
}
