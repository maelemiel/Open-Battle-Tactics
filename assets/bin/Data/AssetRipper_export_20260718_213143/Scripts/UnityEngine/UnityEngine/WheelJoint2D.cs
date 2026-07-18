using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class WheelJoint2D : AnchoredJoint2D
	{
		public JointSuspension2D suspension
		{
			get
			{
				JointSuspension2D value;
				INTERNAL_get_suspension(out value);
				return value;
			}
			set
			{
				INTERNAL_set_suspension(ref value);
			}
		}

		public extern bool useMotor
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public JointMotor2D motor
		{
			get
			{
				JointMotor2D value;
				INTERNAL_get_motor(out value);
				return value;
			}
			set
			{
				INTERNAL_set_motor(ref value);
			}
		}

		public extern float jointTranslation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern float jointSpeed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_suspension(out JointSuspension2D value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_suspension(ref JointSuspension2D value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_motor(out JointMotor2D value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_motor(ref JointMotor2D value);

		public float GetMotorTorque(float timeStep)
		{
			return INTERNAL_CALL_GetMotorTorque(this, timeStep);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern float INTERNAL_CALL_GetMotorTorque(WheelJoint2D self, float timeStep);
	}
}
