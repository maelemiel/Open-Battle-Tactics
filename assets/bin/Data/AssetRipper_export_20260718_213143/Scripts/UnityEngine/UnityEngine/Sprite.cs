using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	public sealed class Sprite : Object
	{
		public extern Bounds bounds
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern Rect rect
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern Texture2D texture
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern Rect textureRect
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public Vector2 textureRectOffset
		{
			get
			{
				Vector2 output;
				Internal_GetTextureRectOffset(this, out output);
				return output;
			}
		}

		public extern bool packed
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern SpritePackingMode packingMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern SpritePackingRotation packingRotation
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern Vector4 border
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, [DefaultValue("100.0f")] float pixelsToUnits, [DefaultValue("0")] uint extrude, [DefaultValue("SpriteMeshType.Tight")] SpriteMeshType meshType, [DefaultValue("Vector4.zero")] Vector4 border)
		{
			return INTERNAL_CALL_Create(texture, ref rect, ref pivot, pixelsToUnits, extrude, meshType, ref border);
		}

		[ExcludeFromDocs]
		public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsToUnits, uint extrude, SpriteMeshType meshType)
		{
			Vector4 zero = Vector4.zero;
			return INTERNAL_CALL_Create(texture, ref rect, ref pivot, pixelsToUnits, extrude, meshType, ref zero);
		}

		[ExcludeFromDocs]
		public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsToUnits, uint extrude)
		{
			Vector4 zero = Vector4.zero;
			SpriteMeshType meshType = SpriteMeshType.Tight;
			return INTERNAL_CALL_Create(texture, ref rect, ref pivot, pixelsToUnits, extrude, meshType, ref zero);
		}

		[ExcludeFromDocs]
		public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot, float pixelsToUnits)
		{
			Vector4 zero = Vector4.zero;
			SpriteMeshType meshType = SpriteMeshType.Tight;
			uint extrude = 0u;
			return INTERNAL_CALL_Create(texture, ref rect, ref pivot, pixelsToUnits, extrude, meshType, ref zero);
		}

		[ExcludeFromDocs]
		public static Sprite Create(Texture2D texture, Rect rect, Vector2 pivot)
		{
			Vector4 zero = Vector4.zero;
			SpriteMeshType meshType = SpriteMeshType.Tight;
			uint extrude = 0u;
			float pixelsToUnits = 100f;
			return INTERNAL_CALL_Create(texture, ref rect, ref pivot, pixelsToUnits, extrude, meshType, ref zero);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern Sprite INTERNAL_CALL_Create(Texture2D texture, ref Rect rect, ref Vector2 pivot, float pixelsToUnits, uint extrude, SpriteMeshType meshType, ref Vector4 border);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_GetTextureRectOffset(Sprite sprite, out Vector2 output);
	}
}
