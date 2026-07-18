using System.Collections;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Principal;

namespace System.Threading
{
	[ComDefaultInterface(typeof(_Thread))]
	[ClassInterface(ClassInterfaceType.None)]
	[ComVisible(true)]
	public sealed class Thread : CriticalFinalizerObject, _Thread
	{
		private int lock_thread_id;

		private IntPtr system_thread_handle;

		private object cached_culture_info;

		private IntPtr unused0;

		private bool threadpool_thread;

		private IntPtr name;

		private int name_len;

		private ThreadState state = ThreadState.Unstarted;

		private object abort_exc;

		private int abort_state_handle;

		private long thread_id;

		private IntPtr start_notify;

		private IntPtr stack_ptr;

		private UIntPtr static_data;

		private IntPtr jit_data;

		private IntPtr lock_data;

		private object current_appcontext;

		private int stack_size;

		private object start_obj;

		private IntPtr appdomain_refs;

		private int interruption_requested;

		private IntPtr suspend_event;

		private IntPtr suspended_event;

		private IntPtr resume_event;

		private IntPtr synch_cs;

		private IntPtr serialized_culture_info;

		private int serialized_culture_info_len;

		private IntPtr serialized_ui_culture_info;

		private int serialized_ui_culture_info_len;

		private bool thread_dump_requested;

		private IntPtr end_stack;

		private bool thread_interrupt_requested;

		private byte apartment_state;

		private volatile int critical_region_level;

		private int small_id;

		private IntPtr manage_callback;

		private object pending_exception;

		private ExecutionContext ec_to_set;

		private IntPtr interrupt_on_stop;

		private IntPtr unused3;

		private IntPtr unused4;

		private IntPtr unused5;

		private IntPtr unused6;

		[ThreadStatic]
		private static object[] local_slots;

		[ThreadStatic]
		private static ExecutionContext _ec;

		private MulticastDelegate threadstart;

		private static int _managed_id_counter;

		private int managed_id;

		private IPrincipal _principal;

		private static Hashtable datastorehash;

		private static object datastore_lock = new object();

		private bool in_currentculture;

		private static object culture_lock = new object();

		public static Context CurrentContext
		{
			get
			{
				return AppDomain.InternalGetContext();
			}
		}

		public static IPrincipal CurrentPrincipal
		{
			get
			{
				IPrincipal principal = null;
				Thread currentThread = CurrentThread;
				lock (currentThread)
				{
					principal = currentThread._principal;
					if (principal == null)
					{
						principal = GetDomain().DefaultPrincipal;
					}
				}
				return principal;
			}
			set
			{
				Thread currentThread = CurrentThread;
				lock (currentThread)
				{
					currentThread._principal = value;
				}
			}
		}

		public static Thread CurrentThread
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
			get
			{
				return CurrentThread_internal();
			}
		}

		internal static int CurrentThreadId
		{
			get
			{
				return (int)CurrentThread.thread_id;
			}
		}

		[Obsolete("Deprecated in favor of GetApartmentState, SetApartmentState and TrySetApartmentState.")]
		public ApartmentState ApartmentState
		{
			get
			{
				if ((ThreadState & ThreadState.Stopped) != ThreadState.Running)
				{
					throw new ThreadStateException("Thread is dead; state can not be accessed.");
				}
				return (ApartmentState)apartment_state;
			}
			set
			{
				TrySetApartmentState(value);
			}
		}

