using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class CharacterJoint : Joint
	{
		public Vector3 swingAxis
		{
			get
			{
				Vector3 value;
				INTERNAL_get_swingAxis(out value);
				return value;
			}
			set
			{
				INTERNAL_set_swingAxis(ref value);
			}
		}

		public SoftJointLimit lowTwistLimit
		{
			get
			{
				SoftJointLimit value;
				INTERNAL_get_lowTwistLimit(out value);
				return value;
			}
			set
			{
				INTERNAL_set_lowTwistLimit(ref value);
			}
		}

		public SoftJointLimit highTwistLimit
		{
			get
			{
				SoftJointLimit value;
				INTERNAL_get_highTwistLimit(out value);
				return value;
			}
			set
			{
				INTERNAL_set_highTwistLimit(ref value);
			}
		}

		public SoftJointLimit swing1Limit
		{
			get
			{
				SoftJointLimit value;
				INTERNAL_get_swing1Limit(out value);
				return value;
			}
			set
			{
				INTERNAL_set_swing1Limit(ref value);
			}
		}

		public SoftJointLimit swing2Limit
		{
			get
			{
				SoftJointLimit value;
				INTERNAL_get_swing2Limit(out value);
				return value;
			}
			set
			{
				INTERNAL_set_swing2Limit(ref value);
			}
		}

		public Quaternion targetRotation
		{
			get
			{
				Quaternion value;
				INTERNAL_get_targetRotation(out value);
				return value;
			}
			set
			{
				INTERNAL_set_targetRotation(ref value);
			}
		}

		public Vector3 targetAngularVelocity
		{
			get
			{
				Vector3 value;
				INTERNAL_get_targetAngularVelocity(out value);
				return value;
			}
			set
			{
				INTERNAL_set_targetAngularVelocity(ref value);
			}
		}

		public JointDrive rotationDrive
		{
			get
			{
				JointDrive value;
				INTERNAL_get_rotationDrive(out value);
				return value;
			}
			set
			{
				INTERNAL_set_rotationDrive(ref value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_swingAxis(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_swingAxis(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_lowTwistLimit(out SoftJointLimit value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_lowTwistLimit(ref SoftJointLimit value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_highTwistLimit(out SoftJointLimit value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_highTwistLimit(ref SoftJointLimit value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_swing1Limit(out SoftJointLimit value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_swing1Limit(ref SoftJointLimit value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_swing2Limit(out SoftJointLimit value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_swing2Limit(ref SoftJointLimit value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_targetRotation(out Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_targetRotation(ref Quaternion value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_targetAngularVelocity(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_targetAngularVelocity(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_rotationDrive(out JointDrive value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_rotationDrive(ref JointDrive value);
	}
}
