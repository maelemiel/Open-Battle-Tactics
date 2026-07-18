using System.Runtime.CompilerServices;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[MonoTODO("Struct should be [StructLayout(LayoutKind.Sequential)] but will need to be reordered for that.")]
	public struct GCHandle
	{
		private int handle;

		public bool IsAllocated
		{
			get
			{
				return handle != 0;
			}
		}

		public object Target
		{
			get
			{
				if (!IsAllocated)
				{
					throw new InvalidOperationException(Locale.GetText("Handle is not allocated"));
				}
				return GetTarget(handle);
			}
			set
			{
				handle = GetTargetHandle(value, handle, (GCHandleType)(-1));
			}
		}

		private GCHandle(IntPtr h)
		{
			handle = (int)h;
		}

		private GCHandle(object obj)
			: this(obj, GCHandleType.Normal)
		{
		}

		private GCHandle(object value, GCHandleType type)
		{
			if (type < GCHandleType.Weak || type > GCHandleType.Pinned)
			{
				type = GCHandleType.Normal;
			}
			handle = GetTargetHandle(value, 0, type);
		}

		public IntPtr AddrOfPinnedObject()
		{
			IntPtr addrOfPinnedObject = GetAddrOfPinnedObject(handle);
			if (addrOfPinnedObject == (IntPtr)(-1))
			{
				throw new ArgumentException("Object contains non-primitive or non-blittable data.");
			}
			if (addrOfPinnedObject == (IntPtr)(-2))
			{
				throw new InvalidOperationException("Handle is not pinned.");
			}
			return addrOfPinnedObject;
		}

		public static GCHandle Alloc(object value)
		{
			return new GCHandle(value);
		}

		public static GCHandle Alloc(object value, GCHandleType type)
		{
			return new GCHandle(value, type);
		}

		public void Free()
		{
			FreeHandle(handle);
			handle = 0;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CheckCurrentDomain(int handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern object GetTarget(int handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetTargetHandle(object obj, int handle, GCHandleType type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void FreeHandle(int handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetAddrOfPinnedObject(int handle);

		public override bool Equals(object o)
		{
			if (o == null || !(o is GCHandle))
			{
				return false;
			}
			return handle == ((GCHandle)o).handle;
		}

		public override int GetHashCode()
		{
			return handle.GetHashCode();
		}

		public static GCHandle FromIntPtr(IntPtr value)
		{
			return (GCHandle)value;
		}

		public static IntPtr ToIntPtr(GCHandle value)
		{
			return (IntPtr)value;
		}

		public static explicit operator IntPtr(GCHandle value)
		{
			return (IntPtr)value.handle;
		}

		public static explicit operator GCHandle(IntPtr value)
		{
			if (value == IntPtr.Zero)
			{
				throw new ArgumentException("GCHandle value cannot be zero");
			}
			if (!CheckCurrentDomain((int)value))
			{
				throw new ArgumentException("GCHandle value belongs to a different domain");
			}
			return new GCHandle(value);
		}

		public static bool operator ==(GCHandle a, GCHandle b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(GCHandle a, GCHandle b)
		{
			return !a.Equals(b);
		}
	}
}
