using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class AnimatorUtility
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void OptimizeTransformHierarchy(GameObject go, string[] exposedTransforms);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void DeoptimizeTransformHierarchy(GameObject go);
	}
}
