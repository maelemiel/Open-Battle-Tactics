using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class LightProbes : Object
	{
		public extern Vector3[] positions
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern float[] coefficients
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern int count
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern int cellCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public void GetInterpolatedLightProbe(Vector3 position, Renderer renderer, float[] coefficients)
		{
			INTERNAL_CALL_GetInterpolatedLightProbe(this, ref position, renderer, coefficients);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_GetInterpolatedLightProbe(LightProbes self, ref Vector3 position, Renderer renderer, float[] coefficients);
	}
}
