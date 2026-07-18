using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[ClassInterface(ClassInterfaceType.None)]
	[ComDefaultInterface(typeof(_FieldBuilder))]
	[ComVisible(true)]
	public sealed class FieldBuilder : FieldInfo, _FieldBuilder
	{
		private FieldAttributes attrs;

		private Type type;

		private string name;

		private object def_value;

		private int offset;

		private int table_idx;

		internal TypeBuilder typeb;

		private byte[] rva_data;

		private CustomAttributeBuilder[] cattrs;

		private UnmanagedMarshal marshal_info;

		private RuntimeFieldHandle handle;

		private Type[] modReq;

		private Type[] modOpt;

		public override FieldAttributes Attributes
		{
			get
			{
				return attrs;
			}
		}

		public override Type DeclaringType
		{
			get
			{
				return typeb;
			}
		}

		public override RuntimeFieldHandle FieldHandle
		{
			get
			{
				throw CreateNotSupportedException();
			}
		}

		public override Type FieldType
		{
			get
			{
				return type;
			}
		}

		public override string Name
		{
			get
			{
				return name;
			}
		}

		public override Type ReflectedType
		{
			get
			{
				return typeb;
			}
		}

		internal override UnmanagedMarshal UMarshal
		{
			get
			{
				return marshal_info;
			}
		}

		public override Module Module
		{
			get
			{
				return base.Module;
			}
		}

		internal FieldBuilder(TypeBuilder tb, string fieldName, Type type, FieldAttributes attributes, Type[] modReq, Type[] modOpt)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			attrs = attributes;
			name = fieldName;
			this.type = type;
			this.modReq = modReq;
			this.modOpt = modOpt;
			offset = -1;
			typeb = tb;
			table_idx = tb.get_next_table_index(this, 4, true);
		}

		void _FieldBuilder.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _FieldBuilder.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _FieldBuilder.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _FieldBuilder.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			if (typeb.is_created)
			{
				return MonoCustomAttrs.GetCustomAttributes(this, inherit);
			}
			throw CreateNotSupportedException();
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			if (typeb.is_created)
			{
				return MonoCustomAttrs.GetCustomAttributes(this, attributeType, inherit);
			}
			throw CreateNotSupportedException();
		}

		public FieldToken GetToken()
		{
			return new FieldToken(MetadataToken);
		}

		public override object GetValue(object obj)
		{
			throw CreateNotSupportedException();
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw CreateNotSupportedException();
		}

		internal override int GetFieldOffset()
		{
			return 0;
		}

		internal void SetRVAData(byte[] data)
		{
			rva_data = (byte[])data.Clone();
		}

		public void SetConstant(object defaultValue)
		{
			RejectIfCreated();
			def_value = defaultValue;
		}

		public void SetCustomAttribute(CustomAttributeBuilder customBuilder)
		{
			RejectIfCreated();
			switch (customBuilder.Ctor.ReflectedType.FullName)
			{
			case "System.Runtime.InteropServices.FieldOffsetAttribute":
			{
				byte[] data = customBuilder.Data;
				offset = data[2];
				offset |= data[3] << 8;
				offset |= data[4] << 16;
				offset |= data[5] << 24;
				return;
			}
			case "System.NonSerializedAttribute":
				attrs |= FieldAttributes.NotSerialized;
				return;
			case "System.Runtime.CompilerServices.SpecialNameAttribute":
				attrs |= FieldAttributes.SpecialName;
				return;
			case "System.Runtime.InteropServices.MarshalAsAttribute":
				attrs |= FieldAttributes.HasFieldMarshal;
				marshal_info = CustomAttributeBuilder.get_umarshal(customBuilder, true);
				return;
			}
			if (cattrs != null)
			{
				CustomAttributeBuilder[] array = new CustomAttributeBuilder[cattrs.Length + 1];
				cattrs.CopyTo(array, 0);
				array[cattrs.Length] = customBuilder;
				cattrs = array;
			}
			else
			{
				cattrs = new CustomAttributeBuilder[1];
				cattrs[0] = customBuilder;
			}
		}

		[ComVisible(true)]
		public void SetCustomAttribute(ConstructorInfo con, byte[] binaryAttribute)
		{
			RejectIfCreated();
			SetCustomAttribute(new CustomAttributeBuilder(con, binaryAttribute));
		}

		[Obsolete("An alternate API is available: Emit the MarshalAs custom attribute instead.")]
		public void SetMarshal(UnmanagedMarshal unmanagedMarshal)
		{
			RejectIfCreated();
			marshal_info = unmanagedMarshal;
			attrs |= FieldAttributes.HasFieldMarshal;
		}

		public void SetOffset(int iOffset)
		{
			RejectIfCreated();
			offset = iOffset;
		}

		public override void SetValue(object obj, object val, BindingFlags invokeAttr, Binder binder, CultureInfo culture)
		{
			throw CreateNotSupportedException();
		}

		private Exception CreateNotSupportedException()
		{
			return new NotSupportedException("The invoked member is not supported in a dynamic module.");
		}

		private void RejectIfCreated()
		{
			if (typeb.is_created)
			{
				throw new InvalidOperationException("Unable to change after type has been created.");
			}
		}
	}
}
