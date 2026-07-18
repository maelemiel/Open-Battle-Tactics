using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class WheelCollider : Collider
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

		public extern float suspensionDistance
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public JointSpring suspensionSpring
		{
			get
			{
				JointSpring value;
				INTERNAL_get_suspensionSpring(out value);
				return value;
			}
			set
			{
				INTERNAL_set_suspensionSpring(ref value);
			}
		}

		public extern float mass
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public WheelFrictionCurve forwardFriction
		{
			get
			{
				WheelFrictionCurve value;
				INTERNAL_get_forwardFriction(out value);
				return value;
			}
			set
			{
				INTERNAL_set_forwardFriction(ref value);
			}
		}

		public WheelFrictionCurve sidewaysFriction
		{
			get
			{
				WheelFrictionCurve value;
				INTERNAL_get_sidewaysFriction(out value);
				return value;
			}
			set
			{
				INTERNAL_set_sidewaysFriction(ref value);
			}
		}

		public extern float motorTorque
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float brakeTorque
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float steerAngle
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern bool isGrounded
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern float rpm
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_center(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_center(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_suspensionSpring(out JointSpring value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_suspensionSpring(ref JointSpring value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_forwardFriction(out WheelFrictionCurve value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_forwardFriction(ref WheelFrictionCurve value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_sidewaysFriction(out WheelFrictionCurve value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_sidewaysFriction(ref WheelFrictionCurve value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern bool GetGroundHit(out WheelHit hit);
	}
}
