using System;

namespace UnityEngine
{
	[Obsolete("iPhoneTouch struct is deprecated. Please use Touch instead.")]
	public struct iPhoneTouch
	{
		private int m_FingerId;

		private Vector2 m_Position;

		private Vector2 m_PositionDelta;

		private float m_TimeDelta;

		private int m_TapCount;

		private iPhoneTouchPhase m_Phase;

		public int fingerId
		{
			get
			{
				return m_FingerId;
			}
		}

		public Vector2 position
		{
			get
			{
				return m_Position;
			}
		}

		public Vector2 deltaPosition
		{
			get
			{
				return m_PositionDelta;
			}
		}

		public float deltaTime
		{
			get
			{
				return m_TimeDelta;
			}
		}

		public int tapCount
		{
			get
			{
				return m_TapCount;
			}
		}

		public iPhoneTouchPhase phase
		{
			get
			{
				return m_Phase;
			}
		}

		[Obsolete("positionDelta property is deprecated. Please use iPhoneTouch.deltaPosition instead.")]
		public Vector2 positionDelta
		{
			get
			{
				return m_PositionDelta;
			}
		}

		[Obsolete("timeDelta property is deprecated. Please use iPhoneTouch.deltaTime instead.")]
		public float timeDelta
		{
			get
			{
				return m_TimeDelta;
			}
		}
	}
}
