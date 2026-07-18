using System.Collections;
using System.ComponentModel.Design;
using System.Reflection;

namespace System.ComponentModel
{
	internal class ComponentInfo : System.ComponentModel.Info
	{
		private IComponent _component;

		private EventDescriptorCollection _events;

		private PropertyDescriptorCollection _properties;

		public ComponentInfo(IComponent component)
			: base(component.GetType())
		{
			_component = component;
		}

		public override AttributeCollection GetAttributes()
		{
			return GetAttributes(_component);
		}

		public override EventDescriptorCollection GetEvents()
		{
			if (_events != null)
			{
				return _events;
			}
			bool flag = true;
			EventInfo[] events = _component.GetType().GetEvents();
			Hashtable hashtable = new Hashtable();
			EventInfo[] array = events;
			foreach (EventInfo eventInfo in array)
			{
				hashtable[eventInfo.Name] = new System.ComponentModel.ReflectionEventDescriptor(eventInfo);
			}
			if (_component.Site != null)
			{
				ITypeDescriptorFilterService typeDescriptorFilterService = (ITypeDescriptorFilterService)_component.Site.GetService(typeof(ITypeDescriptorFilterService));
				if (typeDescriptorFilterService != null)
				{
					flag = typeDescriptorFilterService.FilterEvents(_component, hashtable);
				}
			}
			ArrayList arrayList = new ArrayList();
			arrayList.AddRange(hashtable.Values);
			EventDescriptorCollection eventDescriptorCollection = new EventDescriptorCollection(arrayList);
			if (flag)
			{
				_events = eventDescriptorCollection;
			}
			return eventDescriptorCollection;
		}

		public override PropertyDescriptorCollection GetProperties()
		{
			if (_properties != null)
			{
				return _properties;
			}
			bool flag = true;
			PropertyInfo[] properties = _component.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
			Hashtable hashtable = new Hashtable();
			for (int num = properties.Length - 1; num >= 0; num--)
			{
				hashtable[properties[num].Name] = new System.ComponentModel.ReflectionPropertyDescriptor(properties[num]);
			}
			if (_component.Site != null)
			{
				ITypeDescriptorFilterService typeDescriptorFilterService = (ITypeDescriptorFilterService)_component.Site.GetService(typeof(ITypeDescriptorFilterService));
				if (typeDescriptorFilterService != null)
				{
					flag = typeDescriptorFilterService.FilterProperties(_component, hashtable);
				}
			}
			PropertyDescriptor[] array = new PropertyDescriptor[hashtable.Values.Count];
			hashtable.Values.CopyTo(array, 0);
			PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(array, true);
			if (flag)
			{
				_properties = propertyDescriptorCollection;
			}
			return propertyDescriptorCollection;
		}
	}
}
