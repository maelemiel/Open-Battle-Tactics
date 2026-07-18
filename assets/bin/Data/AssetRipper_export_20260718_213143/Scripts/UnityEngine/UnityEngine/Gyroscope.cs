using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class Gyroscope
	{
		private int m_GyroIndex;

		public Vector3 rotationRate
		{
			get
			{
				return rotationRate_Internal(m_GyroIndex);
			}
		}

		public Vector3 rotationRateUnbiased
		{
			get
			{
				return rotationRateUnbiased_Internal(m_GyroIndex);
			}
		}

		public Vector3 gravity
		{
			get
			{
				return gravity_Internal(m_GyroIndex);
			}
		}

		public Vector3 userAcceleration
		{
			get
			{
				return userAcceleration_Internal(m_GyroIndex);
			}
		}

		public Quaternion attitude
		{
			get
			{
				return attitude_Internal(m_GyroIndex);
			}
		}

		public bool enabled
		{
			get
			{
				return getEnabled_Internal(m_GyroIndex);
			}
			set
			{
				setEnabled_Internal(m_GyroIndex, value);
			}
		}

		public float updateInterval
		{
			get
			{
				return getUpdateInterval_Internal(m_GyroIndex);
			}
			set
			{
				setUpdateInterval_Internal(m_GyroIndex, value);
			}
		}

		internal Gyroscope(int index)
		{
			m_GyroIndex = index;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern Vector3 rotationRate_Internal(int idx);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern Vector3 rotationRateUnbiased_Internal(int idx);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern Vector3 gravity_Internal(int idx);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern Vector3 userAcceleration_Internal(int idx);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern Quaternion attitude_Internal(int idx);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern bool getEnabled_Internal(int idx);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void setEnabled_Internal(int idx, bool enabled);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern float getUpdateInterval_Internal(int idx);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void setUpdateInterval_Internal(int idx, float interval);
	}
}
