using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class ProceduralTexture : Texture
	{
		public extern bool hasAlpha
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

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern ProceduralOutputType GetProceduralOutputType();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		internal extern ProceduralMaterial GetProceduralMaterial();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		internal extern bool HasBeenGenerated();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern Color32[] GetPixels32(int x, int y, int blockWidth, int blockHeight);
	}
}
