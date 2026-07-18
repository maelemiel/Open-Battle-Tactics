namespace UnityEngine
{
	public struct TreeInstance
	{
		private Vector3 m_Position;

		private float m_WidthScale;

		private float m_HeightScale;

		private Color32 m_Color;

		private Color32 m_LightmapColor;

		private int m_Index;

		private float m_TemporaryDistance;

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

		public float widthScale
		{
			get
			{
				return m_WidthScale;
			}
			set
			{
				m_WidthScale = value;
			}
		}

		public float heightScale
		{
			get
			{
				return m_HeightScale;
			}
			set
			{
				m_HeightScale = value;
			}
		}

		public Color color
		{
			get
			{
				return m_Color;
			}
			set
			{
				m_Color = value;
			}
		}

		public Color lightmapColor
		{
			get
			{
				return m_LightmapColor;
			}
			set
			{
				m_LightmapColor = value;
			}
		}

		public int prototypeIndex
		{
			get
			{
				return m_Index;
			}
			set
			{
				m_Index = value;
			}
		}

		internal float temporaryDistance
		{
			get
			{
				return m_TemporaryDistance;
			}
			set
			{
				m_TemporaryDistance = value;
			}
		}
	}
}
