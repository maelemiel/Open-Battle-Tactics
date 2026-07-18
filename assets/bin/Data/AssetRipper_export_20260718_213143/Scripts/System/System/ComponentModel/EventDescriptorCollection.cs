using System.Collections;
using System.Runtime.InteropServices;

namespace System.ComponentModel
{
	[ComVisible(true)]
	public class EventDescriptorCollection : ICollection, IEnumerable, IList
	{
		private ArrayList eventList = new ArrayList();

		private bool isReadOnly;

		public static readonly EventDescriptorCollection Empty = new EventDescriptorCollection(null, true);

		int ICollection.Count
		{
			get
			{
				return Count;
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return isReadOnly;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return isReadOnly;
			}
		}

		object IList.this[int index]
		{
			get
			{
				return eventList[index];
			}
			set
			{
				if (isReadOnly)
				{
					throw new NotSupportedException("The collection is read-only");
				}
				eventList[index] = value;
			}
		}

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
				return null;
			}
		}

		public int Count
		{
			get
			{
				return eventList.Count;
			}
		}

		public virtual EventDescriptor this[string name]
		{
			get
			{
				return Find(name, false);
			}
		}

		public virtual EventDescriptor this[int index]
		{
			get
			{
				return (EventDescriptor)eventList[index];
			}
		}

		private EventDescriptorCollection()
		{
		}

		internal EventDescriptorCollection(ArrayList list)
		{
			eventList = list;
		}

		public EventDescriptorCollection(EventDescriptor[] events)
			: this(events, false)
		{
		}

		public EventDescriptorCollection(EventDescriptor[] events, bool readOnly)
		{
			isReadOnly = readOnly;
			if (events != null)
			{
				for (int i = 0; i < events.Length; i++)
				{
					Add(events[i]);
				}
			}
		}

		void IList.Clear()
		{
			Clear();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		void IList.RemoveAt(int index)
		{
			RemoveAt(index);
		}

		int IList.Add(object value)
		{
			return Add((EventDescriptor)value);
		}

		bool IList.Contains(object value)
		{
			return Contains((EventDescriptor)value);
		}

		int IList.IndexOf(object value)
		{
			return IndexOf((EventDescriptor)value);
		}

		void IList.Insert(int index, object value)
		{
			Insert(index, (EventDescriptor)value);
		}

		void IList.Remove(object value)
		{
			Remove((EventDescriptor)value);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			eventList.CopyTo(array, index);
		}

		public int Add(EventDescriptor value)
		{
			if (isReadOnly)
			{
				throw new NotSupportedException("The collection is read-only");
			}
			return eventList.Add(value);
		}

		public void Clear()
		{
			if (isReadOnly)
			{
				throw new NotSupportedException("The collection is read-only");
			}
			eventList.Clear();
		}

		public bool Contains(EventDescriptor value)
		{
			return eventList.Contains(value);
		}

		public virtual EventDescriptor Find(string name, bool ignoreCase)
		{
			foreach (EventDescriptor @event in eventList)
			{
				if (ignoreCase)
				{
					if (string.Compare(name, @event.Name, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return @event;
					}
				}
				else if (string.Compare(name, @event.Name, StringComparison.Ordinal) == 0)
				{
					return @event;
				}
			}
			return null;
		}

		public IEnumerator GetEnumerator()
		{
			return eventList.GetEnumerator();
		}

		public int IndexOf(EventDescriptor value)
		{
			return eventList.IndexOf(value);
		}

		public void Insert(int index, EventDescriptor value)
		{
			if (isReadOnly)
			{
				throw new NotSupportedException("The collection is read-only");
			}
			eventList.Insert(index, value);
		}

		public void Remove(EventDescriptor value)
		{
			if (isReadOnly)
			{
				throw new NotSupportedException("The collection is read-only");
			}
			eventList.Remove(value);
		}

		public void RemoveAt(int index)
		{
			if (isReadOnly)
			{
				throw new NotSupportedException("The collection is read-only");
			}
			eventList.RemoveAt(index);
		}

		public virtual EventDescriptorCollection Sort()
		{
			EventDescriptorCollection eventDescriptorCollection = CloneCollection();
			eventDescriptorCollection.InternalSort((IComparer)null);
			return eventDescriptorCollection;
		}

		public virtual EventDescriptorCollection Sort(IComparer comparer)
		{
			EventDescriptorCollection eventDescriptorCollection = CloneCollection();
			eventDescriptorCollection.InternalSort(comparer);
			return eventDescriptorCollection;
		}

		public virtual EventDescriptorCollection Sort(string[] order)
		{
			EventDescriptorCollection eventDescriptorCollection = CloneCollection();
			eventDescriptorCollection.InternalSort(order);
			return eventDescriptorCollection;
		}

		public virtual EventDescriptorCollection Sort(string[] order, IComparer comparer)
		{
			EventDescriptorCollection eventDescriptorCollection = CloneCollection();
			if (order != null)
			{
				ArrayList arrayList = eventDescriptorCollection.ExtractItems(order);
				eventDescriptorCollection.InternalSort(comparer);
				arrayList.AddRange(eventDescriptorCollection.eventList);
				eventDescriptorCollection.eventList = arrayList;
			}
			else
			{
				eventDescriptorCollection.InternalSort(comparer);
			}
			return eventDescriptorCollection;
		}

		protected void InternalSort(IComparer comparer)
		{
			if (comparer == null)
			{
				comparer = MemberDescriptor.DefaultComparer;
			}
			eventList.Sort(comparer);
		}

		protected void InternalSort(string[] order)
		{
			if (order != null)
			{
				ArrayList arrayList = ExtractItems(order);
				InternalSort((IComparer)null);
				arrayList.AddRange(eventList);
				eventList = arrayList;
			}
			else
			{
				InternalSort((IComparer)null);
			}
		}

		private ArrayList ExtractItems(string[] names)
		{
			ArrayList arrayList = new ArrayList(eventList.Count);
			object[] array = new object[names.Length];
			for (int i = 0; i < eventList.Count; i++)
			{
				EventDescriptor eventDescriptor = (EventDescriptor)eventList[i];
				int num = Array.IndexOf(names, eventDescriptor.Name);
				if (num != -1)
				{
					array[num] = eventDescriptor;
					eventList.RemoveAt(i);
					i--;
				}
			}
			object[] array2 = array;
			foreach (object obj in array2)
			{
				if (obj != null)
				{
					arrayList.Add(obj);
				}
			}
			return arrayList;
		}

		private EventDescriptorCollection CloneCollection()
		{
			EventDescriptorCollection eventDescriptorCollection = new EventDescriptorCollection();
			eventDescriptorCollection.eventList = (ArrayList)eventList.Clone();
			return eventDescriptorCollection;
		}

		internal EventDescriptorCollection Filter(Attribute[] attributes)
		{
			EventDescriptorCollection eventDescriptorCollection = new EventDescriptorCollection();
			foreach (EventDescriptor @event in eventList)
			{
				if (@event.Attributes.Contains(attributes))
				{
					eventDescriptorCollection.eventList.Add(@event);
				}
			}
			return eventDescriptorCollection;
		}
	}
}
