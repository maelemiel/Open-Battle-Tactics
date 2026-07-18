namespace UnityEngine
{
	public struct JointLimits
	{
		private float m_Min;

		private float m_MinBounce;

		private float m_MinHardness;

		private float m_Max;

		private float m_MaxBounce;

		private float m_MaxHardness;

		public float min
		{
			get
			{
				return m_Min;
			}
			set
			{
				m_Min = value;
			}
		}

		public float minBounce
		{
			get
			{
				return m_MinBounce;
			}
			set
			{
				m_MinBounce = value;
			}
		}

		public float max
		{
			get
			{
				return m_Max;
			}
			set
			{
				m_Max = value;
			}
		}

		public float maxBounce
		{
			get
			{
				return m_MaxBounce;
			}
			set
			{
				m_MaxBounce = value;
			}
		}
	}
}
