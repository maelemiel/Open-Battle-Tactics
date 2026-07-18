using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class LineRenderer : Renderer
	{
		public extern bool useWorldSpace
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public void SetWidth(float start, float end)
		{
			INTERNAL_CALL_SetWidth(this, start, end);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_SetWidth(LineRenderer self, float start, float end);

		public void SetColors(Color start, Color end)
		{
			INTERNAL_CALL_SetColors(this, ref start, ref end);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_SetColors(LineRenderer self, ref Color start, ref Color end);

		public void SetVertexCount(int count)
		{
			INTERNAL_CALL_SetVertexCount(this, count);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_SetVertexCount(LineRenderer self, int count);

		public void SetPosition(int index, Vector3 position)
		{
			INTERNAL_CALL_SetPosition(this, index, ref position);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_SetPosition(LineRenderer self, int index, ref Vector3 position);
	}
}
