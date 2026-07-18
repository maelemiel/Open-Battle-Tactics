namespace UnityEngine
{
	public struct MatchTargetWeightMask
	{
		private Vector3 m_PositionXYZWeight;

		private float m_RotationWeight;

		public Vector3 positionXYZWeight
		{
			get
			{
				return m_PositionXYZWeight;
			}
			set
			{
				m_PositionXYZWeight = value;
			}
		}

		public float rotationWeight
		{
			get
			{
				return m_RotationWeight;
			}
			set
			{
				m_RotationWeight = value;
			}
		}

		public MatchTargetWeightMask(Vector3 positionXYZWeight, float rotationWeight)
		{
			m_PositionXYZWeight = positionXYZWeight;
			m_RotationWeight = rotationWeight;
		}
	}
}
