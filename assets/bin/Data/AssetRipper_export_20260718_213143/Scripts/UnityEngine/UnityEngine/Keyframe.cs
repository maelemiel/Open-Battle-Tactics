namespace UnityEngine
{
	public struct Keyframe
	{
		private float m_Time;

		private float m_Value;

		private float m_InTangent;

		private float m_OutTangent;

		public float time
		{
			get
			{
				return m_Time;
			}
			set
			{
				m_Time = value;
			}
		}

		public float value
		{
			get
			{
				return m_Value;
			}
			set
			{
				m_Value = value;
			}
		}

		public float inTangent
		{
			get
			{
				return m_InTangent;
			}
			set
			{
				m_InTangent = value;
			}
		}

		public float outTangent
		{
			get
			{
				return m_OutTangent;
			}
			set
			{
				m_OutTangent = value;
			}
		}

		public int tangentMode
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		public Keyframe(float time, float value)
		{
			m_Time = time;
			m_Value = value;
			m_InTangent = 0f;
			m_OutTangent = 0f;
		}

		public Keyframe(float time, float value, float inTangent, float outTangent)
		{
			m_Time = time;
			m_Value = value;
			m_InTangent = inTangent;
			m_OutTangent = outTangent;
		}
	}
}
