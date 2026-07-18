using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class SphereCollider : Collider
	{
		public Vector3 center
		{
			get
			{
				Vector3 value;
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
		private extern void INTERNAL_get_center(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_center(ref Vector3 value);
	}
}
