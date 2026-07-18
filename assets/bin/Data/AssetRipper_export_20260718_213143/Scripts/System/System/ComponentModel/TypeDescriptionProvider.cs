using System.Collections;

namespace System.ComponentModel
{
	public abstract class TypeDescriptionProvider
	{
		private sealed class EmptyCustomTypeDescriptor : CustomTypeDescriptor
		{
		}

		private EmptyCustomTypeDescriptor _emptyCustomTypeDescriptor;

		private TypeDescriptionProvider _parent;

		protected TypeDescriptionProvider()
		{
		}

		protected TypeDescriptionProvider(TypeDescriptionProvider parent)
		{
			_parent = parent;
		}

		public virtual object CreateInstance(IServiceProvider provider, Type objectType, Type[] argTypes, object[] args)
		{
			if (_parent != null)
			{
				return _parent.CreateInstance(provider, objectType, argTypes, args);
			}
			return Activator.CreateInstance(objectType, args);
		}

		public virtual IDictionary GetCache(object instance)
		{
			if (_parent != null)
			{
				return _parent.GetCache(instance);
			}
			return null;
		}

		public virtual ICustomTypeDescriptor GetExtendedTypeDescriptor(object instance)
		{
			if (_parent != null)
			{
				return _parent.GetExtendedTypeDescriptor(instance);
			}
			if (_emptyCustomTypeDescriptor == null)
			{
				_emptyCustomTypeDescriptor = new EmptyCustomTypeDescriptor();
			}
			return _emptyCustomTypeDescriptor;
		}

		public virtual string GetFullComponentName(object component)
		{
			if (_parent != null)
			{
				return _parent.GetFullComponentName(component);
			}
			return GetTypeDescriptor(component).GetComponentName();
		}

		public Type GetReflectionType(object instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			return GetReflectionType(instance.GetType(), instance);
		}

		public Type GetReflectionType(Type objectType)
		{
			return GetReflectionType(objectType, null);
		}

		public virtual Type GetReflectionType(Type objectType, object instance)
		{
			if (_parent != null)
			{
				return _parent.GetReflectionType(objectType, instance);
			}
			return objectType;
		}

		public ICustomTypeDescriptor GetTypeDescriptor(object instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			return GetTypeDescriptor(instance.GetType(), instance);
		}

		public ICustomTypeDescriptor GetTypeDescriptor(Type objectType)
		{
			return GetTypeDescriptor(objectType, null);
		}

		public virtual ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
		{
			if (_parent != null)
			{
				return _parent.GetTypeDescriptor(objectType, instance);
			}
			if (_emptyCustomTypeDescriptor == null)
			{
				_emptyCustomTypeDescriptor = new EmptyCustomTypeDescriptor();
			}
			return _emptyCustomTypeDescriptor;
		}
	}
}
