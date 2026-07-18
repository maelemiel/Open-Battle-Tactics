using System.ComponentModel;

namespace System.Diagnostics
{
	[Designer("System.Diagnostics.Design.ProcessThreadDesigner, System.Design, Version=2.0.5.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
	public class ProcessThread : Component
	{
		[MonitoringDescription("The base priority of this thread.")]
		[System.MonoTODO]
		public int BasePriority
		{
			get
			{
				return 0;
			}
		}

		[MonitoringDescription("The current priority of this thread.")]
		[System.MonoTODO]
		public int CurrentPriority
		{
			get
			{
				return 0;
			}
		}

		[MonitoringDescription("The ID of this thread.")]
		[System.MonoTODO]
		public int Id
		{
			get
			{
				return 0;
			}
		}

		[Browsable(false)]
		[System.MonoTODO]
		public int IdealProcessor
		{
			set
			{
			}
		}

		[MonitoringDescription("Thread gets a priority boot when interactively used by a user.")]
		[System.MonoTODO]
		public bool PriorityBoostEnabled
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		[System.MonoTODO]
		[MonitoringDescription("The priority level of this thread.")]
		public ThreadPriorityLevel PriorityLevel
		{
			get
			{
				return ThreadPriorityLevel.Idle;
			}
			set
			{
			}
		}

		[MonitoringDescription("The amount of CPU time used in privileged mode.")]
		[System.MonoTODO]
		public TimeSpan PrivilegedProcessorTime
		{
			get
			{
				return new TimeSpan(0L);
			}
		}

		[Browsable(false)]
		[System.MonoTODO]
		public IntPtr ProcessorAffinity
		{
			set
			{
			}
		}

		[MonitoringDescription("The start address in memory of this thread.")]
		[System.MonoTODO]
		public IntPtr StartAddress
		{
			get
			{
				return (IntPtr)0;
			}
		}

		[MonitoringDescription("The time this thread was started.")]
		[System.MonoTODO]
		public DateTime StartTime
		{
			get
			{
				return new DateTime(0L);
			}
		}

		[System.MonoTODO]
		[MonitoringDescription("The current state of this thread.")]
		public ThreadState ThreadState
		{
			get
			{
				return ThreadState.Initialized;
			}
		}

		[MonitoringDescription("The total amount of CPU time used.")]
		[System.MonoTODO]
		public TimeSpan TotalProcessorTime
		{
			get
			{
				return new TimeSpan(0L);
			}
		}

		[MonitoringDescription("The amount of CPU time used in user mode.")]
		[System.MonoTODO]
		public TimeSpan UserProcessorTime
		{
			get
			{
				return new TimeSpan(0L);
			}
		}

		[MonitoringDescription("The reason why this thread is waiting.")]
		[System.MonoTODO]
		public ThreadWaitReason WaitReason
		{
			get
			{
				return ThreadWaitReason.Executive;
			}
		}

		[System.MonoTODO("Parse parameters")]
		internal ProcessThread()
		{
		}

		[System.MonoTODO]
		public void ResetIdealProcessor()
		{
		}
	}
}
