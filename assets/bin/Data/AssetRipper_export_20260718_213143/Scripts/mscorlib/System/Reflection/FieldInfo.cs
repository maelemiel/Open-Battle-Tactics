using System.Diagnostics;
using System.Globalization;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Serializable]
	[ComDefaultInterface(typeof(_FieldInfo))]
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
	public abstract class FieldInfo : MemberInfo, _FieldInfo
	{
		public abstract FieldAttributes Attributes { get; }

		public abstract RuntimeFieldHandle FieldHandle { get; }

		public abstract Type FieldType { get; }

		public override MemberTypes MemberType
		{
			get
			{
				return MemberTypes.Field;
			}
		}

		public bool IsLiteral
		{
			get
			{
				return (Attributes & FieldAttributes.Literal) != 0;
			}
		}

		public bool IsStatic
		{
			get
			{
				return (Attributes & FieldAttributes.Static) != 0;
			}
		}

		public bool IsInitOnly
		{
			get
			{
				return (Attributes & FieldAttributes.InitOnly) != 0;
			}
		}

		public bool IsPublic
		{
			get
			{
				return (Attributes & FieldAttributes.FieldAccessMask) == FieldAttributes.Public;
			}
		}

		public bool IsPrivate
		{
			get
			{
				return (Attributes & FieldAttributes.FieldAccessMask) == FieldAttributes.Private;
			}
		}

		public bool IsFamily
		{
			get
			{
				return (Attributes & FieldAttributes.FieldAccessMask) == FieldAttributes.Family;
			}
		}

		public bool IsAssembly
		{
			get
			{
				return (Attributes & FieldAttributes.FieldAccessMask) == FieldAttributes.Assembly;
			}
		}

		public bool IsFamilyAndAssembly
		{
			get
			{
				return (Attributes & FieldAttributes.FieldAccessMask) == FieldAttributes.FamANDAssem;
			}
		}

		public bool IsFamilyOrAssembly
		{
			get
			{
				return (Attributes & FieldAttributes.FieldAccessMask) == FieldAttributes.FamORAssem;
			}
		}

		public bool IsPinvokeImpl
		{
			get
			{
				return (Attributes & FieldAttributes.PinvokeImpl) == FieldAttributes.PinvokeImpl;
			}
		}

		public bool IsSpecialName
		{
			get
			{
				return (Attributes & FieldAttributes.SpecialName) == FieldAttributes.SpecialName;
			}
		}

		public bool IsNotSerialized
		{
			get
			{
				return (Attributes & FieldAttributes.NotSerialized) == FieldAttributes.NotSerialized;
			}
		}

		internal virtual UnmanagedMarshal UMarshal
		{
			get
			{
				return GetUnmanagedMarshal();
			}
		}

		void _FieldInfo.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _FieldInfo.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _FieldInfo.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _FieldInfo.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		public abstract object GetValue(object obj);

		public abstract void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, CultureInfo culture);

		[DebuggerHidden]
		[DebuggerStepThrough]
		public void SetValue(object obj, object value)
		{
			SetValue(obj, value, BindingFlags.Default, null, null);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern FieldInfo internal_from_handle_type(IntPtr field_handle, IntPtr type_handle);

		public static FieldInfo GetFieldFromHandle(RuntimeFieldHandle handle)
		{
			if (handle.Value == IntPtr.Zero)
			{
				throw new ArgumentException("The handle is invalid.");
			}
			return internal_from_handle_type(handle.Value, IntPtr.Zero);
		}

		[ComVisible(false)]
		public static FieldInfo GetFieldFromHandle(RuntimeFieldHandle handle, RuntimeTypeHandle declaringType)
		{
			if (handle.Value == IntPtr.Zero)
			{
				throw new ArgumentException("The handle is invalid.");
			}
			FieldInfo fieldInfo = internal_from_handle_type(handle.Value, declaringType.Value);
			if (fieldInfo == null)
			{
				throw new ArgumentException("The field handle and the type handle are incompatible.");
			}
			return fieldInfo;
		}

		internal virtual int GetFieldOffset()
		{
			throw new SystemException("This method should not be called");
		}

		[CLSCompliant(false)]
		[MonoTODO("Not implemented")]
		public virtual object GetValueDirect(TypedReference obj)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Not implemented")]
		[CLSCompliant(false)]
		public virtual void SetValueDirect(TypedReference obj, object value)
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern UnmanagedMarshal GetUnmanagedMarshal();

		internal object[] GetPseudoCustomAttributes()
		{
			int num = 0;
			if (IsNotSerialized)
			{
				num++;
			}
			if (DeclaringType.IsExplicitLayout)
			{
				num++;
			}
			UnmanagedMarshal uMarshal = UMarshal;
			if (uMarshal != null)
			{
				num++;
			}
			if (num == 0)
			{
				return null;
			}
			object[] array = new object[num];
			num = 0;
			if (IsNotSerialized)
			{
				array[num++] = new NonSerializedAttribute();
			}
			if (DeclaringType.IsExplicitLayout)
			{
				array[num++] = new FieldOffsetAttribute(GetFieldOffset());
			}
			if (uMarshal != null)
			{
				array[num++] = uMarshal.ToMarshalAsAttribute();
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

		public virtual object GetRawConstantValue()
		{
			throw new NotSupportedException("This non-CLS method is not implemented.");
		}

		virtual Type _FieldInfo.GetType()
		{
			return GetType();
		}
	}
}
