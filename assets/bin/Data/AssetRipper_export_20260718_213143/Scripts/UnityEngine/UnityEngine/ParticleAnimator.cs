using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class ParticleAnimator : Component
	{
		public extern bool doesAnimateColor
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public Vector3 worldRotationAxis
		{
			get
			{
				Vector3 value;
				INTERNAL_get_worldRotationAxis(out value);
				return value;
			}
			set
			{
				INTERNAL_set_worldRotationAxis(ref value);
			}
		}

		public Vector3 localRotationAxis
		{
			get
			{
				Vector3 value;
				INTERNAL_get_localRotationAxis(out value);
				return value;
			}
			set
			{
				INTERNAL_set_localRotationAxis(ref value);
			}
		}

		public extern float sizeGrow
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public Vector3 rndForce
		{
			get
			{
				Vector3 value;
				INTERNAL_get_rndForce(out value);
				return value;
			}
			set
			{
				INTERNAL_set_rndForce(ref value);
			}
		}

		public Vector3 force
		{
			get
			{
				Vector3 value;
				INTERNAL_get_force(out value);
				return value;
			}
			set
			{
				INTERNAL_set_force(ref value);
			}
		}

		public extern float damping
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern bool autodestruct
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern Color[] colorAnimation
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
		private extern void INTERNAL_get_worldRotationAxis(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_worldRotationAxis(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_localRotationAxis(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_localRotationAxis(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_rndForce(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_rndForce(ref Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_get_force(out Vector3 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void INTERNAL_set_force(ref Vector3 value);
	}
}
