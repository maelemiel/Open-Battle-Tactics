using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class AvatarBuilder
	{
		public static Avatar BuildHumanAvatar(GameObject go, HumanDescription monoHumanDescription)
		{
			return INTERNAL_CALL_BuildHumanAvatar(go, ref monoHumanDescription);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern Avatar INTERNAL_CALL_BuildHumanAvatar(GameObject go, ref HumanDescription monoHumanDescription);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern Avatar BuildGenericAvatar(GameObject go, string rootMotionTransformName);
	}
}
