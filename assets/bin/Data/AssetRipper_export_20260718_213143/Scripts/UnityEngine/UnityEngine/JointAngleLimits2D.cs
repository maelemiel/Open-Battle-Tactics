namespace UnityEngine
{
	public struct JointAngleLimits2D
	{
		private float m_LowerAngle;

		private float m_UpperAngle;

		public float min
		{
			get
			{
				return m_LowerAngle;
			}
			set
			{
				m_LowerAngle = value;
			}
		}

		public float max
		{
			get
			{
				return m_UpperAngle;
			}
			set
			{
				m_UpperAngle = value;
			}
		}
	}
}
