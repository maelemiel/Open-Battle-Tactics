using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class ClothRenderer : Renderer
	{
		public extern bool pauseWhenNotVisible
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
