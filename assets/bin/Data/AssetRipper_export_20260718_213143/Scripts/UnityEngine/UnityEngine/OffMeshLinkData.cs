using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public struct OffMeshLinkData
	{
		private int m_Valid;

		private int m_Activated;

		private int m_InstanceID;

		private OffMeshLinkType m_LinkType;

		private Vector3 m_StartPos;

		private Vector3 m_EndPos;

		public bool valid
		{
			get
			{
				return m_Valid != 0;
			}
		}

		public bool activated
		{
			get
			{
				return m_Activated != 0;
			}
		}

		public OffMeshLinkType linkType
		{
			get
			{
				return m_LinkType;
			}
		}

		public Vector3 startPos
		{
			get
			{
				return m_StartPos;
			}
		}

		public Vector3 endPos
		{
			get
			{
				return m_EndPos;
			}
		}

		public OffMeshLink offMeshLink
		{
			get
			{
				return GetOffMeshLinkInternal(m_InstanceID);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		internal extern OffMeshLink GetOffMeshLinkInternal(int instanceID);
	}
}
