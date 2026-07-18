using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	[Obsolete("Use WheelCollider or BoxCollider instead, RaycastCollider is unreliable")]
	public sealed class RaycastCollider : Collider
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

		[Obsolete("Use WheelCollider or BoxCollider instead, RaycastCollider is unreliable")]
		public extern float length
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[Obsolete("Use WheelCollider or BoxCollider instead, RaycastCollider is unreliable")]
		[WrapperlessIcall]
		private extern void INTERNAL_get_center(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_center(ref Vector3 value);
	}
}
