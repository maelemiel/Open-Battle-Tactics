using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class Cursor
	{
		private static void SetCursor(Texture2D texture, CursorMode cursorMode)
		{
			SetCursor(texture, Vector2.zero, cursorMode);
		}

		public static void SetCursor(Texture2D texture, Vector2 hotspot, CursorMode cursorMode)
		{
			INTERNAL_CALL_SetCursor(texture, ref hotspot, cursorMode);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_SetCursor(Texture2D texture, ref Vector2 hotspot, CursorMode cursorMode);
	}
}
