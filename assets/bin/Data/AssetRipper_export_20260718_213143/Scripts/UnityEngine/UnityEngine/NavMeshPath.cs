using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnityEngine
{
	[StructLayout(LayoutKind.Sequential)]
	public sealed class NavMeshPath
	{
		internal IntPtr m_Ptr;

		internal Vector3[] m_corners;

		public Vector3[] corners
		{
			get
			{
				CalculateCorners();
				return m_corners;
			}
		}

		public extern NavMeshPathStatus status
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern NavMeshPath();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void DestroyNavMeshPath();

		~NavMeshPath()
		{
			DestroyNavMeshPath();
			m_Ptr = IntPtr.Zero;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern Vector3[] CalculateCornersInternal();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void ClearCornersInternal();

		public void ClearCorners()
		{
			ClearCornersInternal();
			m_corners = null;
		}

		private void CalculateCorners()
		{
			if (m_corners == null)
			{
				m_corners = CalculateCornersInternal();
			}
		}
	}
}
