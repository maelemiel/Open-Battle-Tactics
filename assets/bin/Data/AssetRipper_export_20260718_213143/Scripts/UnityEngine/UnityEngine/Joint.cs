using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public class Joint : Component
	{
		public extern Rigidbody connectedBody
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public Vector3 axis
		{
			get
			{
				Vector3 value;
				INTERNAL_get_axis(out value);
				return value;
			}
			set
			{
				INTERNAL_set_axis(ref value);
			}
		}

		public Vector3 anchor
		{
			get
			{
				Vector3 value;
				INTERNAL_get_anchor(out value);
				return value;
			}
			set
			{
				INTERNAL_set_anchor(ref value);
			}
		}

		public Vector3 connectedAnchor
		{
			get
			{
				Vector3 value;
				INTERNAL_get_connectedAnchor(out value);
				return value;
			}
			set
			{
				INTERNAL_set_connectedAnchor(ref value);
			}
		}

		public extern bool autoConfigureConnectedAnchor
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float breakForce
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float breakTorque
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern bool enableCollision
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
		private extern void INTERNAL_get_axis(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_axis(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_anchor(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_anchor(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_connectedAnchor(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_connectedAnchor(ref Vector3 value);
	}
}
