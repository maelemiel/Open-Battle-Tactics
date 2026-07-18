using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UnityEngine
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class RectOffset
	{
		[NonSerialized]
		[NotRenamed]
		internal IntPtr m_Ptr;

		private GUIStyle m_SourceStyle;

		public extern int left
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern int right
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern int top
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern int bottom
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			set;
		}

		public extern int horizontal
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public extern int vertical
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			[WrapperlessIcall]
			get;
		}

		public RectOffset()
		{
			Init();
		}

		internal RectOffset(GUIStyle sourceStyle, IntPtr source)
		{
			m_SourceStyle = sourceStyle;
			m_Ptr = source;
		}

		public RectOffset(int left, int right, int top, int bottom)
		{
			Init();
			this.left = left;
			this.right = right;
			this.top = top;
			this.bottom = bottom;
		}

		~RectOffset()
		{
			if (m_SourceStyle == null)
			{
				Cleanup();
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Init();

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private extern void Cleanup();

		public Rect Add(Rect rect)
		{
			return INTERNAL_CALL_Add(this, ref rect);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern Rect INTERNAL_CALL_Add(RectOffset self, ref Rect rect);

		public Rect Remove(Rect rect)
		{
			return INTERNAL_CALL_Remove(this, ref rect);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[WrapperlessIcall]
		private static extern Rect INTERNAL_CALL_Remove(RectOffset self, ref Rect rect);

		public override string ToString()
		{
			return UnityString.Format("RectOffset (l:{0} r:{1} t:{2} b:{3})", left, right, top, bottom);
		}
	}
}
