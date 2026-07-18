using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class HingeJoint : Joint
	{
		public JointMotor motor
		{
			get
			{
				JointMotor value;
				INTERNAL_get_motor(out value);
				return value;
			}
			set
			{
				INTERNAL_set_motor(ref value);
			}
		}

		public JointLimits limits
		{
			get
			{
				JointLimits value;
				INTERNAL_get_limits(out value);
				return value;
			}
			set
			{
				INTERNAL_set_limits(ref value);
			}
		}

		public JointSpring spring
		{
			get
			{
				JointSpring value;
				INTERNAL_get_spring(out value);
				return value;
			}
			set
			{
				INTERNAL_set_spring(ref value);
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

		public extern bool useLimits
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern bool useSpring
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float velocity
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern float angle
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_motor(out JointMotor value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_motor(ref JointMotor value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_limits(out JointLimits value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_limits(ref JointLimits value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_spring(out JointSpring value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_spring(ref JointSpring value);
	}
}
