using System.Runtime.InteropServices;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Messaging;
using System.Threading;

namespace System.Runtime.Remoting.Contexts
{
	[Serializable]
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Class)]
	public class SynchronizationAttribute : ContextAttribute, IContributeClientContextSink, IContributeServerContextSink
	{
		public const int NOT_SUPPORTED = 1;

		public const int SUPPORTED = 2;

		public const int REQUIRED = 4;

		public const int REQUIRES_NEW = 8;

		private bool _bReEntrant;

		private int _flavor;

		[NonSerialized]
		private bool _locked;

		[NonSerialized]
		private int _lockCount;

		[NonSerialized]
		private Mutex _mutex = new Mutex(false);

		[NonSerialized]
		private Thread _ownerThread;

		public virtual bool IsReEntrant
		{
			get
			{
				return _bReEntrant;
			}
		}

		public virtual bool Locked
		{
			get
			{
				return _locked;
			}
			set
			{
				if (value)
				{
					_mutex.WaitOne();
					lock (this)
					{
						_lockCount++;
						if (_lockCount > 1)
						{
							ReleaseLock();
						}
						_ownerThread = Thread.CurrentThread;
						return;
					}
				}
				lock (this)
				{
					while (_lockCount > 0 && _ownerThread == Thread.CurrentThread)
					{
						_lockCount--;
						_mutex.ReleaseMutex();
						_ownerThread = null;
					}
				}
			}
		}

		public SynchronizationAttribute()
			: this(8, false)
		{
		}

		public SynchronizationAttribute(bool reEntrant)
			: this(8, reEntrant)
		{
		}

		public SynchronizationAttribute(int flag)
			: this(flag, false)
		{
		}

		public SynchronizationAttribute(int flag, bool reEntrant)
			: base("Synchronization")
		{
			if (flag != 1 && flag != 4 && flag != 8 && flag != 2)
			{
				throw new ArgumentException("flag");
			}
			_bReEntrant = reEntrant;
			_flavor = flag;
		}

		internal void AcquireLock()
		{
			_mutex.WaitOne();
			lock (this)
			{
				_ownerThread = Thread.CurrentThread;
				_lockCount++;
			}
		}

		internal void ReleaseLock()
		{
			lock (this)
			{
				if (_lockCount > 0 && _ownerThread == Thread.CurrentThread)
				{
					_lockCount--;
					_mutex.ReleaseMutex();
					_ownerThread = null;
				}
			}
		}

		[ComVisible(true)]
		public override void GetPropertiesForNewContext(IConstructionCallMessage ctorMsg)
		{
			if (_flavor != 1)
			{
				ctorMsg.ContextProperties.Add(this);
			}
		}

		public virtual IMessageSink GetClientContextSink(IMessageSink nextSink)
		{
			return new SynchronizedClientContextSink(nextSink, this);
		}

		public virtual IMessageSink GetServerContextSink(IMessageSink nextSink)
		{
			return new SynchronizedServerContextSink(nextSink, this);
		}

		[ComVisible(true)]
		public override bool IsContextOK(Context ctx, IConstructionCallMessage msg)
		{
			SynchronizationAttribute synchronizationAttribute = ctx.GetProperty("Synchronization") as SynchronizationAttribute;
			switch (_flavor)
			{
			case 1:
				return synchronizationAttribute == null;
			case 4:
				return synchronizationAttribute != null;
			case 8:
				return false;
			case 2:
				return true;
			default:
				return false;
			}
		}

		internal static void ExitContext()
		{
			if (!Thread.CurrentContext.IsDefaultContext)
			{
				SynchronizationAttribute synchronizationAttribute = Thread.CurrentContext.GetProperty("Synchronization") as SynchronizationAttribute;
				if (synchronizationAttribute != null)
				{
					synchronizationAttribute.Locked = false;
				}
			}
		}

		internal static void EnterContext()
		{
			if (!Thread.CurrentContext.IsDefaultContext)
			{
				SynchronizationAttribute synchronizationAttribute = Thread.CurrentContext.GetProperty("Synchronization") as SynchronizationAttribute;
				if (synchronizationAttribute != null)
				{
					synchronizationAttribute.Locked = true;
				}
			}
		}
	}
}
