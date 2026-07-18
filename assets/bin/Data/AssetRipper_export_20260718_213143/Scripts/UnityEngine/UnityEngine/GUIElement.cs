using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	public class GUIElement : Behaviour
	{
		public bool HitTest(Vector3 screenPosition, [DefaultValue("null")] Camera camera)
		{
			return INTERNAL_CALL_HitTest(this, ref screenPosition, camera);
		}

		[ExcludeFromDocs]
		public bool HitTest(Vector3 screenPosition)
		{
			Camera camera = null;
			return INTERNAL_CALL_HitTest(this, ref screenPosition, camera);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern bool INTERNAL_CALL_HitTest(GUIElement self, ref Vector3 screenPosition, Camera camera);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern Rect GetScreenRect([DefaultValue("null")] Camera camera);

		[ExcludeFromDocs]
		public Rect GetScreenRect()
		{
			Camera camera = null;
			return GetScreenRect(camera);
		}
	}
}
