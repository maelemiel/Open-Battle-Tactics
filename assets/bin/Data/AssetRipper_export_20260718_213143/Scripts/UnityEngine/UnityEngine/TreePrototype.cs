using System.Runtime.InteropServices;

namespace UnityEngine
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class TreePrototype
	{
		internal GameObject m_Prefab;

		internal float m_BendFactor;

		public GameObject prefab
		{
			get
			{
				return m_Prefab;
			}
			set
			{
				m_Prefab = value;
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
	}
}
