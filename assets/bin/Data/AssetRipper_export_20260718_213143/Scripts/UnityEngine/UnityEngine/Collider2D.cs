using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public class Collider2D : Behaviour
	{
		public extern bool isTrigger
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern Rigidbody2D attachedRigidbody
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern int shapeCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public Bounds bounds
		{
			get
			{
				Bounds value;
				INTERNAL_get_bounds(out value);
				return value;
			}
		}

		internal extern ColliderErrorState2D errorState
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern PhysicsMaterial2D sharedMaterial
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
		private extern void INTERNAL_get_bounds(out Bounds value);

		public bool OverlapPoint(Vector2 point)
		{
			return INTERNAL_CALL_OverlapPoint(this, ref point);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern bool INTERNAL_CALL_OverlapPoint(Collider2D self, ref Vector2 point);
	}
}
