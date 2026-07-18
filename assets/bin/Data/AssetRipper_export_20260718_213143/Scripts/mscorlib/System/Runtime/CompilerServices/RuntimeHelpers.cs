using System.Runtime.ConstrainedExecution;

namespace System.Runtime.CompilerServices
{
	public static class RuntimeHelpers
	{
		public delegate void TryCode(object userData);

		public delegate void CleanupCode(object userData, bool exceptionThrown);

		public static extern int OffsetToStringData
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InitializeArray(Array array, IntPtr fldHandle);

		public static void InitializeArray(Array array, RuntimeFieldHandle fldHandle)
		{
			if (array == null || fldHandle.Value == IntPtr.Zero)
			{
				throw new ArgumentNullException();
			}
			InitializeArray(array, fldHandle.Value);
		}

		public static int GetHashCode(object o)
		{
			return object.InternalGetHashCode(o);
		}

		public new static bool Equals(object o1, object o2)
		{
			if (o1 == o2)
			{
				return true;
			}
			if (o1 == null || o2 == null)
			{
				return false;
			}
			if (o1 is ValueType)
			{
				return ValueType.DefaultEquals(o1, o2);
			}
			return object.Equals(o1, o2);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern object GetObjectValue(object obj);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RunClassConstructor(IntPtr type);

		public static void RunClassConstructor(RuntimeTypeHandle type)
		{
			if (type.Value == IntPtr.Zero)
			{
				throw new ArgumentException("Handle is not initialized.", "type");
			}
			RunClassConstructor(type.Value);
		}

		[MonoTODO("Currently a no-op")]
		public static void ExecuteCodeWithGuaranteedCleanup(TryCode code, CleanupCode backoutCode, object userData)
		{
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[MonoTODO("Currently a no-op")]
		public static void PrepareConstrainedRegions()
		{
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[MonoTODO("Currently a no-op")]
		public static void PrepareConstrainedRegionsNoOP()
		{
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[MonoTODO("Currently a no-op")]
		public static void ProbeForSufficientStack()
		{
		}

		[MonoTODO("Currently a no-op")]
		public static void PrepareDelegate(Delegate d)
		{
			if ((object)d == null)
			{
				throw new ArgumentNullException("d");
			}
		}

		[MonoTODO("Currently a no-op")]
		public static void PrepareMethod(RuntimeMethodHandle method)
		{
		}

		[MonoTODO("Currently a no-op")]
		public static void PrepareMethod(RuntimeMethodHandle method, RuntimeTypeHandle[] instantiation)
		{
		}

		public static void RunModuleConstructor(ModuleHandle module)
		{
			if (module == ModuleHandle.EmptyHandle)
			{
				throw new ArgumentException("Handle is not initialized.", "module");
			}
			RunModuleConstructor(module.Value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RunModuleConstructor(IntPtr module);
	}
}
