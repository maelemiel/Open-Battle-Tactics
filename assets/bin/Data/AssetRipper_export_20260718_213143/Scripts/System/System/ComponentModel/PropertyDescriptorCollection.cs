using System.Collections;

namespace System.ComponentModel
{
	public class PropertyDescriptorCollection : ICollection, IDictionary, IEnumerable, IList
	{
		public static readonly PropertyDescriptorCollection Empty = new PropertyDescriptorCollection(null, true);

		private ArrayList properties;

		private bool readOnly;

		bool IDictionary.IsFixedSize
		{
			get
			{
				return ((IList)this).IsFixedSize;
			}
		}

		bool IList.IsFixedSize
		{
			get
			{
				return readOnly;
			}
		}

		bool IDictionary.IsReadOnly
		{
			get
			{
				return ((IList)this).IsReadOnly;
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				return readOnly;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		int ICollection.Count
		{
			get
			{
				return Count;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return null;
			}
		}

		ICollection IDictionary.Keys
		{
			get
			{
				string[] array = new string[properties.Count];
				int num = 0;
				foreach (PropertyDescriptor property in properties)
				{
					array[num++] = property.Name;
				}
				return array;
			}
		}

		ICollection IDictionary.Values
		{
			get
			{
				return (ICollection)properties.Clone();
			}
		}

		object IDictionary.this[object key]
		{
			get
			{
				if (!(key is string))
				{
					return null;
				}
				return this[(string)key];
			}
			set
			{
				if (readOnly)
				{
					throw new NotSupportedException();
				}
				if (!(key is string) || !(value is PropertyDescriptor))
				{
					throw new ArgumentException();
				}
				int num = properties.IndexOf(value);
				if (num == -1)
				{
					Add((PropertyDescriptor)value);
				}
				else
				{
					properties[num] = value;
				}
			}
		}

		object IList.this[int index]
		{
			get
			{
				return properties[index];
			}
			set
			{
				if (readOnly)
				{
					throw new NotSupportedException();
				}
				properties[index] = value;
			}
		}

		public int Count
		{
			get
			{
				return properties.Count;
			}
		}

		public virtual PropertyDescriptor this[string s]
		{
			get
			{
				return Find(s, false);
			}
		}

		public virtual PropertyDescriptor this[int index]
		{
			get
			{
				return (PropertyDescriptor)properties[index];
			}
		}

		public PropertyDescriptorCollection(PropertyDescriptor[] properties)
		{
			this.properties = new ArrayList();
			if (properties != null)
			{
				this.properties.AddRange(properties);
			}
		}

		public PropertyDescriptorCollection(PropertyDescriptor[] properties, bool readOnly)
			: this(properties)
		{
			this.readOnly = readOnly;
		}

		private PropertyDescriptorCollection()
		{
		}

		int IList.Add(object value)
		{
			return Add((PropertyDescriptor)value);
		}

		void IDictionary.Add(object key, object value)
		{
			if (!(value is PropertyDescriptor))
			{
				throw new ArgumentException("value");
			}
			Add((PropertyDescriptor)value);
		}

		void IList.Clear()
		{
			Clear();
		}

		void IDictionary.Clear()
		{
			Clear();
		}

		bool IList.Contains(object value)
		{
			return Contains((PropertyDescriptor)value);
		}

		bool IDictionary.Contains(object value)
		{
			return Contains((PropertyDescriptor)value);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		[System.MonoTODO]
		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		int IList.IndexOf(object value)
		{
			return IndexOf((PropertyDescriptor)value);
		}

		void IList.Insert(int index, object value)
		{
			Insert(index, (PropertyDescriptor)value);
		}

		void IDictionary.Remove(object value)
		{
			Remove((PropertyDescriptor)value);
		}

		void IList.Remove(object value)
		{
			Remove((PropertyDescriptor)value);
		}

		void IList.RemoveAt(int index)
		{
			RemoveAt(index);
		}

		public int Add(PropertyDescriptor value)
		{
			if (readOnly)
			{
				throw new NotSupportedException();
			}
			properties.Add(value);
			return properties.Count - 1;
		}

		public void Clear()
		{
			if (readOnly)
			{
				throw new NotSupportedException();
			}
			properties.Clear();
		}

		public bool Contains(PropertyDescriptor value)
		{
			return properties.Contains(value);
		}

		public void CopyTo(Array array, int index)
		{
			properties.CopyTo(array, index);
		}

		public virtual PropertyDescriptor Find(string name, bool ignoreCase)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			for (int i = 0; i < properties.Count; i++)
			{
				PropertyDescriptor propertyDescriptor = (PropertyDescriptor)properties[i];
				if (ignoreCase)
				{
					if (string.Compare(name, propertyDescriptor.Name, StringComparison.OrdinalIgnoreCase) == 0)
					{
						return propertyDescriptor;
					}
				}
				else if (string.Compare(name, propertyDescriptor.Name, StringComparison.Ordinal) == 0)
				{
					return propertyDescriptor;
				}
			}
			return null;
		}

		public virtual IEnumerator GetEnumerator()
		{
			return properties.GetEnumerator();
		}

		public int IndexOf(PropertyDescriptor value)
		{
			return properties.IndexOf(value);
		}

		public void Insert(int index, PropertyDescriptor value)
		{
			if (readOnly)
			{
				throw new NotSupportedException();
			}
			properties.Insert(index, value);
		}

		public void Remove(PropertyDescriptor value)
		{
			if (readOnly)
			{
				throw new NotSupportedException();
			}
			properties.Remove(value);
		}

		public void RemoveAt(int index)
		{
			if (readOnly)
			{
				throw new NotSupportedException();
			}
			properties.RemoveAt(index);
		}

		private PropertyDescriptorCollection CloneCollection()
		{
			PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection();
			propertyDescriptorCollection.properties = (ArrayList)properties.Clone();
			return propertyDescriptorCollection;
		}

		public virtual PropertyDescriptorCollection Sort()
		{
			PropertyDescriptorCollection propertyDescriptorCollection = CloneCollection();
			propertyDescriptorCollection.InternalSort((IComparer)null);
			return propertyDescriptorCollection;
		}

		public virtual PropertyDescriptorCollection Sort(IComparer comparer)
		{
			PropertyDescriptorCollection propertyDescriptorCollection = CloneCollection();
			propertyDescriptorCollection.InternalSort(comparer);
			return propertyDescriptorCollection;
		}

		public virtual PropertyDescriptorCollection Sort(string[] order)
		{
			PropertyDescriptorCollection propertyDescriptorCollection = CloneCollection();
			propertyDescriptorCollection.InternalSort(order);
			return propertyDescriptorCollection;
		}

		public virtual PropertyDescriptorCollection Sort(string[] order, IComparer comparer)
		{
			PropertyDescriptorCollection propertyDescriptorCollection = CloneCollection();
			if (order != null)
			{
				ArrayList arrayList = propertyDescriptorCollection.ExtractItems(order);
				propertyDescriptorCollection.InternalSort(comparer);
				arrayList.AddRange(propertyDescriptorCollection.properties);
				propertyDescriptorCollection.properties = arrayList;
			}
			else
			{
				propertyDescriptorCollection.InternalSort(comparer);
			}
			return propertyDescriptorCollection;
		}

		protected void InternalSort(IComparer ic)
		{
			if (ic == null)
			{
				ic = MemberDescriptor.DefaultComparer;
			}
			properties.Sort(ic);
		}

		protected void InternalSort(string[] order)
		{
			if (order != null)
			{
				ArrayList arrayList = ExtractItems(order);
				InternalSort((IComparer)null);
				arrayList.AddRange(properties);
				properties = arrayList;
			}
			else
			{
				InternalSort((IComparer)null);
			}
		}

		private ArrayList ExtractItems(string[] names)
		{
			ArrayList arrayList = new ArrayList(properties.Count);
			object[] array = new object[names.Length];
			for (int i = 0; i < properties.Count; i++)
			{
				PropertyDescriptor propertyDescriptor = (PropertyDescriptor)properties[i];
				int num = Array.IndexOf(names, propertyDescriptor.Name);
				if (num != -1)
				{
					array[num] = propertyDescriptor;
					properties.RemoveAt(i);
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

		internal PropertyDescriptorCollection Filter(Attribute[] attributes)
		{
			ArrayList arrayList = new ArrayList();
			foreach (PropertyDescriptor property in properties)
			{
				if (property.Attributes.Contains(attributes))
				{
					arrayList.Add(property);
				}
			}
			PropertyDescriptor[] array = new PropertyDescriptor[arrayList.Count];
			arrayList.CopyTo(array);
			return new PropertyDescriptorCollection(array, true);
		}
	}
}
