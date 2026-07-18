using System.Collections;
using System.Runtime.InteropServices;

namespace System.Threading
{
	[ComVisible(true)]
	public sealed class Timer : MarshalByRefObject, IDisposable
	{
		private sealed class TimerComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				Timer timer = x as Timer;
				if (timer == null)
				{
					return -1;
				}
				Timer timer2 = y as Timer;
				if (timer2 == null)
				{
					return 1;
				}
				long num = timer.next_run - timer2.next_run;
				if (num == 0L)
				{
					return (x != y) ? (-1) : 0;
				}
				return (num > 0) ? 1 : (-1);
			}
		}

		private sealed class Scheduler
		{
			private static Scheduler instance;

			private SortedList list;

			public static Scheduler Instance
			{
				get
				{
					return instance;
				}
			}

			private Scheduler()
			{
				list = new SortedList(new TimerComparer(), 1024);
				Thread thread = new Thread(SchedulerThread);
				thread.IsBackground = true;
				thread.Start();
			}

			static Scheduler()
			{
				instance = new Scheduler();
			}

			public void Remove(Timer timer)
			{
				if (timer.next_run == 0L || timer.next_run == long.MaxValue)
				{
					return;
				}
				lock (this)
				{
					InternalRemove(timer);
				}
			}

			public void Change(Timer timer, long new_next_run)
			{
				lock (this)
				{
					InternalRemove(timer);
					if (new_next_run == long.MaxValue)
					{
						timer.next_run = new_next_run;
					}
					else if (!timer.disposed)
					{
						timer.next_run = new_next_run;
						Add(timer);
						if (list.GetByIndex(0) == timer)
						{
							Monitor.Pulse(this);
						}
					}
				}
			}

			private void Add(Timer timer)
			{
				int num = list.IndexOfKey(timer);
				if (num != -1)
				{
					bool flag = ((long.MaxValue - timer.next_run > 20000) ? true : false);
					Timer timer2;
					do
					{
						num++;
						if (flag)
						{
							timer.next_run++;
						}
						else
						{
							timer.next_run--;
						}
						if (num >= list.Count)
						{
							break;
						}
						timer2 = (Timer)list.GetByIndex(num);
					}
					while (timer2.next_run == timer.next_run);
				}
				list.Add(timer, timer);
			}

			private int InternalRemove(Timer timer)
			{
				int num = list.IndexOfKey(timer);
				if (num >= 0)
				{
					list.RemoveAt(num);
				}
				return num;
			}

			private void SchedulerThread()
			{
				Thread.CurrentThread.Name = "Timer-Scheduler";
				ArrayList arrayList = new ArrayList(512);
				while (true)
				{
					long timeMonotonic = DateTime.GetTimeMonotonic();
					lock (this)
					{
						int num = list.Count;
						int num2;
						for (num2 = 0; num2 < num; num2++)
						{
							Timer timer = (Timer)list.GetByIndex(num2);
							if (timer.next_run > timeMonotonic)
							{
								break;
							}
							list.RemoveAt(num2);
							num--;
							num2--;
							ThreadPool.QueueUserWorkItem(timer.callback.Invoke, timer.state);
							long period_ms = timer.period_ms;
							long due_time_ms = timer.due_time_ms;
							if (period_ms == -1 || ((period_ms == 0L || period_ms == -1) && due_time_ms != -1))
							{
								timer.next_run = long.MaxValue;
							}
							else
							{
								timer.next_run = DateTime.GetTimeMonotonic() + 10000 * timer.period_ms;
								arrayList.Add(timer);
							}
						}
						num = arrayList.Count;
						for (num2 = 0; num2 < num; num2++)
						{
							Timer timer2 = (Timer)arrayList[num2];
							Add(timer2);
						}
						arrayList.Clear();
						ShrinkIfNeeded(arrayList, 512);
						int capacity = list.Capacity;
						num = list.Count;
						if (capacity > 1024 && num > 0 && capacity / num > 3)
						{
							list.Capacity = num * 2;
						}
						long num3 = long.MaxValue;
						if (list.Count > 0)
						{
							num3 = ((Timer)list.GetByIndex(0)).next_run;
						}
						int num4 = -1;
						if (num3 != long.MaxValue)
						{
							long num5 = num3 - DateTime.GetTimeMonotonic();
							num4 = (int)(num5 / 10000);
							if (num4 < 0)
							{
								num4 = 0;
							}
						}
						Monitor.Wait(this, num4);
					}
				}
			}

			private void ShrinkIfNeeded(ArrayList list, int initial)
			{
				int capacity = list.Capacity;
				int count = list.Count;
				if (capacity > initial && count > 0 && capacity / count > 3)
				{
					list.Capacity = count * 2;
				}
			}
		}

		private const long MaxValue = 4294967294L;

		private static Scheduler scheduler = Scheduler.Instance;

		private TimerCallback callback;

		private object state;

		private long due_time_ms;

		private long period_ms;

		private long next_run;

		private bool disposed;

		public Timer(TimerCallback callback, object state, int dueTime, int period)
		{
			Init(callback, state, dueTime, period);
		}

		public Timer(TimerCallback callback, object state, long dueTime, long period)
		{
			Init(callback, state, dueTime, period);
		}

		public Timer(TimerCallback callback, object state, TimeSpan dueTime, TimeSpan period)
		{
			Init(callback, state, (long)dueTime.TotalMilliseconds, (long)period.TotalMilliseconds);
		}

		[CLSCompliant(false)]
		public Timer(TimerCallback callback, object state, uint dueTime, uint period)
		{
			Init(callback, state, (dueTime != uint.MaxValue) ? ((long)dueTime) : (-1L), (period != uint.MaxValue) ? ((long)period) : (-1L));
		}

		public Timer(TimerCallback callback)
		{
			Init(callback, this, -1L, -1L);
		}

		private void Init(TimerCallback callback, object state, long dueTime, long period)
		{
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}
			this.callback = callback;
			this.state = state;
			Change(dueTime, period, true);
		}

		public bool Change(int dueTime, int period)
		{
			return Change(dueTime, period, false);
		}

		public bool Change(TimeSpan dueTime, TimeSpan period)
		{
			return Change((long)dueTime.TotalMilliseconds, (long)period.TotalMilliseconds, false);
		}

		[CLSCompliant(false)]
		public bool Change(uint dueTime, uint period)
		{
			long dueTime2 = ((dueTime != uint.MaxValue) ? ((long)dueTime) : (-1L));
			long period2 = ((period != uint.MaxValue) ? ((long)period) : (-1L));
			return Change(dueTime2, period2, false);
		}

		public void Dispose()
		{
			if (!disposed)
			{
				disposed = true;
				scheduler.Remove(this);
			}
		}

		public bool Change(long dueTime, long period)
		{
			return Change(dueTime, period, false);
		}

		private bool Change(long dueTime, long period, bool first)
		{
			if (dueTime > 4294967294u)
			{
				throw new ArgumentOutOfRangeException("Due time too large");
			}
			if (period > 4294967294u)
			{
				throw new ArgumentOutOfRangeException("Period too large");
			}
			if (dueTime < -1)
			{
				throw new ArgumentOutOfRangeException("dueTime");
			}
			if (period < -1)
			{
				throw new ArgumentOutOfRangeException("period");
			}
			if (disposed)
			{
				return false;
			}
			due_time_ms = dueTime;
			period_ms = period;
			long new_next_run;
			if (dueTime == 0L)
			{
				new_next_run = 0L;
			}
			else if (dueTime < 0)
			{
				new_next_run = long.MaxValue;
				if (first)
				{
					next_run = new_next_run;
					return true;
				}
			}
			else
			{
				new_next_run = dueTime * 10000 + DateTime.GetTimeMonotonic();
			}
			scheduler.Change(this, new_next_run);
			return true;
		}

		public bool Dispose(WaitHandle notifyObject)
		{
			if (notifyObject == null)
			{
				throw new ArgumentNullException("notifyObject");
			}
			Dispose();
			NativeEventCalls.SetEvent_internal(notifyObject.Handle);
			return true;
		}
	}
}
