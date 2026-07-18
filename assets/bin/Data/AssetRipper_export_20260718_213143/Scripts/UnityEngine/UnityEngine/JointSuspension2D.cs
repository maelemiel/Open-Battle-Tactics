namespace UnityEngine
{
	public struct JointSuspension2D
	{
		private float m_DampingRatio;

		private float m_Frequency;

		private float m_Angle;

		public float dampingRatio
		{
			get
			{
				return m_DampingRatio;
			}
			set
			{
				m_DampingRatio = value;
			}
		}

		public float frequency
		{
			get
			{
				return m_Frequency;
			}
			set
			{
				m_Frequency = value;
			}
		}

		public float angle
		{
			get
			{
				return m_Angle;
			}
			set
			{
				m_Angle = value;
			}
		}
	}
}
