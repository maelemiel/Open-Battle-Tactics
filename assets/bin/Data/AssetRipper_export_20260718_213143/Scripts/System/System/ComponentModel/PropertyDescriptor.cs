using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.ComponentModel
{
	[ComVisible(true)]
	public abstract class PropertyDescriptor : MemberDescriptor
	{
		private TypeConverter converter;

		private Hashtable notifiers;

		public abstract Type ComponentType { get; }

		public virtual TypeConverter Converter
		{
			get
			{
				if (converter == null && PropertyType != null)
				{
					TypeConverterAttribute typeConverterAttribute = (TypeConverterAttribute)Attributes[typeof(TypeConverterAttribute)];
					if (typeConverterAttribute != null && typeConverterAttribute != TypeConverterAttribute.Default)
					{
						Type typeFromName = GetTypeFromName(typeConverterAttribute.ConverterTypeName);
						if (typeFromName != null && typeof(TypeConverter).IsAssignableFrom(typeFromName))
						{
							converter = (TypeConverter)CreateInstance(typeFromName);
						}
					}
					if (converter == null)
					{
						converter = TypeDescriptor.GetConverter(PropertyType);
					}
				}
				return converter;
			}
		}

		public virtual bool IsLocalizable
		{
			get
			{
				Attribute[] attributeArray = AttributeArray;
				foreach (Attribute attribute in attributeArray)
				{
					if (attribute is LocalizableAttribute)
					{
						return ((LocalizableAttribute)attribute).IsLocalizable;
					}
				}
				return false;
			}
		}

		public abstract bool IsReadOnly { get; }

		public abstract Type PropertyType { get; }

		public virtual bool SupportsChangeEvents
		{
			get
			{
				return false;
			}
		}

		public DesignerSerializationVisibility SerializationVisibility
		{
			get
			{
				Attribute[] attributeArray = AttributeArray;
				foreach (Attribute attribute in attributeArray)
				{
					if (attribute is DesignerSerializationVisibilityAttribute)
					{
						DesignerSerializationVisibilityAttribute designerSerializationVisibilityAttribute = (DesignerSerializationVisibilityAttribute)attribute;
						return designerSerializationVisibilityAttribute.Visibility;
					}
				}
				return DesignerSerializationVisibility.Visible;
			}
		}

		protected PropertyDescriptor(MemberDescriptor reference)
			: base(reference)
		{
		}

		protected PropertyDescriptor(MemberDescriptor reference, Attribute[] attrs)
			: base(reference, attrs)
		{
		}

		protected PropertyDescriptor(string name, Attribute[] attrs)
			: base(name, attrs)
		{
		}

		public virtual void AddValueChanged(object component, EventHandler handler)
		{
			if (component == null)
			{
				throw new ArgumentNullException("component");
			}
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			if (notifiers == null)
			{
				notifiers = new Hashtable();
			}
			EventHandler eventHandler = (EventHandler)notifiers[component];
			if (eventHandler != null)
			{
				eventHandler = (EventHandler)Delegate.Combine(eventHandler, handler);
				notifiers[component] = eventHandler;
			}
			else
			{
				notifiers[component] = handler;
			}
		}

		public virtual void RemoveValueChanged(object component, EventHandler handler)
		{
			if (component == null)
			{
				throw new ArgumentNullException("component");
			}
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			if (notifiers != null)
			{
				EventHandler source = (EventHandler)notifiers[component];
				source = (EventHandler)Delegate.Remove(source, handler);
				if (source == null)
				{
					notifiers.Remove(component);
				}
				else
				{
					notifiers[component] = source;
				}
			}
		}

		protected override void FillAttributes(IList attributeList)
		{
			base.FillAttributes(attributeList);
		}

		protected override object GetInvocationTarget(Type type, object instance)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (instance is CustomTypeDescriptor)
			{
				CustomTypeDescriptor customTypeDescriptor = (CustomTypeDescriptor)instance;
				return customTypeDescriptor.GetPropertyOwner(this);
			}
			return base.GetInvocationTarget(type, instance);
		}

		protected internal EventHandler GetValueChangedHandler(object component)
		{
			if (component == null || notifiers == null)
			{
				return null;
			}
			return (EventHandler)notifiers[component];
		}

		protected virtual void OnValueChanged(object component, EventArgs e)
		{
			if (notifiers != null)
			{
				EventHandler eventHandler = (EventHandler)notifiers[component];
				if (eventHandler != null)
				{
					eventHandler(component, e);
				}
			}
		}

		public abstract object GetValue(object component);

		public abstract void SetValue(object component, object value);

		public abstract void ResetValue(object component);

		public abstract bool CanResetValue(object component);

		public abstract bool ShouldSerializeValue(object component);

		protected object CreateInstance(Type type)
		{
			if (type == null || PropertyType == null)
			{
				return null;
			}
			object obj = null;
			Type[] array = new Type[1] { typeof(Type) };
			ConstructorInfo constructor = type.GetConstructor(array);
			if (constructor != null)
			{
				object[] args = new object[1] { PropertyType };
				return TypeDescriptor.CreateInstance(null, type, array, args);
			}
			return TypeDescriptor.CreateInstance(null, type, null, null);
		}

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}
			PropertyDescriptor propertyDescriptor = obj as PropertyDescriptor;
			if (propertyDescriptor == null)
			{
				return false;
			}
			return propertyDescriptor.PropertyType == PropertyType;
		}

		public PropertyDescriptorCollection GetChildProperties()
		{
			return GetChildProperties(null, null);
		}

		public PropertyDescriptorCollection GetChildProperties(object instance)
		{
			return GetChildProperties(instance, null);
		}

		public PropertyDescriptorCollection GetChildProperties(Attribute[] filter)
		{
			return GetChildProperties(null, filter);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public virtual PropertyDescriptorCollection GetChildProperties(object instance, Attribute[] filter)
		{
			return TypeDescriptor.GetProperties(instance, filter);
		}

		public virtual object GetEditor(Type editorBaseType)
		{
			Type type = null;
			Attribute[] attributeArray = AttributeArray;
			if (attributeArray != null && attributeArray.Length != 0)
			{
				Attribute[] array = attributeArray;
				foreach (Attribute attribute in array)
				{
					EditorAttribute editorAttribute = attribute as EditorAttribute;
					if (editorAttribute != null)
					{
						type = GetTypeFromName(editorAttribute.EditorTypeName);
						if (type != null && type.IsSubclassOf(editorBaseType))
						{
							break;
						}
					}
				}
			}
			object obj = null;
			if (type != null)
			{
				obj = CreateInstance(type);
			}
			if (obj == null)
			{
				obj = TypeDescriptor.GetEditor(PropertyType, editorBaseType);
			}
			return obj;
		}

		protected Type GetTypeFromName(string typeName)
		{
			if (typeName == null || ComponentType == null || typeName.Trim().Length == 0)
			{
				return null;
			}
			Type type = Type.GetType(typeName);
			if (type == null)
			{
				int num = typeName.IndexOf(",");
				if (num != -1)
				{
					typeName = typeName.Substring(0, num);
				}
				type = ComponentType.Assembly.GetType(typeName);
			}
			return type;
		}
	}
}
