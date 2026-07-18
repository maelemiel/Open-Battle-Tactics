using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public sealed class Display
	{
		public delegate void DisplaysUpdatedDelegate();

		internal IntPtr nativeDisplay;

		public static Display[] displays = new Display[1]
		{
			new Display()
		};

		private static Display _mainDisplay = displays[0];

		public int renderingWidth
		{
			get
			{
				int w = 0;
				int h = 0;
				GetRenderingExtImpl(nativeDisplay, out w, out h);
				return w;
			}
		}

		public int renderingHeight
		{
			get
			{
				int w = 0;
				int h = 0;
				GetRenderingExtImpl(nativeDisplay, out w, out h);
				return h;
			}
		}

		public int systemWidth
		{
			get
			{
				int w = 0;
				int h = 0;
				GetSystemExtImpl(nativeDisplay, out w, out h);
				return w;
			}
		}

		public int systemHeight
		{
			get
			{
				int w = 0;
				int h = 0;
				GetSystemExtImpl(nativeDisplay, out w, out h);
				return h;
			}
		}

		public RenderBuffer colorBuffer
		{
			get
			{
				RenderBuffer color;
				RenderBuffer depth;
				GetRenderingBuffersImpl(nativeDisplay, out color, out depth);
				return color;
			}
		}

		public RenderBuffer depthBuffer
		{
			get
			{
				RenderBuffer color;
				RenderBuffer depth;
				GetRenderingBuffersImpl(nativeDisplay, out color, out depth);
				return depth;
			}
		}

		public static Display main
		{
			get
			{
				return _mainDisplay;
			}
		}

		public static event DisplaysUpdatedDelegate onDisplaysUpdated;

		internal Display()
		{
			nativeDisplay = new IntPtr(0);
		}

		internal Display(IntPtr nativeDisplay)
		{
			this.nativeDisplay = nativeDisplay;
		}

		static Display()
		{
			Display.onDisplaysUpdated = null;
		}

		public void Activate()
		{
			ActivateDisplayImpl(nativeDisplay);
		}

		public void SetRenderingResolution(int w, int h)
		{
			SetRenderingResolutionImpl(nativeDisplay, w, h);
		}

		private static void RecreateDisplayList(IntPtr[] nativeDisplay)
		{
			displays = new Display[nativeDisplay.Length];
			for (int i = 0; i < nativeDisplay.Length; i++)
			{
				displays[i] = new Display(nativeDisplay[i]);
			}
			_mainDisplay = displays[0];
		}

		private static void FireDisplaysUpdated()
		{
			if (Display.onDisplaysUpdated != null)
			{
				Display.onDisplaysUpdated();
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void GetSystemExtImpl(IntPtr nativeDisplay, out int w, out int h);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void GetRenderingExtImpl(IntPtr nativeDisplay, out int w, out int h);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void GetRenderingBuffersImpl(IntPtr nativeDisplay, out RenderBuffer color, out RenderBuffer depth);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void SetRenderingResolutionImpl(IntPtr nativeDisplay, int w, int h);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern void ActivateDisplayImpl(IntPtr nativeDisplay);
	}
}
