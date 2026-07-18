namespace UnityEngine
{
	public struct NavMeshHit
	{
		private Vector3 m_Position;

		private Vector3 m_Normal;

		private float m_Distance;

		private int m_Mask;

		private int m_Hit;

		public Vector3 position
		{
			get
			{
				return m_Position;
			}
			set
			{
				m_Position = value;
			}
		}

		public Vector3 normal
		{
			get
			{
				return m_Normal;
			}
			set
			{
				m_Normal = value;
			}
		}

		public float distance
		{
			get
			{
				return m_Distance;
			}
			set
			{
				m_Distance = value;
			}
		}

		public int mask
		{
			get
			{
				return m_Mask;
			}
			set
			{
				m_Mask = value;
			}
		}

		public bool hit
		{
			get
			{
				return m_Hit != 0;
			}
			set
			{
				m_Hit = (value ? 1 : 0);
			}
		}
	}
}
