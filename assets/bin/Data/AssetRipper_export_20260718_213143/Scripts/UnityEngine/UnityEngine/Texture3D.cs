using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	public sealed class Texture3D : Texture
	{
		public extern int depth
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

		public Texture3D(int width, int height, int depth, TextureFormat format, bool mipmap)
		{
			Internal_Create(this, width, height, depth, format, mipmap);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern Color[] GetPixels([DefaultValue("0")] int miplevel);

		[ExcludeFromDocs]
		public Color[] GetPixels()
		{
			int miplevel = 0;
			return GetPixels(miplevel);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void SetPixels(Color[] colors, [DefaultValue("0")] int miplevel);

		[ExcludeFromDocs]
		public void SetPixels(Color[] colors)
		{
			int miplevel = 0;
			SetPixels(colors, miplevel);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void Apply([DefaultValue("true")] bool updateMipmaps);

		[ExcludeFromDocs]
		public void Apply()
		{
			bool updateMipmaps = true;
			Apply(updateMipmaps);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_Create([Writable] Texture3D mono, int width, int height, int depth, TextureFormat format, bool mipmap);
	}
}
