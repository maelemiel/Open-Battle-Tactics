using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class Skybox : Behaviour
	{
		public extern Material material
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}
	}
}
