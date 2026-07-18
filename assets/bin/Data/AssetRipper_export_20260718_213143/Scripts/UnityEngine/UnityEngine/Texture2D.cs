using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	public sealed class Texture2D : Texture
	{
		public extern int mipmapCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern TextureFormat format
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public Texture2D(int width, int height)
		{
			Internal_Create(this, width, height, TextureFormat.ARGB32, true, false, IntPtr.Zero);
		}

		public Texture2D(int width, int height, TextureFormat format, bool mipmap)
		{
			Internal_Create(this, width, height, format, mipmap, false, IntPtr.Zero);
		}

		public Texture2D(int width, int height, TextureFormat format, bool mipmap, bool linear)
		{
			Internal_Create(this, width, height, format, mipmap, linear, IntPtr.Zero);
		}

		internal Texture2D(int width, int height, TextureFormat format, bool mipmap, bool linear, IntPtr nativeTex)
		{
			Internal_Create(this, width, height, format, mipmap, linear, nativeTex);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_Create([Writable] Texture2D mono, int width, int height, TextureFormat format, bool mipmap, bool linear, IntPtr nativeTex);

		public static Texture2D CreateExternalTexture(int width, int height, TextureFormat format, bool mipmap, bool linear, IntPtr nativeTex)
		{
			return new Texture2D(width, height, format, mipmap, linear, nativeTex);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void UpdateExternalTexture(IntPtr nativeTex);

		public void SetPixel(int x, int y, Color color)
		{
			INTERNAL_CALL_SetPixel(this, x, y, ref color);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_SetPixel(Texture2D self, int x, int y, ref Color color);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern Color GetPixel(int x, int y);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern Color GetPixelBilinear(float u, float v);

		[ExcludeFromDocs]
		public void SetPixels(Color[] colors)
		{
			int miplevel = 0;
			SetPixels(colors, miplevel);
		}

		public void SetPixels(Color[] colors, [DefaultValue("0")] int miplevel)
		{
			int num = width >> miplevel;
			if (num < 1)
			{
				num = 1;
			}
			int num2 = height >> miplevel;
			if (num2 < 1)
			{
				num2 = 1;
			}
			SetPixels(0, 0, num, num2, colors, miplevel);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void SetPixels(int x, int y, int blockWidth, int blockHeight, Color[] colors, [DefaultValue("0")] int miplevel);

		[ExcludeFromDocs]
		public void SetPixels(int x, int y, int blockWidth, int blockHeight, Color[] colors)
		{
			int miplevel = 0;
			SetPixels(x, y, blockWidth, blockHeight, colors, miplevel);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void SetPixels32(Color32[] colors, [DefaultValue("0")] int miplevel);

		[ExcludeFromDocs]
		public void SetPixels32(Color32[] colors)
		{
			int miplevel = 0;
			SetPixels32(colors, miplevel);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern bool LoadImage(byte[] data);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void LoadRawTextureData(byte[] data);

		[ExcludeFromDocs]
		public Color[] GetPixels()
		{
			int miplevel = 0;
			return GetPixels(miplevel);
		}

		public Color[] GetPixels([DefaultValue("0")] int miplevel)
		{
			int num = width >> miplevel;
			if (num < 1)
			{
				num = 1;
			}
			int num2 = height >> miplevel;
			if (num2 < 1)
			{
				num2 = 1;
			}
			return GetPixels(0, 0, num, num2, miplevel);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern Color[] GetPixels(int x, int y, int blockWidth, int blockHeight, [DefaultValue("0")] int miplevel);

		[ExcludeFromDocs]
		public Color[] GetPixels(int x, int y, int blockWidth, int blockHeight)
		{
			int miplevel = 0;
			return GetPixels(x, y, blockWidth, blockHeight, miplevel);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern Color32[] GetPixels32([DefaultValue("0")] int miplevel);

		[ExcludeFromDocs]
		public Color32[] GetPixels32()
		{
			int miplevel = 0;
			return GetPixels32(miplevel);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void Apply([DefaultValue("true")] bool updateMipmaps, [DefaultValue("false")] bool makeNoLongerReadable);

		[ExcludeFromDocs]
		public void Apply(bool updateMipmaps)
		{
			bool makeNoLongerReadable = false;
			Apply(updateMipmaps, makeNoLongerReadable);
		}

		[ExcludeFromDocs]
		public void Apply()
		{
			bool makeNoLongerReadable = false;
			bool updateMipmaps = true;
			Apply(updateMipmaps, makeNoLongerReadable);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern bool Resize(int width, int height, TextureFormat format, bool hasMipMap);

		public bool Resize(int width, int height)
		{
			return Internal_ResizeWH(width, height);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern bool Internal_ResizeWH(int width, int height);

		public void Compress(bool highQuality)
		{
			INTERNAL_CALL_Compress(this, highQuality);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_Compress(Texture2D self, bool highQuality);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern Rect[] PackTextures(Texture2D[] textures, int padding, [DefaultValue("2048")] int maximumAtlasSize, [DefaultValue("false")] bool makeNoLongerReadable);

		[ExcludeFromDocs]
		public Rect[] PackTextures(Texture2D[] textures, int padding, int maximumAtlasSize)
		{
			bool makeNoLongerReadable = false;
			return PackTextures(textures, padding, maximumAtlasSize, makeNoLongerReadable);
		}

		[ExcludeFromDocs]
		public Rect[] PackTextures(Texture2D[] textures, int padding)
		{
			bool makeNoLongerReadable = false;
			int maximumAtlasSize = 2048;
			return PackTextures(textures, padding, maximumAtlasSize, makeNoLongerReadable);
		}

		public void ReadPixels(Rect source, int destX, int destY, [DefaultValue("true")] bool recalculateMipMaps)
		{
			INTERNAL_CALL_ReadPixels(this, ref source, destX, destY, recalculateMipMaps);
		}

		[ExcludeFromDocs]
		public void ReadPixels(Rect source, int destX, int destY)
		{
			bool recalculateMipMaps = true;
			INTERNAL_CALL_ReadPixels(this, ref source, destX, destY, recalculateMipMaps);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_ReadPixels(Texture2D self, ref Rect source, int destX, int destY, bool recalculateMipMaps);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern byte[] EncodeToPNG();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern byte[] EncodeToJPG(int quality);

		public byte[] EncodeToJPG()
		{
			return EncodeToJPG(75);
		}
	}
}
