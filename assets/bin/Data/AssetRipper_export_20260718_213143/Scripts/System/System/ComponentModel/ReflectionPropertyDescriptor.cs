using System.Collections;
using System.ComponentModel.Design;
using System.Reflection;

namespace System.ComponentModel
{
	internal class ReflectionPropertyDescriptor : PropertyDescriptor
	{
		private PropertyInfo _member;

		private Type _componentType;

		private Type _propertyType;

		private PropertyInfo getter;

		private PropertyInfo setter;

		private bool accessors_inited;

		public override Type ComponentType
		{
			get
			{
				return _componentType;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				ReadOnlyAttribute readOnlyAttribute = (ReadOnlyAttribute)Attributes[typeof(ReadOnlyAttribute)];
				return !GetPropertyInfo().CanWrite || readOnlyAttribute.IsReadOnly;
			}
		}

		public override Type PropertyType
		{
			get
			{
				return _propertyType;
			}
		}

		public ReflectionPropertyDescriptor(Type componentType, PropertyDescriptor oldPropertyDescriptor, Attribute[] attributes)
			: base(oldPropertyDescriptor, attributes)
		{
			_componentType = componentType;
			_propertyType = oldPropertyDescriptor.PropertyType;
		}

		public ReflectionPropertyDescriptor(Type componentType, string name, Type type, Attribute[] attributes)
			: base(name, attributes)
		{
			_componentType = componentType;
			_propertyType = type;
		}

		public ReflectionPropertyDescriptor(PropertyInfo info)
			: base(info.Name, null)
		{
			_member = info;
			_componentType = _member.DeclaringType;
			_propertyType = info.PropertyType;
		}

		private PropertyInfo GetPropertyInfo()
		{
			if (_member == null)
			{
				_member = _componentType.GetProperty(Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty, null, PropertyType, new Type[0], new ParameterModifier[0]);
				if (_member == null)
				{
					throw new ArgumentException("Accessor methods for the " + Name + " property are missing");
				}
			}
			return _member;
		}

		protected override void FillAttributes(IList attributeList)
		{
			base.FillAttributes(attributeList);
			if (!GetPropertyInfo().CanWrite)
			{
				attributeList.Add(ReadOnlyAttribute.Yes);
			}
			int num = 0;
			Type type = ComponentType;
			while (type != null && type != typeof(object))
			{
				num++;
				type = type.BaseType;
			}
			Attribute[][] array = new Attribute[num][];
			type = ComponentType;
			while (type != null && type != typeof(object))
			{
				PropertyInfo property = type.GetProperty(Name, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, PropertyType, new Type[0], new ParameterModifier[0]);
				if (property != null)
				{
					object[] customAttributes = property.GetCustomAttributes(false);
					Attribute[] array2 = new Attribute[customAttributes.Length];
					customAttributes.CopyTo(array2, 0);
					array[--num] = array2;
				}
				type = type.BaseType;
			}
			Attribute[][] array3 = array;
			foreach (Attribute[] array4 in array3)
			{
				if (array4 != null)
				{
					Attribute[] array5 = array4;
					foreach (Attribute value in array5)
					{
						attributeList.Add(value);
					}
				}
			}
			foreach (Attribute attribute in TypeDescriptor.GetAttributes(PropertyType))
			{
				attributeList.Add(attribute);
			}
		}

		public override object GetValue(object component)
		{
			component = MemberDescriptor.GetInvokee(_componentType, component);
			InitAccessors();
			return getter.GetValue(component, null);
		}

		private DesignerTransaction CreateTransaction(object obj, string description)
		{
			IComponent component = obj as IComponent;
			if (component == null || component.Site == null)
			{
				return null;
			}
			IDesignerHost designerHost = (IDesignerHost)component.Site.GetService(typeof(IDesignerHost));
			if (designerHost == null)
			{
				return null;
			}
			DesignerTransaction result = designerHost.CreateTransaction(description);
			IComponentChangeService componentChangeService = (IComponentChangeService)component.Site.GetService(typeof(IComponentChangeService));
			if (componentChangeService != null)
			{
				componentChangeService.OnComponentChanging(component, this);
			}
			return result;
		}

		private void EndTransaction(object obj, DesignerTransaction tran, object oldValue, object newValue, bool commit)
		{
			if (tran == null)
			{
				OnValueChanged(obj, new PropertyChangedEventArgs(Name));
			}
			else if (commit)
			{
				IComponent component = obj as IComponent;
				IComponentChangeService componentChangeService = (IComponentChangeService)component.Site.GetService(typeof(IComponentChangeService));
				if (componentChangeService != null)
				{
					componentChangeService.OnComponentChanged(component, this, oldValue, newValue);
				}
				tran.Commit();
				OnValueChanged(obj, new PropertyChangedEventArgs(Name));
			}
			else
			{
				tran.Cancel();
			}
		}