		public CultureInfo CurrentCulture
		{
			get
			{
				if (in_currentculture)
				{
					return CultureInfo.InvariantCulture;
				}
				CultureInfo cultureInfo = GetCachedCurrentCulture();
				if (cultureInfo != null)
				{
					return cultureInfo;
				}
				byte[] serializedCurrentCulture = GetSerializedCurrentCulture();
				if (serializedCurrentCulture == null)
				{
					lock (culture_lock)
					{
						in_currentculture = true;
						cultureInfo = CultureInfo.ConstructCurrentCulture();
						SetCachedCurrentCulture(cultureInfo);
						in_currentculture = false;
						NumberFormatter.SetThreadCurrentCulture(cultureInfo);
						return cultureInfo;
					}
				}
				in_currentculture = true;
				try
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					MemoryStream serializationStream = new MemoryStream(serializedCurrentCulture);
					cultureInfo = (CultureInfo)binaryFormatter.Deserialize(serializationStream);
					SetCachedCurrentCulture(cultureInfo);
				}
				finally
				{
					in_currentculture = false;
				}
				NumberFormatter.SetThreadCurrentCulture(cultureInfo);
				return cultureInfo;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				CultureInfo cachedCurrentCulture = GetCachedCurrentCulture();
				if (cachedCurrentCulture == value)
				{
					return;
				}
				value.CheckNeutral();
				in_currentculture = true;
				try
				{
					SetCachedCurrentCulture(value);
					byte[] array = null;
					if (value.IsReadOnly && value.cached_serialized_form != null)
					{
						array = value.cached_serialized_form;
					}
					else
					{
						BinaryFormatter binaryFormatter = new BinaryFormatter();
						MemoryStream memoryStream = new MemoryStream();
						binaryFormatter.Serialize(memoryStream, value);
						array = memoryStream.GetBuffer();
						if (value.IsReadOnly)
						{
							value.cached_serialized_form = array;
						}
					}
					SetSerializedCurrentCulture(array);
				}
				finally
				{
					in_currentculture = false;
				}
				NumberFormatter.SetThreadCurrentCulture(value);
			}
		}

		public CultureInfo CurrentUICulture
		{
			get
			{
				if (in_currentculture)
				{
					return CultureInfo.InvariantCulture;
				}
				CultureInfo cachedCurrentUICulture = GetCachedCurrentUICulture();
				if (cachedCurrentUICulture != null)
				{
					return cachedCurrentUICulture;
				}
				byte[] serializedCurrentUICulture = GetSerializedCurrentUICulture();
				if (serializedCurrentUICulture == null)
				{
					lock (culture_lock)
					{
						in_currentculture = true;
						cachedCurrentUICulture = CultureInfo.ConstructCurrentUICulture();
						SetCachedCurrentUICulture(cachedCurrentUICulture);
						in_currentculture = false;
						return cachedCurrentUICulture;
					}
				}
				in_currentculture = true;
				try
				{
					BinaryFormatter binaryFormatter = new BinaryFormatter();
					MemoryStream serializationStream = new MemoryStream(serializedCurrentUICulture);
					cachedCurrentUICulture = (CultureInfo)binaryFormatter.Deserialize(serializationStream);
					SetCachedCurrentUICulture(cachedCurrentUICulture);
					return cachedCurrentUICulture;
				}
				finally
				{
					in_currentculture = false;
				}
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				CultureInfo cachedCurrentUICulture = GetCachedCurrentUICulture();
				if (cachedCurrentUICulture == value)
				{
					return;
				}
				in_currentculture = true;
				try
				{
					SetCachedCurrentUICulture(value);
					byte[] array = null;
					if (value.IsReadOnly && value.cached_serialized_form != null)
					{
						array = value.cached_serialized_form;
					}
					else
					{
						BinaryFormatter binaryFormatter = new BinaryFormatter();
						MemoryStream memoryStream = new MemoryStream();
						binaryFormatter.Serialize(memoryStream, value);
						array = memoryStream.GetBuffer();
						if (value.IsReadOnly)
						{
							value.cached_serialized_form = array;
						}
					}
					SetSerializedCurrentUICulture(array);
				}
				finally
				{
					in_currentculture = false;
				}
			}
		}

		public bool IsThreadPoolThread
		{
			get
			{
				return IsThreadPoolThreadInternal;
			}
		}

		internal bool IsThreadPoolThreadInternal
		{
			get
			{
				return threadpool_thread;
			}
			set
			{
				threadpool_thread = value;
			}
		}

		public bool IsAlive
		{
			get
			{
				ThreadState threadState = GetState();
				if ((threadState & ThreadState.Aborted) != ThreadState.Running || (threadState & ThreadState.Stopped) != ThreadState.Running || (threadState & ThreadState.Unstarted) != ThreadState.Running)
				{
					return false;
				}
				return true;
			}
		}

		public bool IsBackground
		{
			get
			{
				ThreadState threadState = GetState();
				if ((threadState & ThreadState.Stopped) != ThreadState.Running)
				{
					throw new ThreadStateException("Thread is dead; state can not be accessed.");
				}
				return (threadState & ThreadState.Background) != 0;
			}
			set
			{
				if (value)
				{
					SetState(ThreadState.Background);
				}
				else
				{
					ClrState(ThreadState.Background);
				}
			}
		}

		public string Name
		{
			get
			{
				return GetName_internal();
			}
			set
			{
				SetName_internal(value);
			}
		}

		public ThreadPriority Priority
		{
			get
			{
				return ThreadPriority.Lowest;
			}
			set
			{
			}
		}

		public ThreadState ThreadState
		{
			get
			{
				return GetState();
			}
		}

		[MonoTODO("limited to CompressedStack support")]
		public ExecutionContext ExecutionContext
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
			get
			{
				if (_ec == null)
				{
					_ec = new ExecutionContext();
				}
				return _ec;
			}
		}

		public int ManagedThreadId
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			get
			{
				if (managed_id == 0)
				{
					int newManagedId = GetNewManagedId();
					Interlocked.CompareExchange(ref managed_id, newManagedId, 0);
				}
				return managed_id;
			}
		}

		public Thread(ThreadStart start)
		{
			if (start == null)
			{
				throw new ArgumentNullException("Null ThreadStart");
			}
			threadstart = start;
			Thread_init();
		}

		public Thread(ThreadStart start, int maxStackSize)
		{
			if (start == null)
			{
				throw new ArgumentNullException("start");
			}
			if (maxStackSize < 131072)
			{
				throw new ArgumentException("< 128 kb", "maxStackSize");
			}
			threadstart = start;
			stack_size = maxStackSize;
			Thread_init();
		}

		public Thread(ParameterizedThreadStart start)
		{
			if (start == null)
			{
				throw new ArgumentNullException("start");
			}
			threadstart = start;
			Thread_init();
		}

		public Thread(ParameterizedThreadStart start, int maxStackSize)
		{
			if (start == null)
			{
				throw new ArgumentNullException("start");
			}
			if (maxStackSize < 131072)
			{
				throw new ArgumentException("< 128 kb", "maxStackSize");
			}
			threadstart = start;
			stack_size = maxStackSize;
			Thread_init();
		}

		void _Thread.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _Thread.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _Thread.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _Thread.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Thread CurrentThread_internal();

		private static void InitDataStoreHash()
		{
			lock (datastore_lock)
			{
				if (datastorehash == null)
				{
					datastorehash = Hashtable.Synchronized(new Hashtable());
				}
			}
		}

		public static LocalDataStoreSlot AllocateNamedDataSlot(string name)
		{
			lock (datastore_lock)
			{
				if (datastorehash == null)
				{
					InitDataStoreHash();
				}
				LocalDataStoreSlot localDataStoreSlot = (LocalDataStoreSlot)datastorehash[name];
				if (localDataStoreSlot != null)
				{
					throw new ArgumentException("Named data slot already added");
				}
				localDataStoreSlot = AllocateDataSlot();
				datastorehash.Add(name, localDataStoreSlot);
				return localDataStoreSlot;
			}
		}

		public static void FreeNamedDataSlot(string name)
		{
			lock (datastore_lock)
			{
				if (datastorehash == null)
				{
					InitDataStoreHash();
				}
				LocalDataStoreSlot localDataStoreSlot = (LocalDataStoreSlot)datastorehash[name];
				if (localDataStoreSlot != null)
				{
					datastorehash.Remove(localDataStoreSlot);
				}
			}
		}

		public static LocalDataStoreSlot AllocateDataSlot()
		{
			return new LocalDataStoreSlot(true);
		}

		public static object GetData(LocalDataStoreSlot slot)
		{
			object[] array = local_slots;
			if (slot == null)
			{
				throw new ArgumentNullException("slot");
			}
			if (array != null && slot.slot < array.Length)
			{
				return array[slot.slot];
			}
			return null;
		}

		public static void SetData(LocalDataStoreSlot slot, object data)
		{
			object[] array = local_slots;
			if (slot == null)
			{
				throw new ArgumentNullException("slot");
			}
			if (array == null)
			{
				array = (local_slots = new object[slot.slot + 2]);
			}
			else if (slot.slot >= array.Length)
			{
				object[] array2 = new object[slot.slot + 2];
				array.CopyTo(array2, 0);
				array = (local_slots = array2);
			}
			array[slot.slot] = data;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void FreeLocalSlotValues(int slot, bool thread_local);

		public static LocalDataStoreSlot GetNamedDataSlot(string name)
		{
			lock (datastore_lock)
			{
				if (datastorehash == null)
				{
					InitDataStoreHash();
				}
				LocalDataStoreSlot localDataStoreSlot = (LocalDataStoreSlot)datastorehash[name];
				if (localDataStoreSlot == null)
				{
					localDataStoreSlot = AllocateNamedDataSlot(name);
				}
				return localDataStoreSlot;
			}
		}

		public static AppDomain GetDomain()
		{
			return AppDomain.CurrentDomain;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetDomainID();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResetAbort_internal();

		public static void ResetAbort()
		{
			ResetAbort_internal();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Sleep_internal(int ms);

		public static void Sleep(int millisecondsTimeout)
		{
			if (millisecondsTimeout < -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout", "Negative timeout");
			}
			Sleep_internal(millisecondsTimeout);
		}

		public static void Sleep(TimeSpan timeout)
		{
			long num = (long)timeout.TotalMilliseconds;
			if (num < -1 || num > int.MaxValue)
			{
				throw new ArgumentOutOfRangeException("timeout", "timeout out of range");
			}
			Sleep_internal((int)num);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern IntPtr Thread_internal(MulticastDelegate start);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Thread_init();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern CultureInfo GetCachedCurrentCulture();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern byte[] GetSerializedCurrentCulture();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetCachedCurrentCulture(CultureInfo culture);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetSerializedCurrentCulture(byte[] culture);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern CultureInfo GetCachedCurrentUICulture();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern byte[] GetSerializedCurrentUICulture();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetCachedCurrentUICulture(CultureInfo culture);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetSerializedCurrentUICulture(byte[] culture);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern string GetName_internal();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetName_internal(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Abort_internal(object stateInfo);

		public void Abort()
		{
			Abort_internal(null);
		}

		public void Abort(object stateInfo)
		{
			Abort_internal(stateInfo);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern object GetAbortExceptionState();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Interrupt_internal();

		public void Interrupt()
		{
			Interrupt_internal();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool Join_internal(int ms, IntPtr handle);

		public void Join()
		{
			Join_internal(-1, system_thread_handle);
		}

		public bool Join(int millisecondsTimeout)
		{
			if (millisecondsTimeout < -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout", "Timeout less than zero");
			}
			return Join_internal(millisecondsTimeout, system_thread_handle);
		}

		public bool Join(TimeSpan timeout)
		{
			long num = (long)timeout.TotalMilliseconds;
			if (num < -1 || num > int.MaxValue)
			{
				throw new ArgumentOutOfRangeException("timeout", "timeout out of range");
			}
			return Join_internal((int)num, system_thread_handle);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void MemoryBarrier();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Resume_internal();

		[Obsolete("")]
		public void Resume()
		{
			Resume_internal();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SpinWait_nop();

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static void SpinWait(int iterations)
		{
			if (iterations >= 0)
			{
				while (iterations-- > 0)
				{
					SpinWait_nop();
				}
			}
		}

		public void Start()
		{
			if (!ExecutionContext.IsFlowSuppressed())
			{
				ec_to_set = ExecutionContext.Capture();
			}
			if (CurrentThread._principal != null)
			{
				_principal = CurrentThread._principal;
			}
			if (Thread_internal(threadstart) == (IntPtr)0)
			{
				throw new SystemException("Thread creation failed.");
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Suspend_internal();

		[Obsolete("")]
		public void Suspend()
		{
			Suspend_internal();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void Thread_free_internal(IntPtr handle);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		~Thread()
		{
			Thread_free_internal(system_thread_handle);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetState(ThreadState set);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void ClrState(ThreadState clr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern ThreadState GetState();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern byte VolatileRead(ref byte address);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern double VolatileRead(ref double address);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern short VolatileRead(ref short address);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int VolatileRead(ref int address);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern long VolatileRead(ref long address);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr VolatileRead(ref IntPtr address);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern object VolatileRead(ref object address);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[CLSCompliant(false)]
		public static extern sbyte VolatileRead(ref sbyte address);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float VolatileRead(ref float address);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[CLSCompliant(false)]
		public static extern ushort VolatileRead(ref ushort address);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[CLSCompliant(false)]
		public static extern uint VolatileRead(ref uint address);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[CLSCompliant(false)]
		public static extern ulong VolatileRead(ref ulong address);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[CLSCompliant(false)]
		public static extern UIntPtr VolatileRead(ref UIntPtr address);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void VolatileWrite(ref byte address, byte value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void VolatileWrite(ref double address, double value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void VolatileWrite(ref short address, short value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void VolatileWrite(ref int address, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void VolatileWrite(ref long address, long value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void VolatileWrite(ref IntPtr address, IntPtr value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void VolatileWrite(ref object address, object value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[CLSCompliant(false)]
		public static extern void VolatileWrite(ref sbyte address, sbyte value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void VolatileWrite(ref float address, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[CLSCompliant(false)]
		public static extern void VolatileWrite(ref ushort address, ushort value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[CLSCompliant(false)]
		public static extern void VolatileWrite(ref uint address, uint value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[CLSCompliant(false)]
		public static extern void VolatileWrite(ref ulong address, ulong value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[CLSCompliant(false)]
		public static extern void VolatileWrite(ref UIntPtr address, UIntPtr value);

		private static int GetNewManagedId()
		{
			return Interlocked.Increment(ref _managed_id_counter);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static void BeginCriticalRegion()
		{
			CurrentThread.critical_region_level++;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static void EndCriticalRegion()
		{
			CurrentThread.critical_region_level--;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static void BeginThreadAffinity()
		{
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static void EndThreadAffinity()
		{
		}

		public ApartmentState GetApartmentState()
		{
			return (ApartmentState)apartment_state;
		}

		public void SetApartmentState(ApartmentState state)
		{
			if (!TrySetApartmentState(state))
			{
				throw new InvalidOperationException("Failed to set the specified COM apartment state.");
			}
		}

		public bool TrySetApartmentState(ApartmentState state)
		{
			if (this != CurrentThread && (ThreadState & ThreadState.Unstarted) == 0)
			{
				throw new ThreadStateException("Thread was in an invalid state for the operation being executed.");
			}
			if (apartment_state != 2)
			{
				return false;
			}
			apartment_state = (byte)state;
			return true;
		}

		[ComVisible(false)]
		public override int GetHashCode()
		{
			return ManagedThreadId;
		}

		public void Start(object parameter)
		{
			start_obj = parameter;
			Start();
		}

		[Obsolete("see CompressedStack class")]
		public CompressedStack GetCompressedStack()
		{
			CompressedStack compressedStack = ExecutionContext.SecurityContext.CompressedStack;
			return (compressedStack != null && !compressedStack.IsEmpty()) ? compressedStack.CreateCopy() : null;
		}

		[Obsolete("see CompressedStack class")]
		public void SetCompressedStack(CompressedStack stack)
		{
			ExecutionContext.SecurityContext.CompressedStack = stack;
		}
	}
}
