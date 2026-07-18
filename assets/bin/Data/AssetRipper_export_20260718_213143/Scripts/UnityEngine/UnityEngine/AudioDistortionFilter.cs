using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class AudioDistortionFilter : Behaviour
	{
		public extern float distortionLevel
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