		private void InitAccessors()
		{
			if (accessors_inited)
			{
				return;
			}
			PropertyInfo propertyInfo = GetPropertyInfo();
			MethodInfo methodInfo = propertyInfo.GetSetMethod(true);
			MethodInfo methodInfo2 = propertyInfo.GetGetMethod(true);
			if (methodInfo2 != null)
			{
				getter = propertyInfo;
			}
			if (methodInfo != null)
			{
				setter = propertyInfo;
			}
			if (methodInfo != null && methodInfo2 != null)
			{
				accessors_inited = true;
				return;
			}
			if (methodInfo == null && methodInfo2 == null)
			{
				accessors_inited = true;
				return;
			}
			MethodInfo methodInfo3 = ((methodInfo2 == null) ? methodInfo : methodInfo2);
			if (methodInfo3 == null || !methodInfo3.IsVirtual || (methodInfo3.Attributes & MethodAttributes.VtableLayoutMask) == MethodAttributes.VtableLayoutMask)
			{
				accessors_inited = true;
				return;
			}
			Type baseType = _componentType.BaseType;
			while (baseType != null && baseType != typeof(object))
			{
				propertyInfo = baseType.GetProperty(Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty, null, PropertyType, new Type[0], new ParameterModifier[0]);
				if (propertyInfo == null)
				{
					break;
				}
				if (methodInfo == null)
				{
					methodInfo = (methodInfo3 = propertyInfo.GetSetMethod());
				}
				else
				{
					methodInfo2 = (methodInfo3 = propertyInfo.GetGetMethod());
				}
				if (methodInfo2 != null && getter == null)
				{
					getter = propertyInfo;
				}
				if (methodInfo != null && setter == null)
				{
					setter = propertyInfo;
				}
				if (methodInfo3 != null)
				{
					break;
				}
				baseType = baseType.BaseType;
			}
			accessors_inited = true;
		}

		public override void SetValue(object component, object value)
		{
			DesignerTransaction tran = CreateTransaction(component, "Set Property '" + Name + "'");
			object invokee = MemberDescriptor.GetInvokee(_componentType, component);
			object value2 = GetValue(invokee);
			try
			{
				InitAccessors();
				setter.SetValue(invokee, value, null);
				EndTransaction(component, tran, value2, value, true);
			}
			catch
			{
				EndTransaction(component, tran, value2, value, false);
				throw;
			}
		}

		private MethodInfo FindPropertyMethod(object o, string method_name)
		{
			MethodInfo result = null;
			string text = method_name + Name;
			MethodInfo[] methods = o.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (MethodInfo methodInfo in methods)
			{
				if (methodInfo.Name == text && methodInfo.GetParameters().Length == 0)
				{
					result = methodInfo;
					break;
				}
			}
			return result;
		}

		public override void ResetValue(object component)
		{
			object invokee = MemberDescriptor.GetInvokee(_componentType, component);
			DefaultValueAttribute defaultValueAttribute = (DefaultValueAttribute)Attributes[typeof(DefaultValueAttribute)];
			if (defaultValueAttribute != null)
			{
				SetValue(invokee, defaultValueAttribute.Value);
			}
			DesignerTransaction tran = CreateTransaction(component, "Reset Property '" + Name + "'");
			object value = GetValue(invokee);
			try
			{
				MethodInfo methodInfo = FindPropertyMethod(invokee, "Reset");
				if (methodInfo != null)
				{
					methodInfo.Invoke(invokee, null);
				}
				EndTransaction(component, tran, value, GetValue(invokee), true);
			}
			catch
			{
				EndTransaction(component, tran, value, GetValue(invokee), false);
				throw;
			}
		}

		public override bool CanResetValue(object component)
		{
			component = MemberDescriptor.GetInvokee(_componentType, component);
			DefaultValueAttribute defaultValueAttribute = (DefaultValueAttribute)Attributes[typeof(DefaultValueAttribute)];
			if (defaultValueAttribute != null)
			{
				object value = GetValue(component);
				if (defaultValueAttribute.Value == null || value == null)
				{
					if (defaultValueAttribute.Value != value)
					{
						return true;
					}
					if (defaultValueAttribute.Value == null && value == null)
					{
						return false;
					}
				}
				return !defaultValueAttribute.Value.Equals(value);
			}
			if (!_member.CanWrite)
			{
				return false;
			}
			MethodInfo methodInfo = FindPropertyMethod(component, "ShouldPersist");
			if (methodInfo != null)
			{
				return (bool)methodInfo.Invoke(component, null);
			}
			methodInfo = FindPropertyMethod(component, "ShouldSerialize");
			if (methodInfo != null && !(bool)methodInfo.Invoke(component, null))
			{
				return false;
			}
			methodInfo = FindPropertyMethod(component, "Reset");
			return methodInfo != null;
		}

		public override bool ShouldSerializeValue(object component)
		{
			component = MemberDescriptor.GetInvokee(_componentType, component);
			if (IsReadOnly)
			{
				MethodInfo methodInfo = FindPropertyMethod(component, "ShouldSerialize");
				if (methodInfo != null)
				{
					return (bool)methodInfo.Invoke(component, null);
				}
				return Attributes.Contains(DesignerSerializationVisibilityAttribute.Content);
			}
			DefaultValueAttribute defaultValueAttribute = (DefaultValueAttribute)Attributes[typeof(DefaultValueAttribute)];
			if (defaultValueAttribute != null)
			{
				object value = GetValue(component);
				if (defaultValueAttribute.Value == null || value == null)
				{
					return defaultValueAttribute.Value != value;
				}
				return !defaultValueAttribute.Value.Equals(value);
			}
			MethodInfo methodInfo2 = FindPropertyMethod(component, "ShouldSerialize");
			if (methodInfo2 != null)
			{
				return (bool)methodInfo2.Invoke(component, null);
			}
			return true;
		}
	}
}
