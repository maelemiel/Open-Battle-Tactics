using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class ComputeShader : Object
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern int FindKernel(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void SetFloat(string name, float val);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void SetInt(string name, int val);

		public void SetVector(string name, Vector4 val)
		{
			INTERNAL_CALL_SetVector(this, name, ref val);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_SetVector(ComputeShader self, string name, ref Vector4 val);

		public void SetFloats(string name, params float[] values)
		{
			Internal_SetFloats(name, values);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Internal_SetFloats(string name, float[] values);

		public void SetInts(string name, params int[] values)
		{
			Internal_SetInts(name, values);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Internal_SetInts(string name, int[] values);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void SetTexture(int kernelIndex, string name, Texture texture);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void SetBuffer(int kernelIndex, string name, ComputeBuffer buffer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void Dispatch(int kernelIndex, int threadsX, int threadsY, int threadsZ);
	}
}
