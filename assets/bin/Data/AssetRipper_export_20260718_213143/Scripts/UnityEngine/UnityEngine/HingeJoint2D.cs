using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class HingeJoint2D : AnchoredJoint2D
	{
		public extern bool useMotor
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern bool useLimits
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

		public JointAngleLimits2D limits
		{
			get
			{
				JointAngleLimits2D value;
				INTERNAL_get_limits(out value);
				return value;
			}
			set
			{
				INTERNAL_set_limits(ref value);
			}
		}

		public extern JointLimitState2D limitState
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern float referenceAngle
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern float jointAngle
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
		private extern void INTERNAL_get_motor(out JointMotor2D value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_motor(ref JointMotor2D value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_limits(out JointAngleLimits2D value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_limits(ref JointAngleLimits2D value);

		public Vector2 GetReactionForce(float timeStep)
		{
			Vector2 value;
			HingeJoint2D_CUSTOM_INTERNAL_GetReactionForce(this, timeStep, out value);
			return value;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void HingeJoint2D_CUSTOM_INTERNAL_GetReactionForce(HingeJoint2D joint, float timeStep, out Vector2 value);

		public float GetReactionTorque(float timeStep)
		{
			return INTERNAL_CALL_GetReactionTorque(this, timeStep);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern float INTERNAL_CALL_GetReactionTorque(HingeJoint2D self, float timeStep);

		public float GetMotorTorque(float timeStep)
		{
			return INTERNAL_CALL_GetMotorTorque(this, timeStep);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern float INTERNAL_CALL_GetMotorTorque(HingeJoint2D self, float timeStep);
	}
}
