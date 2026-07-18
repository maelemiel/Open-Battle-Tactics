using System.Runtime.CompilerServices;

namespace System.Reflection
{
	[Serializable]
	internal class MonoGenericCMethod : MonoCMethod
	{
		public override extern Type ReflectedType
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal MonoGenericCMethod()
		{
			throw new InvalidOperationException();
		}
	}
}
