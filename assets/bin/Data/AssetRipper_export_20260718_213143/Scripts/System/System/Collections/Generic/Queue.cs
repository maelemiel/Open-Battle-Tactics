using System.Runtime.InteropServices;

namespace System.Collections.Generic
{
	[Serializable]
	[ComVisible(false)]
	public class Queue<T> : IEnumerable<T>, ICollection, IEnumerable
	{
		[Serializable]
		public struct Enumerator : IEnumerator, IDisposable, IEnumerator<T>
		{
			private const int NOT_STARTED = -2;

			private const int FINISHED = -1;

			private Queue<T> q;

			private int idx;

			private int ver;

			object IEnumerator.Current
			{
				get
				{
					return Current;
				}
			}

			public T Current
			{
				get
				{
					if (idx < 0)
					{
						throw new InvalidOperationException();
					}
					return q._array[(q._size - 1 - idx + q._head) % q._array.Length];
				}
			}

			internal Enumerator(Queue<T> q)
			{
				this.q = q;
				idx = -2;
				ver = q._version;
			}

			void IEnumerator.Reset()
			{
				if (ver != q._version)
				{
					throw new InvalidOperationException();
				}
				idx = -2;
			}

			public void Dispose()
			{
				idx = -2;
			}

			public bool MoveNext()
			{
				if (ver != q._version)
				{
					throw new InvalidOperationException();
				}
				if (idx == -2)
				{
					idx = q._size;
				}
				return idx != -1 && --idx != -1;
			}
		}

		private T[] _array;

		private int _head;

		private int _tail;

		private int _size;

		private int _version;

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		public int Count
		{
			get
			{
				return _size;
			}
		}

		public Queue()
		{
			_array = new T[0];
		}

		public Queue(int count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			_array = new T[count];
		}

		public Queue(IEnumerable<T> collection)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			ICollection<T> collection2 = collection as ICollection<T>;
			_array = new T[(collection2 != null) ? collection2.Count : 0];
			foreach (T item in collection)
			{
				Enqueue(item);
			}
		}

		void ICollection.CopyTo(Array array, int idx)
		{
			if (array == null)
			{
				throw new ArgumentNullException();
			}
			if ((uint)idx > (uint)array.Length)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (array.Length - idx < _size)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (_size == 0)
			{
				return;
			}
			try
			{
				int num = _array.Length;
				int num2 = num - _head;
				Array.Copy(_array, _head, array, idx, Math.Min(_size, num2));
				if (_size > num2)
				{
					Array.Copy(_array, 0, array, idx + num2, _size - num2);
				}
			}
			catch (ArrayTypeMismatchException)
			{
				throw new ArgumentException();
			}
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Clear()
		{
			Array.Clear(_array, 0, _array.Length);
			_head = (_tail = (_size = 0));
			_version++;
		}

		public bool Contains(T item)
		{
			if (item == null)
			{
				using (Enumerator enumerator = GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						T current = enumerator.Current;
						if (current == null)
						{
							return true;
						}
					}
				}
			}
			else
			{
				using (Enumerator enumerator2 = GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						T current2 = enumerator2.Current;
						if (item.Equals(current2))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public void CopyTo(T[] array, int idx)
		{
			if (array == null)
			{
				throw new ArgumentNullException();
			}
			((ICollection)this).CopyTo((Array)array, idx);
		}

		public T Dequeue()
		{
			T result = Peek();
			_array[_head] = default(T);
			if (++_head == _array.Length)
			{
				_head = 0;
			}
			_size--;
			_version++;
			return result;
		}

		public T Peek()
		{
			if (_size == 0)
			{
				throw new InvalidOperationException();
			}
			return _array[_head];
		}

		public void Enqueue(T item)
		{
			if (_size == _array.Length || _tail == _array.Length)
			{
				SetCapacity(Math.Max(Math.Max(_size, _tail) * 2, 4));
			}
			_array[_tail] = item;
			if (++_tail == _array.Length)
			{
				_tail = 0;
			}
			_size++;
			_version++;
		}

		public T[] ToArray()
		{
			T[] array = new T[_size];
			CopyTo(array, 0);
			return array;
		}

		public void TrimExcess()
		{
			if ((double)_size < (double)_array.Length * 0.9)
			{
				SetCapacity(_size);
			}
		}

		private void SetCapacity(int new_size)
		{
			if (new_size != _array.Length)
			{
				if (new_size < _size)
				{
					throw new InvalidOperationException("shouldnt happen");
				}
				T[] array = new T[new_size];
				if (_size > 0)
				{
					CopyTo(array, 0);
				}
				_array = array;
				_tail = _size;
				_head = 0;
				_version++;
			}
		}

		public Enumerator GetEnumerator()
		{
			return new Enumerator(this);
		}
	}
}
