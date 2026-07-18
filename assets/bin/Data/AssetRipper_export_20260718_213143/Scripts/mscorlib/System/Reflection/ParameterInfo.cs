using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Serializable]
	[ClassInterface(ClassInterfaceType.None)]
	[ComVisible(true)]
	[ComDefaultInterface(typeof(_ParameterInfo))]
	public class ParameterInfo : ICustomAttributeProvider, _ParameterInfo
	{
		protected Type ClassImpl;

		protected object DefaultValueImpl;

		protected MemberInfo MemberImpl;

		protected string NameImpl;

		protected int PositionImpl;

		protected ParameterAttributes AttrsImpl;

		private UnmanagedMarshal marshalAs;

		public virtual Type ParameterType
		{
			get
			{
				return ClassImpl;
			}
		}

		public virtual ParameterAttributes Attributes
		{
			get
			{
				return AttrsImpl;
			}
		}

		public virtual object DefaultValue
		{
			get
			{
				if (ClassImpl == typeof(decimal))
				{
					DecimalConstantAttribute[] array = (DecimalConstantAttribute[])GetCustomAttributes(typeof(DecimalConstantAttribute), false);
					if (array.Length > 0)
					{
						return array[0].Value;
					}
				}
				else if (ClassImpl == typeof(DateTime))
				{
					DateTimeConstantAttribute[] array2 = (DateTimeConstantAttribute[])GetCustomAttributes(typeof(DateTimeConstantAttribute), false);
					if (array2.Length > 0)
					{
						return new DateTime(array2[0].Ticks);
					}
				}
				return DefaultValueImpl;
			}
		}

		public bool IsIn
		{
			get
			{
				return (Attributes & ParameterAttributes.In) != 0;
			}
		}

		public bool IsLcid
		{
			get
			{
				return (Attributes & ParameterAttributes.Lcid) != 0;
			}
		}

		public bool IsOptional
		{
			get
			{
				return (Attributes & ParameterAttributes.Optional) != 0;
			}
		}

		public bool IsOut
		{
			get
			{
				return (Attributes & ParameterAttributes.Out) != 0;
			}
		}

		public bool IsRetval
		{
			get
			{
				return (Attributes & ParameterAttributes.Retval) != 0;
			}
		}

		public virtual MemberInfo Member
		{
			get
			{
				return MemberImpl;
			}
		}

		public virtual string Name
		{
			get
			{
				return NameImpl;
			}
		}

		public virtual int Position
		{
			get
			{
				return PositionImpl;
			}
		}

		public int MetadataToken
		{
			get
			{
				if (MemberImpl is PropertyInfo)
				{
					PropertyInfo propertyInfo = (PropertyInfo)MemberImpl;
					MethodInfo methodInfo = propertyInfo.GetGetMethod(true);
					if (methodInfo == null)
					{
						methodInfo = propertyInfo.GetSetMethod(true);
					}
					return methodInfo.GetParameters()[PositionImpl].MetadataToken;
				}
				if (MemberImpl is MethodBase)
				{
					return GetMetadataToken();
				}
				throw new ArgumentException("Can't produce MetadataToken for member of type " + MemberImpl.GetType());
			}
		}

		public virtual object RawDefaultValue
		{
			get
			{
				return DefaultValue;
			}
		}

		protected ParameterInfo()
		{
		}

		internal ParameterInfo(ParameterBuilder pb, Type type, MemberInfo member, int position)
		{
			ClassImpl = type;
			MemberImpl = member;
			if (pb != null)
			{
				NameImpl = pb.Name;
				PositionImpl = pb.Position - 1;
				AttrsImpl = (ParameterAttributes)pb.Attributes;
			}
			else
			{
				NameImpl = null;
				PositionImpl = position - 1;
				AttrsImpl = ParameterAttributes.None;
			}
		}

		internal ParameterInfo(ParameterInfo pinfo, MemberInfo member)
		{
			ClassImpl = pinfo.ParameterType;
			MemberImpl = member;
			NameImpl = pinfo.Name;
			PositionImpl = pinfo.Position;
			AttrsImpl = pinfo.Attributes;
		}

		internal ParameterInfo(Type type, MemberInfo member, UnmanagedMarshal marshalAs)
		{
			ClassImpl = type;
			MemberImpl = member;
			NameImpl = string.Empty;
			PositionImpl = -1;
			AttrsImpl = ParameterAttributes.Retval;
			this.marshalAs = marshalAs;
		}

		void _ParameterInfo.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _ParameterInfo.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _ParameterInfo.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _ParameterInfo.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			Type type = ClassImpl;
			while (type.HasElementType)
			{
				type = type.GetElementType();
			}
			string text = ((!type.IsPrimitive && ClassImpl != typeof(void) && !(ClassImpl.Namespace == MemberImpl.DeclaringType.Namespace)) ? ClassImpl.FullName : ClassImpl.Name);
			if (!IsRetval)
			{
				text += ' ';
				text += NameImpl;
			}
			return text;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int GetMetadataToken();

		public virtual object[] GetCustomAttributes(bool inherit)
		{
			return MonoCustomAttrs.GetCustomAttributes(this, inherit);
		}

		public virtual object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return MonoCustomAttrs.GetCustomAttributes(this, attributeType, inherit);
		}

		public virtual bool IsDefined(Type attributeType, bool inherit)
		{
			return MonoCustomAttrs.IsDefined(this, attributeType, inherit);
		}

		internal object[] GetPseudoCustomAttributes()
		{
			int num = 0;
			if (IsIn)
			{
				num++;
			}
			if (IsOut)
			{
				num++;
			}
			if (IsOptional)
			{
				num++;
			}
			if (marshalAs != null)
			{
				num++;
			}
			if (num == 0)
			{
				return null;
			}
			object[] array = new object[num];
			num = 0;
			if (IsIn)
			{
				array[num++] = new InAttribute();
			}
			if (IsOptional)
			{
				array[num++] = new OptionalAttribute();
			}
			if (IsOut)
			{
				array[num++] = new OutAttribute();
			}
			if (marshalAs != null)
			{
				array[num++] = marshalAs.ToMarshalAsAttribute();
			}
			return array;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Type[] GetTypeModifiers(bool optional);

		public virtual Type[] GetOptionalCustomModifiers()
		{
			Type[] typeModifiers = GetTypeModifiers(true);
			if (typeModifiers == null)
			{
				return Type.EmptyTypes;
			}
			return typeModifiers;
		}

		public virtual Type[] GetRequiredCustomModifiers()
		{
			Type[] typeModifiers = GetTypeModifiers(false);
			if (typeModifiers == null)
			{
				return Type.EmptyTypes;
			}
			return typeModifiers;
		}
	}
}
