using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	public sealed class RenderTexture : Texture
	{
		public override int width
		{
			get
			{
				return Internal_GetWidth(this);
			}
			set
			{
				Internal_SetWidth(this, value);
			}
		}

		public override int height
		{
			get
			{
				return Internal_GetHeight(this);
			}
			set
			{
				Internal_SetHeight(this, value);
			}
		}

		public extern int depth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern bool isPowerOfTwo
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern bool sRGB
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern RenderTextureFormat format
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern bool useMipMap
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern bool generateMips
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern bool isCubemap
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern bool isVolume
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern int volumeDepth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern int antiAliasing
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern bool enableRandomWrite
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public RenderBuffer colorBuffer
		{
			get
			{
				RenderBuffer res;
				GetColorBuffer(out res);
				return res;
			}
		}

		public RenderBuffer depthBuffer
		{
			get
			{
				RenderBuffer res;
				GetDepthBuffer(out res);
				return res;
			}
		}

		public static extern RenderTexture active
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		[Obsolete("Use SystemInfo.supportsRenderTextures instead.")]
		public static extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public RenderTexture(int width, int height, int depth, RenderTextureFormat format, RenderTextureReadWrite readWrite)
		{
			Internal_CreateRenderTexture(this);
			this.width = width;
			this.height = height;
			this.depth = depth;
			this.format = format;
			bool flag = readWrite == RenderTextureReadWrite.sRGB;
			if (readWrite == RenderTextureReadWrite.Default)
			{
				flag = QualitySettings.activeColorSpace == ColorSpace.Linear;
			}
			Internal_SetSRGBReadWrite(this, flag);
		}

		public RenderTexture(int width, int height, int depth, RenderTextureFormat format)
		{
			Internal_CreateRenderTexture(this);
			this.width = width;
			this.height = height;
			this.depth = depth;
			this.format = format;
			Internal_SetSRGBReadWrite(this, QualitySettings.activeColorSpace == ColorSpace.Linear);
		}

		public RenderTexture(int width, int height, int depth)
		{
			Internal_CreateRenderTexture(this);
			this.width = width;
			this.height = height;
			this.depth = depth;
			format = RenderTextureFormat.Default;
			Internal_SetSRGBReadWrite(this, QualitySettings.activeColorSpace == ColorSpace.Linear);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_CreateRenderTexture([Writable] RenderTexture rt);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern RenderTexture GetTemporary(int width, int height, [DefaultValue("0")] int depthBuffer, [DefaultValue("RenderTextureFormat.Default")] RenderTextureFormat format, [DefaultValue("RenderTextureReadWrite.Default")] RenderTextureReadWrite readWrite, [DefaultValue("1")] int antiAliasing);

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format, RenderTextureReadWrite readWrite)
		{
			int num = 1;
			return GetTemporary(width, height, depthBuffer, format, readWrite, num);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format)
		{
			int num = 1;
			RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default;
			return GetTemporary(width, height, depthBuffer, format, readWrite, num);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height, int depthBuffer)
		{
			int num = 1;
			RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default;
			RenderTextureFormat renderTextureFormat = RenderTextureFormat.Default;
			return GetTemporary(width, height, depthBuffer, renderTextureFormat, readWrite, num);
		}

		[ExcludeFromDocs]
		public static RenderTexture GetTemporary(int width, int height)
		{
			int num = 1;
			RenderTextureReadWrite readWrite = RenderTextureReadWrite.Default;
			RenderTextureFormat renderTextureFormat = RenderTextureFormat.Default;
			int num2 = 0;
			return GetTemporary(width, height, num2, renderTextureFormat, readWrite, num);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern void ReleaseTemporary(RenderTexture temp);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern int Internal_GetWidth(RenderTexture mono);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_SetWidth(RenderTexture mono, int width);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern int Internal_GetHeight(RenderTexture mono);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_SetHeight(RenderTexture mono, int width);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_SetSRGBReadWrite(RenderTexture mono, bool sRGB);

		public bool Create()
		{
			return INTERNAL_CALL_Create(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern bool INTERNAL_CALL_Create(RenderTexture self);

		public void Release()
		{
			INTERNAL_CALL_Release(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_Release(RenderTexture self);

		public bool IsCreated()
		{
			return INTERNAL_CALL_IsCreated(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern bool INTERNAL_CALL_IsCreated(RenderTexture self);

		public void DiscardContents()
		{
			INTERNAL_CALL_DiscardContents(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_DiscardContents(RenderTexture self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void DiscardContents(bool discardColor, bool discardDepth);

		public void MarkRestoreExpected()
		{
			INTERNAL_CALL_MarkRestoreExpected(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_MarkRestoreExpected(RenderTexture self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void GetColorBuffer(out RenderBuffer res);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void GetDepthBuffer(out RenderBuffer res);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern void SetGlobalShaderProperty(string propertyName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_GetTexelOffset(RenderTexture tex, out Vector2 output);

		public Vector2 GetTexelOffset()
		{
			Vector2 output;
			Internal_GetTexelOffset(this, out output);
			return output;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public static extern bool SupportsStencil(RenderTexture rt);

		[Obsolete("RenderTexture.SetBorderColor was removed", true)]
		public void SetBorderColor(Color color)
		{
		}
	}
}
