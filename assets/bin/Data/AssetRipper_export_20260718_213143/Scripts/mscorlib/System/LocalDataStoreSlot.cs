using System.Runtime.InteropServices;
using System.Threading;

namespace System
{
	[ComVisible(true)]
	public sealed class LocalDataStoreSlot
	{
		internal int slot;

		internal bool thread_local;

		private static object lock_obj = new object();

		private static bool[] slot_bitmap_thread;

		private static bool[] slot_bitmap_context;

		internal LocalDataStoreSlot(bool in_thread)
		{
			thread_local = in_thread;
			lock (lock_obj)
			{
				bool[] array = ((!in_thread) ? slot_bitmap_context : slot_bitmap_thread);
				int i;
				if (array != null)
				{
					for (i = 0; i < array.Length; i++)
					{
						if (!array[i])
						{
							slot = i;
							array[i] = true;
							return;
						}
					}
					bool[] array2 = new bool[i + 2];
					array.CopyTo(array2, 0);
					array = array2;
				}
				else
				{
					array = new bool[2];
					i = 0;
				}
				array[i] = true;
				slot = i;
				if (in_thread)
				{
					slot_bitmap_thread = array;
				}
				else
				{
					slot_bitmap_context = array;
				}
			}
		}

		~LocalDataStoreSlot()
		{
			Thread.FreeLocalSlotValues(slot, thread_local);
			lock (lock_obj)
			{
				if (thread_local)
				{
					slot_bitmap_thread[slot] = false;
				}
				else
				{
					slot_bitmap_context[slot] = false;
				}
			}
		}
	}
}
