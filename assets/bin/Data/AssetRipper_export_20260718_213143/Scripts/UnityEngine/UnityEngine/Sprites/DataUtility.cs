using System.Runtime.CompilerServices;

namespace UnityEngine.Sprites
{
	public sealed class DataUtility
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern Vector4 GetInnerUV(Sprite sprite);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern Vector4 GetOuterUV(Sprite sprite);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern Vector4 GetPadding(Sprite sprite);

		public static Vector2 GetMinSize(Sprite sprite)
		{
			Vector2 output;
			Internal_GetMinSize(sprite, out output);
			return output;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_GetMinSize(Sprite sprite, out Vector2 output);
	}
}
