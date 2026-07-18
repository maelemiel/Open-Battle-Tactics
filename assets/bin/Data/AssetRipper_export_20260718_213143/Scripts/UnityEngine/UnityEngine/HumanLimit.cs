namespace UnityEngine
{
	public struct HumanLimit
	{
		private Vector3 m_Min;

		private Vector3 m_Max;

		private Vector3 m_Center;

		private float m_AxisLength;

		private int m_UseDefaultValues;

		public bool useDefaultValues
		{
			get
			{
				return m_UseDefaultValues != 0;
			}
			set
			{
				m_UseDefaultValues = (value ? 1 : 0);
			}
		}

		public Vector3 min
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

		public Vector3 max
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

		public Vector3 center
		{
			get
			{
				return m_Center;
			}
			set
			{
				m_Center = value;
			}
		}

		public float axisLength
		{
			get
			{
				return m_AxisLength;
			}
			set
			{
				m_AxisLength = value;
			}
		}
	}
}
