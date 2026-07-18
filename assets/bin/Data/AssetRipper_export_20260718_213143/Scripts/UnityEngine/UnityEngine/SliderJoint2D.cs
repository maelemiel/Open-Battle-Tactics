using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class SliderJoint2D : AnchoredJoint2D
	{
		public extern float angle
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
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

		public JointTranslationLimits2D limits
		{
			get
			{
				JointTranslationLimits2D value;
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
		private extern void INTERNAL_get_motor(out JointMotor2D value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_motor(ref JointMotor2D value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_limits(out JointTranslationLimits2D value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_limits(ref JointTranslationLimits2D value);

		public float GetMotorForce(float timeStep)
		{
			return INTERNAL_CALL_GetMotorForce(this, timeStep);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern float INTERNAL_CALL_GetMotorForce(SliderJoint2D self, float timeStep);
	}
}
