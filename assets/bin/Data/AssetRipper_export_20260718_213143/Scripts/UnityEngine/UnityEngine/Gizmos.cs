using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	public sealed class Gizmos
	{
		public static Color color
		{
			get
			{
				Color value;
				INTERNAL_get_color(out value);
				return value;
			}
			set
			{
				INTERNAL_set_color(ref value);
			}
		}

		public static Matrix4x4 matrix
		{
			get
			{
				Matrix4x4 value;
				INTERNAL_get_matrix(out value);
				return value;
			}
			set
			{
				INTERNAL_set_matrix(ref value);
			}
		}

		public static void DrawRay(Ray r)
		{
			DrawLine(r.origin, r.origin + r.direction);
		}

		public static void DrawRay(Vector3 from, Vector3 direction)
		{
			DrawLine(from, from + direction);
		}

		public static void DrawLine(Vector3 from, Vector3 to)
		{
			INTERNAL_CALL_DrawLine(ref from, ref to);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_DrawLine(ref Vector3 from, ref Vector3 to);

		public static void DrawWireSphere(Vector3 center, float radius)
		{
			INTERNAL_CALL_DrawWireSphere(ref center, radius);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_DrawWireSphere(ref Vector3 center, float radius);

		public static void DrawSphere(Vector3 center, float radius)
		{
			INTERNAL_CALL_DrawSphere(ref center, radius);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_DrawSphere(ref Vector3 center, float radius);

		public static void DrawWireCube(Vector3 center, Vector3 size)
		{
			INTERNAL_CALL_DrawWireCube(ref center, ref size);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_DrawWireCube(ref Vector3 center, ref Vector3 size);

		public static void DrawCube(Vector3 center, Vector3 size)
		{
			INTERNAL_CALL_DrawCube(ref center, ref size);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_DrawCube(ref Vector3 center, ref Vector3 size);

		public static void DrawIcon(Vector3 center, string name, [DefaultValue("true")] bool allowScaling)
		{
			INTERNAL_CALL_DrawIcon(ref center, name, allowScaling);
		}

		[ExcludeFromDocs]
		public static void DrawIcon(Vector3 center, string name)
		{
			bool allowScaling = true;
			INTERNAL_CALL_DrawIcon(ref center, name, allowScaling);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_DrawIcon(ref Vector3 center, string name, bool allowScaling);

		[ExcludeFromDocs]
		public static void DrawGUITexture(Rect screenRect, Texture texture)
		{
			Material mat = null;
			DrawGUITexture(screenRect, texture, mat);
		}

		public static void DrawGUITexture(Rect screenRect, Texture texture, [DefaultValue("null")] Material mat)
		{
			DrawGUITexture(screenRect, texture, 0, 0, 0, 0, mat);
		}

		public static void DrawGUITexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder, [DefaultValue("null")] Material mat)
		{
			INTERNAL_CALL_DrawGUITexture(ref screenRect, texture, leftBorder, rightBorder, topBorder, bottomBorder, mat);
		}

		[ExcludeFromDocs]
		public static void DrawGUITexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder)
		{
			Material mat = null;
			INTERNAL_CALL_DrawGUITexture(ref screenRect, texture, leftBorder, rightBorder, topBorder, bottomBorder, mat);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_DrawGUITexture(ref Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Material mat);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_get_color(out Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_set_color(ref Color value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_get_matrix(out Matrix4x4 value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_set_matrix(ref Matrix4x4 value);

		public static void DrawFrustum(Vector3 center, float fov, float maxRange, float minRange, float aspect)
		{
			INTERNAL_CALL_DrawFrustum(ref center, fov, maxRange, minRange, aspect);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_DrawFrustum(ref Vector3 center, float fov, float maxRange, float minRange, float aspect);
	}
}
