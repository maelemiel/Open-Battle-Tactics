namespace UnityEngine
{
	public struct HumanDescription
	{
		public HumanBone[] human;

		public SkeletonBone[] skeleton;

		private float m_ArmTwist;

		private float m_ForeArmTwist;

		private float m_UpperLegTwist;

		private float m_LegTwist;

		private float m_ArmStretch;

		private float m_LegStretch;

		private float m_FeetSpacing;

		public float upperArmTwist
		{
			get
			{
				return m_ArmTwist;
			}
			set
			{
				m_ArmTwist = value;
			}
		}

		public float lowerArmTwist
		{
			get
			{
				return m_ForeArmTwist;
			}
			set
			{
				m_ForeArmTwist = value;
			}
		}

		public float upperLegTwist
		{
			get
			{
				return m_UpperLegTwist;
			}
			set
			{
				m_UpperLegTwist = value;
			}
		}

		public float lowerLegTwist
		{
			get
			{
				return m_LegTwist;
			}
			set
			{
				m_LegTwist = value;
			}
		}

		public float armStretch
		{
			get
			{
				return m_ArmStretch;
			}
			set
			{
				m_ArmStretch = value;
			}
		}

		public float legStretch
		{
			get
			{
				return m_LegStretch;
			}
			set
			{
				m_LegStretch = value;
			}
		}

		public float feetSpacing
		{
			get
			{
				return m_FeetSpacing;
			}
			set
			{
				m_FeetSpacing = value;
			}
		}
	}
}
