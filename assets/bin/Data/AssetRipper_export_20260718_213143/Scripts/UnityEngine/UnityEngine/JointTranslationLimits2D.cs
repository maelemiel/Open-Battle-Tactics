namespace UnityEngine
{
	public struct JointTranslationLimits2D
	{
		private float m_LowerTranslation;

		private float m_UpperTranslation;

		public float min
		{
			get
			{
				return m_LowerTranslation;
			}
			set
			{
				m_LowerTranslation = value;
			}
		}

		public float max
		{
			get
			{
				return m_UpperTranslation;
			}
			set
			{
				m_UpperTranslation = value;
			}
		}
	}
}
