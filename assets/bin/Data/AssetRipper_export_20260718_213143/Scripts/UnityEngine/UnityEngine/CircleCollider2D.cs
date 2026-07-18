using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class CircleCollider2D : Collider2D
	{
		public Vector2 center
		{
			get
			{
				Vector2 value;
				INTERNAL_get_center(out value);
				return value;
			}
			set
			{
				INTERNAL_set_center(ref value);
			}
		}

		public extern float radius
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
		private extern void INTERNAL_get_center(out Vector2 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_center(ref Vector2 value);
	}
}
