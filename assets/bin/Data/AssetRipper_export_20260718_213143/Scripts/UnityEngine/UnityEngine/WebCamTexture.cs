using System.Runtime.CompilerServices;
using UnityEngine.Internal;

namespace UnityEngine
{
	public sealed class WebCamTexture : Texture
	{
		public extern bool isPlaying
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern string deviceName
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern float requestedFPS
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern int requestedWidth
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern int requestedHeight
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public static extern WebCamDevice[] devices
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern int videoRotationAngle
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern bool videoVerticallyMirrored
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern bool didUpdateThisFrame
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public WebCamTexture(string deviceName, int requestedWidth, int requestedHeight, int requestedFPS)
		{
			Internal_CreateWebCamTexture(this, deviceName, requestedWidth, requestedHeight, requestedFPS);
		}

		public WebCamTexture(string deviceName, int requestedWidth, int requestedHeight)
		{
			Internal_CreateWebCamTexture(this, deviceName, requestedWidth, requestedHeight, 0);
		}

		public WebCamTexture(string deviceName)
		{
			Internal_CreateWebCamTexture(this, deviceName, 0, 0, 0);
		}

		public WebCamTexture(int requestedWidth, int requestedHeight, int requestedFPS)
		{
			Internal_CreateWebCamTexture(this, string.Empty, requestedWidth, requestedHeight, requestedFPS);
		}

		public WebCamTexture(int requestedWidth, int requestedHeight)
		{
			Internal_CreateWebCamTexture(this, string.Empty, requestedWidth, requestedHeight, 0);
		}

		public WebCamTexture()
		{
			Internal_CreateWebCamTexture(this, string.Empty, 0, 0, 0);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void Internal_CreateWebCamTexture([Writable] WebCamTexture self, string scriptingDevice, int requestedWidth, int requestedHeight, int maxFramerate);

		public void Play()
		{
			INTERNAL_CALL_Play(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_Play(WebCamTexture self);

		public void Pause()
		{
			INTERNAL_CALL_Pause(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_Pause(WebCamTexture self);

		public void Stop()
		{
			INTERNAL_CALL_Stop(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void INTERNAL_CALL_Stop(WebCamTexture self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern Color GetPixel(int x, int y);

		public Color[] GetPixels()
		{
			return GetPixels(0, 0, width, height);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern Color[] GetPixels(int x, int y, int blockWidth, int blockHeight);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		public extern Color32[] GetPixels32([DefaultValue("null")] Color32[] colors);

		[ExcludeFromDocs]
		public Color32[] GetPixels32()
		{
			Color32[] colors = null;
			return GetPixels32(colors);
		}
	}
}
