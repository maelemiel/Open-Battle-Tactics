using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public struct CombineInstance
	{
		private int m_MeshInstanceID;

		private int m_SubMeshIndex;

		private Matrix4x4 m_Transform;

		public Mesh mesh
		{
			get
			{
				return InternalGetMesh(m_MeshInstanceID);
			}
			set
			{
				m_MeshInstanceID = ((value != null) ? value.GetInstanceID() : 0);
			}
		}

		public int subMeshIndex
		{
			get
			{
				return m_SubMeshIndex;
			}
			set
			{
				m_SubMeshIndex = value;
			}
		}

		public Matrix4x4 transform
		{
			get
			{
				return m_Transform;
			}
			set
			{
				m_Transform = value;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern Mesh InternalGetMesh(int instanceID);
	}
}
