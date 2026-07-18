using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	public sealed class PolygonCollider2D : Collider2D
	{
		public extern Vector2[] points
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern int pathCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern Vector2[] GetPath(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void SetPath(int index, Vector2[] points);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern int GetTotalPointCount();

		public void CreatePrimitive(int sides, [DefaultValue("Vector2.one")] Vector2 scale, [DefaultValue("Vector2.zero")] Vector2 offset)
		{
			INTERNAL_CALL_CreatePrimitive(this, sides, ref scale, ref offset);
		}

		[ExcludeFromDocs]
		public void CreatePrimitive(int sides, Vector2 scale)
		{
			Vector2 offset = Vector2.zero;
			INTERNAL_CALL_CreatePrimitive(this, sides, ref scale, ref offset);
		}

		[ExcludeFromDocs]
		public void CreatePrimitive(int sides)
		{
			Vector2 offset = Vector2.zero;
			Vector2 scale = Vector2.one;
			INTERNAL_CALL_CreatePrimitive(this, sides, ref scale, ref offset);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_CreatePrimitive(PolygonCollider2D self, int sides, ref Vector2 scale, ref Vector2 offset);
	}
}
