namespace UnityEngine
{
	public struct HumanBone
	{
		private string m_BoneName;

		private string m_HumanName;

		public HumanLimit limit;

		public string boneName
		{
			get
			{
				return m_BoneName;
			}
			set
			{
				m_BoneName = value;
			}
		}

		public string humanName
		{
			get
			{
				return m_HumanName;
			}
			set
			{
				m_HumanName = value;
			}
		}
	}
}
