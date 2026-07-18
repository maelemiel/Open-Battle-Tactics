using System.Reflection;
using System.Runtime.InteropServices;

namespace System
{
	[Serializable]
	[ComVisible(true)]
	[ComDefaultInterface(typeof(_Attribute))]
	[AttributeUsage(AttributeTargets.All)]
	[ClassInterface(ClassInterfaceType.None)]
	public abstract class Attribute : _Attribute
	{
		public virtual object TypeId
		{
			get
			{
				return GetType();
			}
		}

		void _Attribute.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _Attribute.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _Attribute.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _Attribute.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		private static void CheckParameters(object element, Type attributeType)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			if (attributeType == null)
			{
				throw new ArgumentNullException("attributeType");
			}
			if (!typeof(Attribute).IsAssignableFrom(attributeType))
			{
				throw new ArgumentException(Locale.GetText("Type is not derived from System.Attribute."), "attributeType");
			}
		}

		private static Attribute FindAttribute(object[] attributes)
		{
			if (attributes.Length > 1)
			{
				throw new AmbiguousMatchException(Locale.GetText("<element> has more than one attribute of type <attribute_type>"));
			}
			if (attributes.Length < 1)
			{
				return null;
			}
			return (Attribute)attributes[0];
		}

		public static Attribute GetCustomAttribute(ParameterInfo element, Type attributeType)
		{
			return GetCustomAttribute(element, attributeType, true);
		}

		public static Attribute GetCustomAttribute(MemberInfo element, Type attributeType)
		{
			return GetCustomAttribute(element, attributeType, true);
		}

		public static Attribute GetCustomAttribute(Assembly element, Type attributeType)
		{
			return GetCustomAttribute(element, attributeType, true);
		}

		public static Attribute GetCustomAttribute(Module element, Type attributeType)
		{
			return GetCustomAttribute(element, attributeType, true);
		}

		public static Attribute GetCustomAttribute(Module element, Type attributeType, bool inherit)
		{
			CheckParameters(element, attributeType);
			object[] customAttributes = element.GetCustomAttributes(attributeType, inherit);
			return FindAttribute(customAttributes);
		}

		public static Attribute GetCustomAttribute(Assembly element, Type attributeType, bool inherit)
		{
			CheckParameters(element, attributeType);
			object[] customAttributes = element.GetCustomAttributes(attributeType, inherit);
			return FindAttribute(customAttributes);
		}

		public static Attribute GetCustomAttribute(ParameterInfo element, Type attributeType, bool inherit)
		{
			CheckParameters(element, attributeType);
			object[] customAttributes = element.GetCustomAttributes(attributeType, inherit);
			return FindAttribute(customAttributes);
		}

		public static Attribute GetCustomAttribute(MemberInfo element, Type attributeType, bool inherit)
		{
			CheckParameters(element, attributeType);
			return MonoCustomAttrs.GetCustomAttribute(element, attributeType, inherit);
		}

		public static Attribute[] GetCustomAttributes(Assembly element)
		{
			return GetCustomAttributes(element, true);
		}

		public static Attribute[] GetCustomAttributes(ParameterInfo element)
		{
			return GetCustomAttributes(element, true);
		}

		public static Attribute[] GetCustomAttributes(MemberInfo element)
		{
			return GetCustomAttributes(element, true);
		}

		public static Attribute[] GetCustomAttributes(Module element)
		{
			return GetCustomAttributes(element, true);
		}

		public static Attribute[] GetCustomAttributes(Assembly element, Type attributeType)
		{
			return GetCustomAttributes(element, attributeType, true);
		}

		public static Attribute[] GetCustomAttributes(Module element, Type attributeType)
		{
			return GetCustomAttributes(element, attributeType, true);
		}

		public static Attribute[] GetCustomAttributes(ParameterInfo element, Type attributeType)
		{
			return GetCustomAttributes(element, attributeType, true);
		}

		public static Attribute[] GetCustomAttributes(MemberInfo element, Type type)
		{
			return GetCustomAttributes(element, type, true);
		}

		public static Attribute[] GetCustomAttributes(Assembly element, Type attributeType, bool inherit)
		{
			CheckParameters(element, attributeType);
			return (Attribute[])element.GetCustomAttributes(attributeType, inherit);
		}

