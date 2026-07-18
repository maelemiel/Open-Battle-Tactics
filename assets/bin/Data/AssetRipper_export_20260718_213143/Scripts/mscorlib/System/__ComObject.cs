using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Mono.Interop;

namespace System
{
	internal class __ComObject : MarshalByRefObject
	{
		private IntPtr iunknown;

		private IntPtr hash_table;

		internal IntPtr IUnknown
		{
			get
			{
				if (iunknown == IntPtr.Zero)
				{
					throw new InvalidComObjectException("COM object that has been separated from its underlying RCW cannot be used.");
				}
				return iunknown;
			}
		}

		internal IntPtr IDispatch
		{
			get
			{
				IntPtr intPtr = GetInterface(typeof(IDispatch));
				if (intPtr == IntPtr.Zero)
				{
					throw new InvalidComObjectException("COM object that has been separated from its underlying RCW cannot be used.");
				}
				return intPtr;
			}
		}

		internal static Guid IID_IUnknown
		{
			get
			{
				return new Guid("00000000-0000-0000-C000-000000000046");
			}
		}

		internal static Guid IID_IDispatch
		{
			get
			{
				return new Guid("00020400-0000-0000-C000-000000000046");
			}
		}

		public __ComObject()
		{
			Initialize(GetType());
		}

		internal __ComObject(Type t)
		{
			Initialize(t);
		}

		internal __ComObject(IntPtr pItf)
		{
			Guid iid = IID_IUnknown;
			int errorCode = Marshal.QueryInterface(pItf, ref iid, out iunknown);
			Marshal.ThrowExceptionForHR(errorCode);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern __ComObject CreateRCW(Type t);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ReleaseInterfaces();

		~__ComObject()
		{
			ReleaseInterfaces();
		}

		internal void Initialize(Type t)
		{
			if (iunknown != IntPtr.Zero)
			{
				return;
			}
			ObjectCreationDelegate objectCreationCallback = ExtensibleClassFactory.GetObjectCreationCallback(t);
			if (objectCreationCallback != null)
			{
				iunknown = objectCreationCallback(IntPtr.Zero);
				if (iunknown == IntPtr.Zero)
				{
					throw new COMException(string.Format("ObjectCreationDelegate for type {0} failed to return a valid COM object", t));
				}
			}
			else
			{
				int errorCode = CoCreateInstance(GetCLSID(t), IntPtr.Zero, 21u, IID_IUnknown, out iunknown);
				Marshal.ThrowExceptionForHR(errorCode);
			}
		}

		private static Guid GetCLSID(Type t)
		{
			if (t.IsImport)
			{
				return t.GUID;
			}
			for (Type baseType = t.BaseType; baseType != typeof(object); baseType = baseType.BaseType)
			{
				if (baseType.IsImport)
				{
					return baseType.GUID;
				}
			}
			throw new COMException("Could not find base COM type for type " + t.ToString());
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern IntPtr GetInterfaceInternal(Type t, bool throwException);

		internal IntPtr GetInterface(Type t, bool throwException)
		{
			CheckIUnknown();
			return GetInterfaceInternal(t, throwException);
		}

		internal IntPtr GetInterface(Type t)
		{
			return GetInterface(t, true);
		}

		private void CheckIUnknown()
		{
			if (iunknown == IntPtr.Zero)
			{
				throw new InvalidComObjectException("COM object that has been separated from its underlying RCW cannot be used.");
			}
		}

		public override bool Equals(object obj)
		{
			CheckIUnknown();
			if (obj == null)
			{
				return false;
			}
			__ComObject _ComObject = obj as __ComObject;
			if (_ComObject == null)
			{
				return false;
			}
			return iunknown == _ComObject.IUnknown;
		}

		public override int GetHashCode()
		{
			CheckIUnknown();
			return iunknown.ToInt32();
		}

		[DllImport("ole32.dll", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
		private static extern int CoCreateInstance([In][MarshalAs(UnmanagedType.LPStruct)] Guid rclsid, IntPtr pUnkOuter, uint dwClsContext, [In][MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IntPtr pUnk);
	}
}
