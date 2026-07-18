namespace UnityEngine
{
	public struct WebCamDevice
	{
		internal string m_Name;

		internal int m_Flags;

		public string name
		{
			get
			{
				return m_Name;
			}
		}

		public bool isFrontFacing
		{
			get
			{
				return (m_Flags & 1) == 1;
			}
		}
	}
}
