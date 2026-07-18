namespace UnityEngine
{
	public struct WheelHit
	{
		private Vector3 m_Point;

		private Vector3 m_Normal;

		private Vector3 m_ForwardDir;

		private Vector3 m_SidewaysDir;

		private float m_Force;

		private float m_ForwardSlip;

		private float m_SidewaysSlip;

		private Collider m_Collider;

		public Collider collider
		{
			get
			{
				return m_Collider;
			}
			set
			{
				m_Collider = value;
			}
		}

		public Vector3 point
		{
			get
			{
				return m_Point;
			}
			set
			{
				m_Point = value;
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

		public Vector3 forwardDir
		{
			get
			{
				return m_ForwardDir;
			}
			set
			{
				m_ForwardDir = value;
			}
		}

		public Vector3 sidewaysDir
		{
			get
			{
				return m_SidewaysDir;
			}
			set
			{
				m_SidewaysDir = value;
			}
		}

		public float force
		{
			get
			{
				return m_Force;
			}
			set
			{
				m_Force = value;
			}
		}

		public float forwardSlip
		{
			get
			{
				return m_ForwardSlip;
			}
			set
			{
				m_Force = m_ForwardSlip;
			}
		}

		public float sidewaysSlip
		{
			get
			{
				return m_SidewaysSlip;
			}
			set
			{
				m_SidewaysSlip = value;
			}
		}
	}
}
