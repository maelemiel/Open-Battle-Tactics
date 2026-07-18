using System.Collections;
using System.ComponentModel.Design;

namespace System.ComponentModel
{
	internal abstract class Info
	{
		private Type _infoType;

		private EventDescriptor _defaultEvent;

		private bool _gotDefaultEvent;

		private PropertyDescriptor _defaultProperty;

		private bool _gotDefaultProperty;

		private AttributeCollection _attributes;

		public Type InfoType
		{
			get
			{
				return _infoType;
			}
		}

		public Info(Type infoType)
		{
			_infoType = infoType;
		}

		public abstract AttributeCollection GetAttributes();

		public abstract EventDescriptorCollection GetEvents();

		public abstract PropertyDescriptorCollection GetProperties();

		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			EventDescriptorCollection events = GetEvents();
			if (attributes == null)
			{
				return events;
			}
			return events.Filter(attributes);
		}

		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection properties = GetProperties();
			if (attributes == null)
			{
				return properties;
			}
			return properties.Filter(attributes);
		}

		public EventDescriptor GetDefaultEvent()
		{
			if (_gotDefaultEvent)
			{
				return _defaultEvent;
			}
			DefaultEventAttribute defaultEventAttribute = (DefaultEventAttribute)GetAttributes()[typeof(DefaultEventAttribute)];
			if (defaultEventAttribute == null || defaultEventAttribute.Name == null)
			{
				_defaultEvent = null;
			}
			else
			{
				EventDescriptorCollection events = GetEvents();
				_defaultEvent = events[defaultEventAttribute.Name];
			}
			_gotDefaultEvent = true;
			return _defaultEvent;
		}

		public PropertyDescriptor GetDefaultProperty()
		{
			if (_gotDefaultProperty)
			{
				return _defaultProperty;
			}
			DefaultPropertyAttribute defaultPropertyAttribute = (DefaultPropertyAttribute)GetAttributes()[typeof(DefaultPropertyAttribute)];
			if (defaultPropertyAttribute == null || defaultPropertyAttribute.Name == null)
			{
				_defaultProperty = null;
			}
			else
			{
				PropertyDescriptorCollection properties = GetProperties();
				_defaultProperty = properties[defaultPropertyAttribute.Name];
			}
			_gotDefaultProperty = true;
			return _defaultProperty;
		}

		protected AttributeCollection GetAttributes(IComponent comp)
		{
			if (_attributes != null)
			{
				return _attributes;
			}
			bool flag = true;
			ArrayList arrayList = new ArrayList();
			object[] customAttributes = _infoType.GetCustomAttributes(false);
			for (int i = 0; i < customAttributes.Length; i++)
			{
				Attribute value = (Attribute)customAttributes[i];
				arrayList.Add(value);
			}
			Type baseType = _infoType.BaseType;
			while (baseType != null && baseType != typeof(object))
			{
				object[] customAttributes2 = baseType.GetCustomAttributes(false);
				for (int j = 0; j < customAttributes2.Length; j++)
				{
					Attribute value2 = (Attribute)customAttributes2[j];
					arrayList.Add(value2);
				}
				baseType = baseType.BaseType;
			}
			Type[] interfaces = _infoType.GetInterfaces();
			foreach (Type componentType in interfaces)
			{
				foreach (Attribute attribute2 in TypeDescriptor.GetAttributes(componentType))
				{
					arrayList.Add(attribute2);
				}
			}
			Hashtable hashtable = new Hashtable();
			for (int num = arrayList.Count - 1; num >= 0; num--)
			{
				Attribute attribute = (Attribute)arrayList[num];
				hashtable[attribute.TypeId] = attribute;
			}
			if (comp != null && comp.Site != null)
			{
				ITypeDescriptorFilterService typeDescriptorFilterService = (ITypeDescriptorFilterService)comp.Site.GetService(typeof(ITypeDescriptorFilterService));
				if (typeDescriptorFilterService != null)
				{
					flag = typeDescriptorFilterService.FilterAttributes(comp, hashtable);
				}
			}
			Attribute[] array = new Attribute[hashtable.Values.Count];
			hashtable.Values.CopyTo(array, 0);
			AttributeCollection attributeCollection = new AttributeCollection(array);
			if (flag)
			{
				_attributes = attributeCollection;
			}
			return attributeCollection;
		}
	}
}
