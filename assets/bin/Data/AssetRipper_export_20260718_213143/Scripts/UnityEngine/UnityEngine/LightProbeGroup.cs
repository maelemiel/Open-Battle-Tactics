using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class LightProbeGroup : Component
	{
		public extern Vector3[] probePositions
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
