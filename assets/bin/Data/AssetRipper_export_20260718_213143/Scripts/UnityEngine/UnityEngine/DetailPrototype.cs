using System.Runtime.InteropServices;

namespace UnityEngine
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class DetailPrototype
	{
		private GameObject m_Prototype;

		private Texture2D m_PrototypeTexture;

		private Color m_HealthyColor = new Color(0.2627451f, 83f / 85f, 14f / 85f, 1f);

		private Color m_DryColor = new Color(41f / 51f, 0.7372549f, 0.101960786f, 1f);

		private float m_MinWidth = 1f;

		private float m_MaxWidth = 2f;

		private float m_MinHeight = 1f;

		private float m_MaxHeight = 2f;

		private float m_NoiseSpread = 0.1f;

		private float m_BendFactor = 0.1f;

		private int m_RenderMode = 2;

		private int m_UsePrototypeMesh;

		public GameObject prototype
		{
			get
			{
				return m_Prototype;
			}
			set
			{
				m_Prototype = value;
			}
		}

		public Texture2D prototypeTexture
		{
			get
			{
				return m_PrototypeTexture;
			}
			set
			{
				m_PrototypeTexture = value;
			}
		}

		public float minWidth
		{
			get
			{
				return m_MinWidth;
			}
			set
			{
				m_MinWidth = value;
			}
		}

		public float maxWidth
		{
			get
			{
				return m_MaxWidth;
			}
			set
			{
				m_MaxWidth = value;
			}
		}

		public float minHeight
		{
			get
			{
				return m_MinHeight;
			}
			set
			{
				m_MinHeight = value;
			}
		}

		public float maxHeight
		{
			get
			{
				return m_MaxHeight;
			}
			set
			{
				m_MaxHeight = value;
			}
		}

		public float noiseSpread
		{
			get
			{
				return m_NoiseSpread;
			}
			set
			{
				m_NoiseSpread = value;
			}
		}

		public float bendFactor
		{
			get
			{
				return m_BendFactor;
			}
			set
			{
				m_BendFactor = value;
			}
		}

		public Color healthyColor
		{
			get
			{
				return m_HealthyColor;
			}
			set
			{
				m_HealthyColor = value;
			}
		}

		public Color dryColor
		{
			get
			{
				return m_DryColor;
			}
			set
			{
				m_DryColor = value;
			}
		}

		public DetailRenderMode renderMode
		{
			get
			{
				return (DetailRenderMode)m_RenderMode;
			}
			set
			{
				m_RenderMode = (int)value;
			}
		}

		public bool usePrototypeMesh
		{
			get
			{
				return m_UsePrototypeMesh != 0;
			}
			set
			{
				m_UsePrototypeMesh = (value ? 1 : 0);
			}
		}
	}
}
