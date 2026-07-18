using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class SparseTexture : Texture
	{
		public extern int tileWidth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern int tileHeight
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern bool isCreated
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public SparseTexture(int width, int height, TextureFormat format, int mipCount)
		{
			Internal_Create(this, width, height, format, mipCount, false);
		}

		public SparseTexture(int width, int height, TextureFormat format, int mipCount, bool linear)
		{
			Internal_Create(this, width, height, format, mipCount, linear);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_Create([Writable] SparseTexture mono, int width, int height, TextureFormat format, int mipCount, bool linear);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void UpdateTile(int tileX, int tileY, int miplevel, Color32[] data);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void UpdateTileRaw(int tileX, int tileY, int miplevel, byte[] data);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void UnloadTile(int tileX, int tileY, int miplevel);
	}
}
