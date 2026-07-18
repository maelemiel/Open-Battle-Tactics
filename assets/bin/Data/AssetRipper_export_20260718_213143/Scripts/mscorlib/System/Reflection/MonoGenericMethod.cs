using System.Runtime.CompilerServices;

namespace System.Reflection
{
	[Serializable]
	internal class MonoGenericMethod : MonoMethod
	{
		public override extern Type ReflectedType
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal MonoGenericMethod()
		{
			throw new InvalidOperationException();
		}
	}
}
