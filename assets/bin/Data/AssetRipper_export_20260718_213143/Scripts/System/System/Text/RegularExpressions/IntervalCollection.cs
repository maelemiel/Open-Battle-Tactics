using System.Collections;

namespace System.Text.RegularExpressions
{
	internal class IntervalCollection : ICollection, IEnumerable
	{
		private class Enumerator : IEnumerator
		{
			private IList list;

			private int ptr;

			public object Current
			{
				get
				{
					if (ptr >= list.Count)
					{
						throw new InvalidOperationException();
					}
					return list[ptr];
				}
			}

			public Enumerator(IList list)
			{
				this.list = list;
				Reset();
			}

			public bool MoveNext()
			{
				if (ptr > list.Count)
				{
					throw new InvalidOperationException();
				}
				return ++ptr < list.Count;
			}

			public void Reset()
			{
				ptr = -1;
			}
		}

		public delegate double CostDelegate(System.Text.RegularExpressions.Interval i);

		private ArrayList intervals;

		public System.Text.RegularExpressions.Interval this[int i]
		{
			get
			{
				return (System.Text.RegularExpressions.Interval)intervals[i];
			}
			set
			{
				intervals[i] = value;
			}
		}

		public int Count
		{
			get
			{
				return intervals.Count;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public object SyncRoot
		{
			get
			{
				return intervals;
			}
		}

		public IntervalCollection()
		{
			intervals = new ArrayList();
		}

		public void Add(System.Text.RegularExpressions.Interval i)
		{
			intervals.Add(i);
		}

		public void Clear()
		{
			intervals.Clear();
		}

		public void Sort()
		{
			intervals.Sort();
		}

		public void Normalize()
		{
			intervals.Sort();
			int num = 0;
			while (num < intervals.Count - 1)
			{
				System.Text.RegularExpressions.Interval interval = (System.Text.RegularExpressions.Interval)intervals[num];
				System.Text.RegularExpressions.Interval i = (System.Text.RegularExpressions.Interval)intervals[num + 1];
				if (!interval.IsDisjoint(i) || interval.IsAdjacent(i))
				{
					interval.Merge(i);
					intervals[num] = interval;
					intervals.RemoveAt(num + 1);
				}
				else
				{
					num++;
				}
			}
		}

		public System.Text.RegularExpressions.IntervalCollection GetMetaCollection(CostDelegate cost_del)
		{
			System.Text.RegularExpressions.IntervalCollection intervalCollection = new System.Text.RegularExpressions.IntervalCollection();
			Normalize();
			Optimize(0, Count - 1, intervalCollection, cost_del);
			intervalCollection.intervals.Sort();
			return intervalCollection;
		}

		private void Optimize(int begin, int end, System.Text.RegularExpressions.IntervalCollection meta, CostDelegate cost_del)
		{
			System.Text.RegularExpressions.Interval i = default(System.Text.RegularExpressions.Interval);
			i.contiguous = false;
			int num = -1;
			int num2 = -1;
			double num3 = 0.0;
			for (int j = begin; j <= end; j++)
			{
				i.low = this[j].low;
				double num4 = 0.0;
				for (int k = j; k <= end; k++)
				{
					i.high = this[k].high;
					num4 += cost_del(this[k]);
					double num5 = cost_del(i);
					if (num5 < num4 && num4 > num3)
					{
						num = j;
						num2 = k;
						num3 = num4;
					}
				}
			}
			if (num < 0)
			{
				for (int l = begin; l <= end; l++)
				{
					meta.Add(this[l]);
				}
				return;
			}
			i.low = this[num].low;
			i.high = this[num2].high;
			meta.Add(i);
			if (num > begin)
			{
				Optimize(begin, num - 1, meta, cost_del);
			}
			if (num2 < end)
			{
				Optimize(num2 + 1, end, meta, cost_del);
			}
		}

		public void CopyTo(Array array, int index)
		{
			foreach (System.Text.RegularExpressions.Interval interval in intervals)
			{
				if (index > array.Length)
				{
					break;
				}
				array.SetValue(interval, index++);
			}
		}

		public IEnumerator GetEnumerator()
		{
			return new Enumerator(intervals);
		}
	}
}
