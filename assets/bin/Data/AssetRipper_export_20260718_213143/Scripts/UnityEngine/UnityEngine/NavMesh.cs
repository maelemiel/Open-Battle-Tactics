using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class NavMesh : Object
	{
		public static bool Raycast(Vector3 sourcePosition, Vector3 targetPosition, out NavMeshHit hit, int passableMask)
		{
			return INTERNAL_CALL_Raycast(ref sourcePosition, ref targetPosition, out hit, passableMask);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern bool INTERNAL_CALL_Raycast(ref Vector3 sourcePosition, ref Vector3 targetPosition, out NavMeshHit hit, int passableMask);

		public static bool CalculatePath(Vector3 sourcePosition, Vector3 targetPosition, int passableMask, NavMeshPath path)
		{
			path.ClearCorners();
			return CalculatePathInternal(sourcePosition, targetPosition, passableMask, path);
		}

		internal static bool CalculatePathInternal(Vector3 sourcePosition, Vector3 targetPosition, int passableMask, NavMeshPath path)
		{
			return INTERNAL_CALL_CalculatePathInternal(ref sourcePosition, ref targetPosition, passableMask, path);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern bool INTERNAL_CALL_CalculatePathInternal(ref Vector3 sourcePosition, ref Vector3 targetPosition, int passableMask, NavMeshPath path);

		public static bool FindClosestEdge(Vector3 sourcePosition, out NavMeshHit hit, int passableMask)
		{
			return INTERNAL_CALL_FindClosestEdge(ref sourcePosition, out hit, passableMask);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern bool INTERNAL_CALL_FindClosestEdge(ref Vector3 sourcePosition, out NavMeshHit hit, int passableMask);

		public static bool SamplePosition(Vector3 sourcePosition, out NavMeshHit hit, float maxDistance, int allowedMask)
		{
			return INTERNAL_CALL_SamplePosition(ref sourcePosition, out hit, maxDistance, allowedMask);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern bool INTERNAL_CALL_SamplePosition(ref Vector3 sourcePosition, out NavMeshHit hit, float maxDistance, int allowedMask);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void SetLayerCost(int layer, float cost);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern float GetLayerCost(int layer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern int GetNavMeshLayerFromName(string layerName);

		public static NavMeshTriangulation CalculateTriangulation()
		{
			NavMeshTriangulation result = default(NavMeshTriangulation);
			TriangulateInternal(ref result.vertices, ref result.indices, ref result.layers);
			return result;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		internal static extern void TriangulateInternal(ref Vector3[] vertices, ref int[] indices, ref int[] layers);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		[Obsolete("use NavMesh.CalculateTriangulation() instead.")]
		public static extern void Triangulate(out Vector3[] vertices, out int[] indices);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void AddOffMeshLinks();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void RestoreNavMesh();
	}
}
