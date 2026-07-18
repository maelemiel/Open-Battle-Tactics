using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System.Reflection
{
	[Serializable]
	internal class MonoField : FieldInfo, ISerializable
	{
		internal IntPtr klass;

		internal RuntimeFieldHandle fhandle;

		private string name;

		private Type type;

		private FieldAttributes attrs;

		public override FieldAttributes Attributes
		{
			get
			{
				return attrs;
			}
		}

		public override RuntimeFieldHandle FieldHandle
		{
			get
			{
				return fhandle;
			}
		}

		public override Type FieldType
		{
			get
			{
				return type;
			}
		}

		public override Type ReflectedType
		{
			get
			{
				return GetParentType(false);
			}
		}

		public override Type DeclaringType
		{
			get
			{
				return GetParentType(true);
			}
		}

		public override string Name
		{
			get
			{
				return name;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Type GetParentType(bool declaring);

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			return MonoCustomAttrs.IsDefined(this, attributeType, inherit);
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			return MonoCustomAttrs.GetCustomAttributes(this, inherit);
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return MonoCustomAttrs.GetCustomAttributes(this, attributeType, inherit);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal override extern int GetFieldOffset();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern object GetValueInternal(object obj);

		public override object GetValue(object obj)
		{
			if (!IsStatic && obj == null)
			{
				throw new TargetException("Non-static field requires a target");
			}
			if (!IsLiteral)
			{
				CheckGeneric();
			}
			return GetValueInternal(obj);
		}

		public override string ToString()
		{
			return string.Format("{0} {1}", type, name);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetValueInternal(FieldInfo fi, object obj, object value);

		public override void SetValue(object obj, object val, BindingFlags invokeAttr, Binder binder, CultureInfo culture)
		{
			if (!IsStatic && obj == null)
			{
				throw new TargetException("Non-static field requires a target");
			}
			if (IsLiteral)
			{
				throw new FieldAccessException("Cannot set a constant field");
			}
			if (binder == null)
			{
				binder = Binder.DefaultBinder;
			}
			CheckGeneric();
			if (val != null)
			{
				object obj2 = binder.ChangeType(val, type, culture);
				if (obj2 == null)
				{
					throw new ArgumentException(string.Concat("Object type ", val.GetType(), " cannot be converted to target type: ", type), "val");
				}
				val = obj2;
			}
			SetValueInternal(this, obj, val);
		}

		internal MonoField Clone(string newName)
		{
			MonoField monoField = new MonoField();
			monoField.name = newName;
			monoField.type = type;
			monoField.attrs = attrs;
			monoField.klass = klass;
			monoField.fhandle = fhandle;
			return monoField;
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			MemberInfoSerializationHolder.Serialize(info, Name, ReflectedType, ToString(), MemberTypes.Field);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public override extern object GetRawConstantValue();

		private void CheckGeneric()
		{
			if (DeclaringType.ContainsGenericParameters)
			{
				throw new InvalidOperationException("Late bound operations cannot be performed on fields with types for which Type.ContainsGenericParameters is true.");
			}
		}
	}
}
