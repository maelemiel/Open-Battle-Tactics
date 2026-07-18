using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.ComponentModel
{
	[ComVisible(true)]
	public class AttributeCollection : ICollection, IEnumerable
	{
		private ArrayList attrList = new ArrayList();

		public static readonly AttributeCollection Empty = new AttributeCollection((ArrayList)null);

		bool ICollection.IsSynchronized
		{
			get
			{
				return attrList.IsSynchronized;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return attrList.SyncRoot;
			}
		}

		int ICollection.Count
		{
			get
			{
				return Count;
			}
		}

		public int Count
		{
			get
			{
				return (attrList != null) ? attrList.Count : 0;
			}
		}

		public virtual Attribute this[Type type]
		{
			get
			{
				Attribute attribute = null;
				if (attrList != null)
				{
					foreach (Attribute attr in attrList)
					{
						if (type.IsAssignableFrom(attr.GetType()))
						{
							attribute = attr;
							break;
						}
					}
				}
				if (attribute == null)
				{
					attribute = GetDefaultAttribute(type);
				}
				return attribute;
			}
		}

		public virtual Attribute this[int index]
		{
			get
			{
				return (Attribute)attrList[index];
			}
		}

		internal AttributeCollection(ArrayList attributes)
		{
			if (attributes != null)
			{
				attrList = attributes;
			}
		}

		public AttributeCollection(params Attribute[] attributes)
		{
			if (attributes != null)
			{
				for (int i = 0; i < attributes.Length; i++)
				{
					attrList.Add(attributes[i]);
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public static AttributeCollection FromExisting(AttributeCollection existing, params Attribute[] newAttributes)
		{
			if (existing == null)
			{
				throw new ArgumentNullException("existing");
			}
			AttributeCollection attributeCollection = new AttributeCollection();
			attributeCollection.attrList.AddRange(existing.attrList);
			if (newAttributes != null)
			{
				attributeCollection.attrList.AddRange(newAttributes);
			}
			return attributeCollection;
		}

		public bool Contains(Attribute attr)
		{
			Attribute attribute = this[attr.GetType()];
			if (attribute != null)
			{
				return attr.Equals(attribute);
			}
			return false;
		}

		public bool Contains(Attribute[] attributes)
		{
			if (attributes == null)
			{
				return true;
			}
			foreach (Attribute attr in attributes)
			{
				if (!Contains(attr))
				{
					return false;
				}
			}
			return true;
		}

		public void CopyTo(Array array, int index)
		{
			attrList.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return attrList.GetEnumerator();
		}

		public bool Matches(Attribute attr)
		{
			foreach (Attribute attr2 in attrList)
			{
				if (attr2.Match(attr))
				{
					return true;
				}
			}
			return false;
		}

		public bool Matches(Attribute[] attributes)
		{
			foreach (Attribute attr in attributes)
			{
				if (!Matches(attr))
				{
					return false;
				}
			}
			return true;
		}

		protected Attribute GetDefaultAttribute(Type attributeType)
		{
			Attribute attribute = null;
			BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.Public;
			FieldInfo field = attributeType.GetField("Default", bindingAttr);
			if (field == null)
			{
				ConstructorInfo constructor = attributeType.GetConstructor(Type.EmptyTypes);
				if (constructor != null)
				{
					attribute = constructor.Invoke(null) as Attribute;
				}
				if (attribute != null && !attribute.IsDefaultAttribute())
				{
					attribute = null;
				}
			}
			else
			{
				attribute = (Attribute)field.GetValue(null);
			}
			return attribute;
		}
	}
}
