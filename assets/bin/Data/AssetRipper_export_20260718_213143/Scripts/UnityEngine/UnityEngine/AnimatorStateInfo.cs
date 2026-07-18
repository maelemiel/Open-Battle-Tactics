namespace UnityEngine
{
	public struct AnimatorStateInfo
	{
		private int m_Name;

		private int m_Path;

		private float m_NormalizedTime;

		private float m_Length;

		private int m_Tag;

		private int m_Loop;

		public int nameHash
		{
			get
			{
				return m_Path;
			}
		}

		public float normalizedTime
		{
			get
			{
				return m_NormalizedTime;
			}
		}

		public float length
		{
			get
			{
				return m_Length;
			}
		}

		public int tagHash
		{
			get
			{
				return m_Tag;
			}
		}

		public bool loop
		{
			get
			{
				return m_Loop != 0;
			}
		}

		public bool IsName(string name)
		{
			int num = Animator.StringToHash(name);
			return num == m_Name || num == m_Path;
		}

		public bool IsTag(string tag)
		{
			return Animator.StringToHash(tag) == m_Tag;
		}
	}
}
