namespace UnityEngine
{
	public struct AnimatorTransitionInfo
	{
		private int m_Name;

		private int m_UserName;

		private float m_NormalizedTime;

		public int nameHash
		{
			get
			{
				return m_Name;
			}
		}

		public int userNameHash
		{
			get
			{
				return m_UserName;
			}
		}

		public float normalizedTime
		{
			get
			{
				return m_NormalizedTime;
			}
		}

		public bool IsName(string name)
		{
			return Animator.StringToHash(name) == m_Name;
		}

		public bool IsUserName(string name)
		{
			return Animator.StringToHash(name) == m_UserName;
		}
	}
}
