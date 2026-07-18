namespace System.ComponentModel
{
	public abstract class CustomTypeDescriptor : ICustomTypeDescriptor
	{
		private ICustomTypeDescriptor _parent;

		protected CustomTypeDescriptor()
		{
		}

		protected CustomTypeDescriptor(ICustomTypeDescriptor parent)
		{
			_parent = parent;
		}

		public virtual AttributeCollection GetAttributes()
		{
			if (_parent != null)
			{
				return _parent.GetAttributes();
			}
			return AttributeCollection.Empty;
		}

		public virtual string GetClassName()
		{
			if (_parent != null)
			{
				return _parent.GetClassName();
			}
			return null;
		}

		public virtual string GetComponentName()
		{
			if (_parent != null)
			{
				return _parent.GetComponentName();
			}
			return null;
		}

		public virtual TypeConverter GetConverter()
		{
			if (_parent != null)
			{
				return _parent.GetConverter();
			}
			return new TypeConverter();
		}

		public virtual EventDescriptor GetDefaultEvent()
		{
			if (_parent != null)
			{
				return _parent.GetDefaultEvent();
			}
			return null;
		}

		public virtual PropertyDescriptor GetDefaultProperty()
		{
			if (_parent != null)
			{
				return _parent.GetDefaultProperty();
			}
			return null;
		}

		public virtual object GetEditor(Type editorBaseType)
		{
			if (_parent != null)
			{
				return _parent.GetEditor(editorBaseType);
			}
			return null;
		}

		public virtual EventDescriptorCollection GetEvents()
		{
			if (_parent != null)
			{
				return _parent.GetEvents();
			}
			return EventDescriptorCollection.Empty;
		}

		public virtual EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			if (_parent != null)
			{
				return _parent.GetEvents(attributes);
			}
			return EventDescriptorCollection.Empty;
		}

		public virtual PropertyDescriptorCollection GetProperties()
		{
			if (_parent != null)
			{
				return _parent.GetProperties();
			}
			return PropertyDescriptorCollection.Empty;
		}

		public virtual PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			if (_parent != null)
			{
				return _parent.GetProperties(attributes);
			}
			return PropertyDescriptorCollection.Empty;
		}

		public virtual object GetPropertyOwner(PropertyDescriptor pd)
		{
			if (_parent != null)
			{
				return _parent.GetPropertyOwner(pd);
			}
			return null;
		}
	}
}
