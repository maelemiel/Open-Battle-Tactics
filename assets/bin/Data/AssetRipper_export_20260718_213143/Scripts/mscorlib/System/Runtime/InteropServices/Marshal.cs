using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using System.Threading;
using Mono.Interop;

namespace System.Runtime.InteropServices
{
	[SuppressUnmanagedCodeSecurity]
	public static class Marshal
	{
		public static readonly int SystemMaxDBCSCharSize;

		public static readonly int SystemDefaultCharSize;

		static Marshal()
		{
			SystemMaxDBCSCharSize = 2;
			SystemDefaultCharSize = ((Environment.OSVersion.Platform != PlatformID.Win32NT) ? 1 : 2);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int AddRefInternal(IntPtr pUnk);

		public static int AddRef(IntPtr pUnk)
		{
			if (pUnk == IntPtr.Zero)
			{
				throw new ArgumentException("Value cannot be null.", "pUnk");
			}
			return AddRefInternal(pUnk);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr AllocCoTaskMem(int cb);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static extern IntPtr AllocHGlobal(IntPtr cb);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static IntPtr AllocHGlobal(int cb)
		{
			return AllocHGlobal((IntPtr)cb);
		}

		[MonoTODO]
		public static object BindToMoniker(string monikerName)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static void ChangeWrapperHandleStrength(object otp, bool fIsWeak)
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void copy_to_unmanaged(Array source, int startIndex, IntPtr destination, int length);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void copy_from_unmanaged(IntPtr source, int startIndex, Array destination, int length);

		public static void Copy(byte[] source, int startIndex, IntPtr destination, int length)
		{
			copy_to_unmanaged(source, startIndex, destination, length);
		}

		public static void Copy(char[] source, int startIndex, IntPtr destination, int length)
		{
			copy_to_unmanaged(source, startIndex, destination, length);
		}

		public static void Copy(short[] source, int startIndex, IntPtr destination, int length)
		{
			copy_to_unmanaged(source, startIndex, destination, length);
		}

		public static void Copy(int[] source, int startIndex, IntPtr destination, int length)
		{
			copy_to_unmanaged(source, startIndex, destination, length);
		}

		public static void Copy(long[] source, int startIndex, IntPtr destination, int length)
		{
			copy_to_unmanaged(source, startIndex, destination, length);
		}

		public static void Copy(float[] source, int startIndex, IntPtr destination, int length)
		{
			copy_to_unmanaged(source, startIndex, destination, length);
		}

		public static void Copy(double[] source, int startIndex, IntPtr destination, int length)
		{
			copy_to_unmanaged(source, startIndex, destination, length);
		}

		public static void Copy(IntPtr[] source, int startIndex, IntPtr destination, int length)
		{
			copy_to_unmanaged(source, startIndex, destination, length);
		}

		public static void Copy(IntPtr source, byte[] destination, int startIndex, int length)
		{
			copy_from_unmanaged(source, startIndex, destination, length);
		}

		public static void Copy(IntPtr source, char[] destination, int startIndex, int length)
		{
			copy_from_unmanaged(source, startIndex, destination, length);
		}

		public static void Copy(IntPtr source, short[] destination, int startIndex, int length)
		{
			copy_from_unmanaged(source, startIndex, destination, length);
		}

		public static void Copy(IntPtr source, int[] destination, int startIndex, int length)
		{
			copy_from_unmanaged(source, startIndex, destination, length);
		}

		public static void Copy(IntPtr source, long[] destination, int startIndex, int length)
		{
			copy_from_unmanaged(source, startIndex, destination, length);
		}

		public static void Copy(IntPtr source, float[] destination, int startIndex, int length)
		{
			copy_from_unmanaged(source, startIndex, destination, length);
		}

		public static void Copy(IntPtr source, double[] destination, int startIndex, int length)
		{
			copy_from_unmanaged(source, startIndex, destination, length);
		}

		public static void Copy(IntPtr source, IntPtr[] destination, int startIndex, int length)
		{
			copy_from_unmanaged(source, startIndex, destination, length);
		}

		public static IntPtr CreateAggregatedObject(IntPtr pOuter, object o)
		{
			throw new NotImplementedException();
		}

		public static object CreateWrapperOfType(object o, Type t)
		{
			__ComObject _ComObject = o as __ComObject;
			if (_ComObject == null)
			{
				throw new ArgumentException("o must derive from __ComObject", "o");
			}
			if (t == null)
			{
				throw new ArgumentNullException("t");
			}
			Type[] interfaces = o.GetType().GetInterfaces();
			Type[] array = interfaces;
			foreach (Type type in array)
			{
				if (type.IsImport && _ComObject.GetInterface(type) == IntPtr.Zero)
				{
					throw new InvalidCastException();
				}
			}
			return ComInteropProxy.GetProxy(_ComObject.IUnknown, t).GetTransparentProxy();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[ComVisible(true)]
		public static extern void DestroyStructure(IntPtr ptr, Type structuretype);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void FreeBSTR(IntPtr ptr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void FreeCoTaskMem(IntPtr ptr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static extern void FreeHGlobal(IntPtr hglobal);

		private static void ClearBSTR(IntPtr ptr)
		{
			int num = ReadInt32(ptr, -4);
			for (int i = 0; i < num; i++)
			{
				WriteByte(ptr, i, 0);
			}
		}

		public static void ZeroFreeBSTR(IntPtr s)
		{
			ClearBSTR(s);
			FreeBSTR(s);
		}

		private static void ClearAnsi(IntPtr ptr)
		{
			for (int i = 0; ReadByte(ptr, i) != 0; i++)
			{
				WriteByte(ptr, i, 0);
			}
		}

		private static void ClearUnicode(IntPtr ptr)
		{
			for (int i = 0; ReadInt16(ptr, i) != 0; i += 2)
			{
				WriteInt16(ptr, i, 0);
			}
		}

		public static void ZeroFreeCoTaskMemAnsi(IntPtr s)
		{
			ClearAnsi(s);
			FreeCoTaskMem(s);
		}

		public static void ZeroFreeCoTaskMemUnicode(IntPtr s)
		{
			ClearUnicode(s);
			FreeCoTaskMem(s);
		}

		public static void ZeroFreeGlobalAllocAnsi(IntPtr s)
		{
			ClearAnsi(s);
			FreeHGlobal(s);
		}

		public static void ZeroFreeGlobalAllocUnicode(IntPtr s)
		{
			ClearUnicode(s);
			FreeHGlobal(s);
		}

		public static Guid GenerateGuidForType(Type type)
		{
			return type.GUID;
		}

		[MonoTODO]
		public static string GenerateProgIdForType(Type type)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static object GetActiveObject(string progID)
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetCCW(object o, Type T);

		private static IntPtr GetComInterfaceForObjectInternal(object o, Type T)
		{
			if (IsComObject(o))
			{
				return ((__ComObject)o).GetInterface(T);
			}
			return GetCCW(o, T);
		}

		public static IntPtr GetComInterfaceForObject(object o, Type T)
		{
			IntPtr comInterfaceForObjectInternal = GetComInterfaceForObjectInternal(o, T);
			AddRef(comInterfaceForObjectInternal);
			return comInterfaceForObjectInternal;
		}

		[MonoTODO]
		public static IntPtr GetComInterfaceForObjectInContext(object o, Type t)
		{
			throw new NotImplementedException();
		}

		[MonoNotSupported("MSDN states user code should never need to call this method.")]
		public static object GetComObjectData(object obj, object key)
		{
			throw new NotSupportedException("MSDN states user code should never need to call this method.");
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetComSlotForMethodInfoInternal(MemberInfo m);

		public static int GetComSlotForMethodInfo(MemberInfo m)
		{
			if (m == null)
			{
				throw new ArgumentNullException("m");
			}
			if (!(m is MethodInfo))
			{
				throw new ArgumentException("The MemberInfo must be an interface method.", "m");
			}
			if (!m.DeclaringType.IsInterface)
			{
				throw new ArgumentException("The MemberInfo must be an interface method.", "m");
			}
			return GetComSlotForMethodInfoInternal(m);
		}

		[MonoTODO]
		public static int GetEndComSlot(Type t)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static int GetExceptionCode()
		{
			throw new NotImplementedException();
		}

		[ComVisible(true)]
		[MonoTODO]
		public static IntPtr GetExceptionPointers()
		{
			throw new NotImplementedException();
		}

		public static IntPtr GetHINSTANCE(Module m)
		{
			if (m == null)
			{
				throw new ArgumentNullException("m");
			}
			return m.GetHINSTANCE();
		}

		[MonoTODO("SetErrorInfo")]
		public static int GetHRForException(Exception e)
		{
			return e.hresult;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[MonoTODO]
		public static int GetHRForLastWin32Error()
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetIDispatchForObjectInternal(object o);

		public static IntPtr GetIDispatchForObject(object o)
		{
			IntPtr iDispatchForObjectInternal = GetIDispatchForObjectInternal(o);
			AddRef(iDispatchForObjectInternal);
			return iDispatchForObjectInternal;
		}

		[MonoTODO]
		public static IntPtr GetIDispatchForObjectInContext(object o)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static IntPtr GetITypeInfoForType(Type t)
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetIUnknownForObjectInternal(object o);

		public static IntPtr GetIUnknownForObject(object o)
		{
			IntPtr iUnknownForObjectInternal = GetIUnknownForObjectInternal(o);
			AddRef(iUnknownForObjectInternal);
			return iUnknownForObjectInternal;
		}

		[MonoTODO]
		public static IntPtr GetIUnknownForObjectInContext(object o)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		[Obsolete("This method has been deprecated")]
		public static IntPtr GetManagedThunkForUnmanagedMethodPtr(IntPtr pfnMethodToWrap, IntPtr pbSignature, int cbSignature)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static MemberInfo GetMethodInfoForComSlot(Type t, int slot, ref ComMemberType memberType)
		{
			throw new NotImplementedException();
		}

		public static void GetNativeVariantForObject(object obj, IntPtr pDstNativeVariant)
		{
			Variant variant = default(Variant);
			variant.SetValue(obj);
			StructureToPtr(variant, pDstNativeVariant, false);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern object GetObjectForCCW(IntPtr pUnk);

		public static object GetObjectForIUnknown(IntPtr pUnk)
		{
			object obj = GetObjectForCCW(pUnk);
			if (obj == null)
			{
				ComInteropProxy proxy = ComInteropProxy.GetProxy(pUnk, typeof(__ComObject));
				obj = proxy.GetTransparentProxy();
			}
			return obj;
		}

		public static object GetObjectForNativeVariant(IntPtr pSrcNativeVariant)
		{
			return ((Variant)PtrToStructure(pSrcNativeVariant, typeof(Variant))).GetValue();
		}

		public static object[] GetObjectsForNativeVariants(IntPtr aSrcNativeVariant, int cVars)
		{
			if (cVars < 0)
			{
				throw new ArgumentOutOfRangeException("cVars", "cVars cannot be a negative number.");
			}
			object[] array = new object[cVars];
			for (int i = 0; i < cVars; i++)
			{
				array[i] = GetObjectForNativeVariant((IntPtr)(aSrcNativeVariant.ToInt64() + i * SizeOf(typeof(Variant))));
			}
			return array;
		}

		[MonoTODO]
		public static int GetStartComSlot(Type t)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		[Obsolete("This method has been deprecated")]
		public static Thread GetThreadFromFiberCookie(int cookie)
		{
			throw new NotImplementedException();
		}

		public static object GetTypedObjectForIUnknown(IntPtr pUnk, Type t)
		{
			ComInteropProxy comInteropProxy = new ComInteropProxy(pUnk, t);
			__ComObject _ComObject = (__ComObject)comInteropProxy.GetTransparentProxy();
			Type[] interfaces = t.GetInterfaces();
			foreach (Type type in interfaces)
			{
				if ((type.Attributes & TypeAttributes.Import) == TypeAttributes.Import && _ComObject.GetInterface(type) == IntPtr.Zero)
				{
					return null;
				}
			}
			return _ComObject;
		}

		[MonoTODO]
		public static Type GetTypeForITypeInfo(IntPtr piTypeInfo)
		{
			throw new NotImplementedException();
		}

		[Obsolete]
		[MonoTODO]
		public static string GetTypeInfoName(UCOMITypeInfo pTI)
		{
			throw new NotImplementedException();
		}

		public static string GetTypeInfoName(ITypeInfo typeInfo)
		{
			throw new NotImplementedException();
		}

		[Obsolete]
		[MonoTODO]
		public static Guid GetTypeLibGuid(UCOMITypeLib pTLB)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static Guid GetTypeLibGuid(ITypeLib typelib)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static Guid GetTypeLibGuidForAssembly(Assembly asm)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		[Obsolete]
		public static int GetTypeLibLcid(UCOMITypeLib pTLB)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static int GetTypeLibLcid(ITypeLib typelib)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		[Obsolete]
		public static string GetTypeLibName(UCOMITypeLib pTLB)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static string GetTypeLibName(ITypeLib typelib)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static void GetTypeLibVersionForAssembly(Assembly inputAssembly, out int majorVersion, out int minorVersion)
		{
			throw new NotImplementedException();
		}

		public static object GetUniqueObjectForIUnknown(IntPtr unknown)
		{
			throw new NotImplementedException();
		}

		[Obsolete("This method has been deprecated")]
		[MonoTODO]
		public static IntPtr GetUnmanagedThunkForManagedMethodPtr(IntPtr pfnMethodToWrap, IntPtr pbSignature, int cbSignature)
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsComObject(object o);

		[MonoTODO]
		public static bool IsTypeVisibleFromCom(Type t)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static int NumParamBytes(MethodInfo m)
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static extern int GetLastWin32Error();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr OffsetOf(Type t, string fieldName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Prelink(MethodInfo m);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void PrelinkAll(Type c);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string PtrToStringAnsi(IntPtr ptr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string PtrToStringAnsi(IntPtr ptr, int len);

		public static string PtrToStringAuto(IntPtr ptr)
		{
			return (SystemDefaultCharSize != 2) ? PtrToStringAnsi(ptr) : PtrToStringUni(ptr);
		}

		public static string PtrToStringAuto(IntPtr ptr, int len)
		{
			return (SystemDefaultCharSize != 2) ? PtrToStringAnsi(ptr, len) : PtrToStringUni(ptr, len);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string PtrToStringUni(IntPtr ptr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string PtrToStringUni(IntPtr ptr, int len);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string PtrToStringBSTR(IntPtr ptr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[ComVisible(true)]
		public static extern void PtrToStructure(IntPtr ptr, object structure);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[ComVisible(true)]
		public static extern object PtrToStructure(IntPtr ptr, Type structureType);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int QueryInterfaceInternal(IntPtr pUnk, ref Guid iid, out IntPtr ppv);

		public static int QueryInterface(IntPtr pUnk, ref Guid iid, out IntPtr ppv)
		{
			if (pUnk == IntPtr.Zero)
			{
				throw new ArgumentException("Value cannot be null.", "pUnk");
			}
			return QueryInterfaceInternal(pUnk, ref iid, out ppv);
		}

		public static byte ReadByte(IntPtr ptr)
		{
			return ReadByte(ptr, 0);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern byte ReadByte(IntPtr ptr, int ofs);

		[MonoTODO]
		public static byte ReadByte([In][MarshalAs(UnmanagedType.AsAny)] object ptr, int ofs)
		{
			throw new NotImplementedException();
		}

		public static short ReadInt16(IntPtr ptr)
		{
			return ReadInt16(ptr, 0);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern short ReadInt16(IntPtr ptr, int ofs);

		[MonoTODO]
		public static short ReadInt16([In][MarshalAs(UnmanagedType.AsAny)] object ptr, int ofs)
		{
			throw new NotImplementedException();
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static int ReadInt32(IntPtr ptr)
		{
			return ReadInt32(ptr, 0);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static extern int ReadInt32(IntPtr ptr, int ofs);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[MonoTODO]
		public static int ReadInt32([In][MarshalAs(UnmanagedType.AsAny)] object ptr, int ofs)
		{
			throw new NotImplementedException();
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static long ReadInt64(IntPtr ptr)
		{
			return ReadInt64(ptr, 0);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static extern long ReadInt64(IntPtr ptr, int ofs);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[MonoTODO]
		public static long ReadInt64([In][MarshalAs(UnmanagedType.AsAny)] object ptr, int ofs)
		{
			throw new NotImplementedException();
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static IntPtr ReadIntPtr(IntPtr ptr)
		{
			return ReadIntPtr(ptr, 0);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static extern IntPtr ReadIntPtr(IntPtr ptr, int ofs);

		[MonoTODO]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static IntPtr ReadIntPtr([In][MarshalAs(UnmanagedType.AsAny)] object ptr, int ofs)
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr ReAllocCoTaskMem(IntPtr pv, int cb);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr ReAllocHGlobal(IntPtr pv, IntPtr cb);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		private static extern int ReleaseInternal(IntPtr pUnk);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static int Release(IntPtr pUnk)
		{
			if (pUnk == IntPtr.Zero)
			{
				throw new ArgumentException("Value cannot be null.", "pUnk");
			}
			return ReleaseInternal(pUnk);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int ReleaseComObjectInternal(object co);

		public static int ReleaseComObject(object o)
		{
			if (o == null)
			{
				throw new ArgumentException("Value cannot be null.", "o");
			}
			if (!IsComObject(o))
			{
				throw new ArgumentException("Value must be a Com object.", "o");
			}
			return ReleaseComObjectInternal(o);
		}

		[Obsolete]
		[MonoTODO]
		public static void ReleaseThreadCache()
		{
			throw new NotImplementedException();
		}

		[MonoNotSupported("MSDN states user code should never need to call this method.")]
		public static bool SetComObjectData(object obj, object key, object data)
		{
			throw new NotSupportedException("MSDN states user code should never need to call this method.");
		}

		[ComVisible(true)]
		public static int SizeOf(object structure)
		{
			return SizeOf(structure.GetType());
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int SizeOf(Type t);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr StringToBSTR(string s);

		public static IntPtr StringToCoTaskMemAnsi(string s)
		{
			int num = s.Length + 1;
			IntPtr intPtr = AllocCoTaskMem(num);
			byte[] array = new byte[num];
			for (int i = 0; i < s.Length; i++)
			{
				array[i] = (byte)s[i];
			}
			array[s.Length] = 0;
			copy_to_unmanaged(array, 0, intPtr, num);
			return intPtr;
		}

		public static IntPtr StringToCoTaskMemAuto(string s)
		{
			return (SystemDefaultCharSize != 2) ? StringToCoTaskMemAnsi(s) : StringToCoTaskMemUni(s);
		}

		public static IntPtr StringToCoTaskMemUni(string s)
		{
			int num = s.Length + 1;
			IntPtr intPtr = AllocCoTaskMem(num * 2);
			char[] array = new char[num];
			s.CopyTo(0, array, 0, s.Length);
			array[s.Length] = '\0';
			copy_to_unmanaged(array, 0, intPtr, num);
			return intPtr;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr StringToHGlobalAnsi(string s);

		public static IntPtr StringToHGlobalAuto(string s)
		{
			return (SystemDefaultCharSize != 2) ? StringToHGlobalAnsi(s) : StringToHGlobalUni(s);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr StringToHGlobalUni(string s);

		public static IntPtr SecureStringToBSTR(SecureString s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			int length = s.Length;
			IntPtr intPtr = AllocCoTaskMem((length + 1) * 2 + 4);
			byte[] array = null;
			WriteInt32(intPtr, 0, length * 2);
			try
			{
				array = s.GetBuffer();
				for (int i = 0; i < length; i++)
				{
					WriteInt16(intPtr, 4 + i * 2, (short)((array[i * 2] << 8) | array[i * 2 + 1]));
				}
				WriteInt16(intPtr, 4 + array.Length, 0);
			}
			finally
			{
				if (array != null)
				{
					int num = array.Length;
					while (num > 0)
					{
						num--;
						array[num] = 0;
					}
				}
			}
			return (IntPtr)((long)intPtr + 4);
		}

		public static IntPtr SecureStringToCoTaskMemAnsi(SecureString s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			int length = s.Length;
			IntPtr intPtr = AllocCoTaskMem(length + 1);
			byte[] array = new byte[length + 1];
			try
			{
				byte[] buffer = s.GetBuffer();
				int num = 0;
				int num2 = 0;
				while (num < length)
				{
					array[num] = buffer[num2 + 1];
					buffer[num2] = 0;
					buffer[num2 + 1] = 0;
					num++;
					num2 += 2;
				}
				array[num] = 0;
				copy_to_unmanaged(array, 0, intPtr, length + 1);
				return intPtr;
			}
			finally
			{
				int num3 = length;
				while (num3 > 0)
				{
					num3--;
					array[num3] = 0;
				}
			}
		}

		public static IntPtr SecureStringToCoTaskMemUnicode(SecureString s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			int length = s.Length;
			IntPtr intPtr = AllocCoTaskMem(length * 2 + 2);
			byte[] array = null;
			try
			{
				array = s.GetBuffer();
				for (int i = 0; i < length; i++)
				{
					WriteInt16(intPtr, i * 2, (short)((array[i * 2] << 8) | array[i * 2 + 1]));
				}
				WriteInt16(intPtr, array.Length, 0);
				return intPtr;
			}
			finally
			{
				if (array != null)
				{
					int num = array.Length;
					while (num > 0)
					{
						num--;
						array[num] = 0;
					}
				}
			}
		}

		public static IntPtr SecureStringToGlobalAllocAnsi(SecureString s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			return SecureStringToCoTaskMemAnsi(s);
		}

		public static IntPtr SecureStringToGlobalAllocUnicode(SecureString s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			return SecureStringToCoTaskMemUnicode(s);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[ComVisible(true)]
		public static extern void StructureToPtr(object structure, IntPtr ptr, bool fDeleteOld);

		public static void ThrowExceptionForHR(int errorCode)
		{
			Exception exceptionForHR = GetExceptionForHR(errorCode);
			if (exceptionForHR != null)
			{
				throw exceptionForHR;
			}
		}

		public static void ThrowExceptionForHR(int errorCode, IntPtr errorInfo)
		{
			Exception exceptionForHR = GetExceptionForHR(errorCode, errorInfo);
			if (exceptionForHR != null)
			{
				throw exceptionForHR;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr UnsafeAddrOfPinnedArrayElement(Array arr, int index);

		public static void WriteByte(IntPtr ptr, byte val)
		{
			WriteByte(ptr, 0, val);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void WriteByte(IntPtr ptr, int ofs, byte val);

		[MonoTODO]
		public static void WriteByte([In][Out][MarshalAs(UnmanagedType.AsAny)] object ptr, int ofs, byte val)
		{
			throw new NotImplementedException();
		}

		public static void WriteInt16(IntPtr ptr, short val)
		{
			WriteInt16(ptr, 0, val);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void WriteInt16(IntPtr ptr, int ofs, short val);

		[MonoTODO]
		public static void WriteInt16([In][Out][MarshalAs(UnmanagedType.AsAny)] object ptr, int ofs, short val)
		{
			throw new NotImplementedException();
		}

		public static void WriteInt16(IntPtr ptr, char val)
		{
			WriteInt16(ptr, 0, val);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		[MonoTODO]
		public static extern void WriteInt16(IntPtr ptr, int ofs, char val);

		[MonoTODO]
		public static void WriteInt16([In][Out] object ptr, int ofs, char val)
		{
			throw new NotImplementedException();
		}

		public static void WriteInt32(IntPtr ptr, int val)
		{
			WriteInt32(ptr, 0, val);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void WriteInt32(IntPtr ptr, int ofs, int val);

		[MonoTODO]
		public static void WriteInt32([In][Out][MarshalAs(UnmanagedType.AsAny)] object ptr, int ofs, int val)
		{
			throw new NotImplementedException();
		}

		public static void WriteInt64(IntPtr ptr, long val)
		{
			WriteInt64(ptr, 0, val);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void WriteInt64(IntPtr ptr, int ofs, long val);

		[MonoTODO]
		public static void WriteInt64([In][Out][MarshalAs(UnmanagedType.AsAny)] object ptr, int ofs, long val)
		{
			throw new NotImplementedException();
		}

		public static void WriteIntPtr(IntPtr ptr, IntPtr val)
		{
			WriteIntPtr(ptr, 0, val);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void WriteIntPtr(IntPtr ptr, int ofs, IntPtr val);

		[MonoTODO]
		public static void WriteIntPtr([In][Out][MarshalAs(UnmanagedType.AsAny)] object ptr, int ofs, IntPtr val)
		{
			throw new NotImplementedException();
		}

		public static Exception GetExceptionForHR(int errorCode)
		{
			return GetExceptionForHR(errorCode, IntPtr.Zero);
		}

		public static Exception GetExceptionForHR(int errorCode, IntPtr errorInfo)
		{
			switch (errorCode)
			{
			case -2147024882:
				return new OutOfMemoryException();
			case -2147024809:
				return new ArgumentException();
			default:
				if (errorCode < 0)
				{
					return new COMException(string.Empty, errorCode);
				}
				return null;
			}
		}

		public static int FinalReleaseComObject(object o)
		{
			while (ReleaseComObject(o) != 0)
			{
			}
			return 0;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Delegate GetDelegateForFunctionPointerInternal(IntPtr ptr, Type t);

		public static Delegate GetDelegateForFunctionPointer(IntPtr ptr, Type t)
		{
			if (t == null)
			{
				throw new ArgumentNullException("t");
			}
			if (!t.IsSubclassOf(typeof(MulticastDelegate)) || t == typeof(MulticastDelegate))
			{
				throw new ArgumentException("Type is not a delegate", "t");
			}
			if (ptr == IntPtr.Zero)
			{
				throw new ArgumentNullException("ptr");
			}
			return GetDelegateForFunctionPointerInternal(ptr, t);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetFunctionPointerForDelegateInternal(Delegate d);

		public static IntPtr GetFunctionPointerForDelegate(Delegate d)
		{
			if ((object)d == null)
			{
				throw new ArgumentNullException("d");
			}
			return GetFunctionPointerForDelegateInternal(d);
		}
	}
}