		public static Attribute[] GetCustomAttributes(ParameterInfo element, Type attributeType, bool inherit)
		{
			CheckParameters(element, attributeType);
			return (Attribute[])element.GetCustomAttributes(attributeType, inherit);
		}

		public static Attribute[] GetCustomAttributes(Module element, Type attributeType, bool inherit)
		{
			CheckParameters(element, attributeType);
			return (Attribute[])element.GetCustomAttributes(attributeType, inherit);
		}

		public static Attribute[] GetCustomAttributes(MemberInfo element, Type type, bool inherit)
		{
			CheckParameters(element, type);
			MemberTypes memberType = element.MemberType;
			if (memberType == MemberTypes.Property)
			{
				return (Attribute[])MonoCustomAttrs.GetCustomAttributes(element, type, inherit);
			}
			return (Attribute[])element.GetCustomAttributes(type, inherit);
		}

		public static Attribute[] GetCustomAttributes(Module element, bool inherit)
		{
			CheckParameters(element, typeof(Attribute));
			return (Attribute[])element.GetCustomAttributes(inherit);
		}

		public static Attribute[] GetCustomAttributes(Assembly element, bool inherit)
		{
			CheckParameters(element, typeof(Attribute));
			return (Attribute[])element.GetCustomAttributes(inherit);
		}

		public static Attribute[] GetCustomAttributes(MemberInfo element, bool inherit)
		{
			CheckParameters(element, typeof(Attribute));
			MemberTypes memberType = element.MemberType;
			if (memberType == MemberTypes.Property)
			{
				return (Attribute[])MonoCustomAttrs.GetCustomAttributes(element, inherit);
			}
			return (Attribute[])element.GetCustomAttributes(typeof(Attribute), inherit);
		}

		public static Attribute[] GetCustomAttributes(ParameterInfo element, bool inherit)
		{
			CheckParameters(element, typeof(Attribute));
			return (Attribute[])element.GetCustomAttributes(inherit);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public virtual bool IsDefaultAttribute()
		{
			return false;
		}

		public static bool IsDefined(Module element, Type attributeType)
		{
			return IsDefined(element, attributeType, false);
		}

		public static bool IsDefined(ParameterInfo element, Type attributeType)
		{
			return IsDefined(element, attributeType, true);
		}

		public static bool IsDefined(MemberInfo element, Type attributeType)
		{
			return IsDefined(element, attributeType, true);
		}

		public static bool IsDefined(Assembly element, Type attributeType)
		{
			return IsDefined(element, attributeType, true);
		}

		public static bool IsDefined(MemberInfo element, Type attributeType, bool inherit)
		{
			CheckParameters(element, attributeType);
			MemberTypes memberType = element.MemberType;
			if (memberType != MemberTypes.Constructor && memberType != MemberTypes.Event && memberType != MemberTypes.Field && memberType != MemberTypes.Method && memberType != MemberTypes.Property && memberType != MemberTypes.TypeInfo && memberType != MemberTypes.NestedType)
			{
				throw new NotSupportedException(Locale.GetText("Element is not a constructor, method, property, event, type or field."));
			}
			if (memberType == MemberTypes.Property)
			{
				return MonoCustomAttrs.IsDefined(element, attributeType, inherit);
			}
			return element.IsDefined(attributeType, inherit);
		}

		public static bool IsDefined(Assembly element, Type attributeType, bool inherit)
		{
			CheckParameters(element, attributeType);
			return element.IsDefined(attributeType, inherit);
		}

		public static bool IsDefined(Module element, Type attributeType, bool inherit)
		{
			CheckParameters(element, attributeType);
			return element.IsDefined(attributeType, inherit);
		}

		public static bool IsDefined(ParameterInfo element, Type attributeType, bool inherit)
		{
			CheckParameters(element, attributeType);
			if (element.IsDefined(attributeType, inherit))
			{
				return true;
			}
			return IsDefined(element.Member, attributeType, inherit);
		}

		public virtual bool Match(object obj)
		{
			return Equals(obj);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is Attribute))
			{
				return false;
			}
			return ValueType.DefaultEquals(this, obj);
		}
	}
}
