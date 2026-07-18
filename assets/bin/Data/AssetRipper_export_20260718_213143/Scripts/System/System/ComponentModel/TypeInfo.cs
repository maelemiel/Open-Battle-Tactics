using System.Collections;
using System.Reflection;

namespace System.ComponentModel
{
	internal class TypeInfo : System.ComponentModel.Info
	{
		private EventDescriptorCollection _events;

		private PropertyDescriptorCollection _properties;

		public TypeInfo(Type t)
			: base(t)
		{
		}

		public override AttributeCollection GetAttributes()
		{
			return GetAttributes(null);
		}

		public override EventDescriptorCollection GetEvents()
		{
			if (_events != null)
			{
				return _events;
			}
			EventInfo[] events = base.InfoType.GetEvents();
			EventDescriptor[] array = new EventDescriptor[events.Length];
			for (int i = 0; i < events.Length; i++)
			{
				array[i] = new System.ComponentModel.ReflectionEventDescriptor(events[i]);
			}
			_events = new EventDescriptorCollection(array);
			return _events;
		}

		public override PropertyDescriptorCollection GetProperties()
		{
			if (_properties != null)
			{
				return _properties;
			}
			Hashtable hashtable = new Hashtable();
			ArrayList arrayList = new ArrayList();
			Type type = base.InfoType;
			while (type != null && type != typeof(object))
			{
				PropertyInfo[] properties = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
				PropertyInfo[] array = properties;
				foreach (PropertyInfo propertyInfo in array)
				{
					if (propertyInfo.GetIndexParameters().Length == 0 && propertyInfo.CanRead && !hashtable.ContainsKey(propertyInfo.Name))
					{
						arrayList.Add(new System.ComponentModel.ReflectionPropertyDescriptor(propertyInfo));
						hashtable.Add(propertyInfo.Name, null);
					}
				}
				type = type.BaseType;
			}
			_properties = new PropertyDescriptorCollection((PropertyDescriptor[])arrayList.ToArray(typeof(PropertyDescriptor)), true);
			return _properties;
		}
	}
}
