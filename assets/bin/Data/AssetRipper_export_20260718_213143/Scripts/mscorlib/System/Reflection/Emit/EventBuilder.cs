using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
	[ComDefaultInterface(typeof(_EventBuilder))]
	public sealed class EventBuilder : _EventBuilder
	{
		internal string name;

		private Type type;

		private TypeBuilder typeb;

		private CustomAttributeBuilder[] cattrs;

		internal MethodBuilder add_method;

		internal MethodBuilder remove_method;

		internal MethodBuilder raise_method;

		internal MethodBuilder[] other_methods;

		internal EventAttributes attrs;

		private int table_idx;

		internal EventBuilder(TypeBuilder tb, string eventName, EventAttributes eventAttrs, Type eventType)
		{
			name = eventName;
			attrs = eventAttrs;
			type = eventType;
			typeb = tb;
			table_idx = get_next_table_index(this, 20, true);
		}

		void _EventBuilder.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _EventBuilder.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _EventBuilder.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _EventBuilder.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		internal int get_next_table_index(object obj, int table, bool inc)
		{
			return typeb.get_next_table_index(obj, table, inc);
		}

		public void AddOtherMethod(MethodBuilder mdBuilder)
		{
			if (mdBuilder == null)
			{
				throw new ArgumentNullException("mdBuilder");
			}
			RejectIfCreated();
			if (other_methods != null)
			{
				MethodBuilder[] array = new MethodBuilder[other_methods.Length + 1];
				other_methods.CopyTo(array, 0);
				other_methods = array;
			}
			else
			{
				other_methods = new MethodBuilder[1];
			}
			other_methods[other_methods.Length - 1] = mdBuilder;
		}

		public EventToken GetEventToken()
		{
			return new EventToken(0x14000000 | table_idx);
		}

		public void SetAddOnMethod(MethodBuilder mdBuilder)
		{
			if (mdBuilder == null)
			{
				throw new ArgumentNullException("mdBuilder");
			}
			RejectIfCreated();
			add_method = mdBuilder;
		}

		public void SetRaiseMethod(MethodBuilder mdBuilder)
		{
			if (mdBuilder == null)
			{
				throw new ArgumentNullException("mdBuilder");
			}
			RejectIfCreated();
			raise_method = mdBuilder;
		}

		public void SetRemoveOnMethod(MethodBuilder mdBuilder)
		{
			if (mdBuilder == null)
			{
				throw new ArgumentNullException("mdBuilder");
			}
			RejectIfCreated();
			remove_method = mdBuilder;
		}

		public void SetCustomAttribute(CustomAttributeBuilder customBuilder)
		{
			if (customBuilder == null)
			{
				throw new ArgumentNullException("customBuilder");
			}
			RejectIfCreated();
			string fullName = customBuilder.Ctor.ReflectedType.FullName;
			if (fullName == "System.Runtime.CompilerServices.SpecialNameAttribute")
			{
				attrs |= EventAttributes.SpecialName;
			}
			else if (cattrs != null)
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
			if (con == null)
			{
				throw new ArgumentNullException("con");
			}
			if (binaryAttribute == null)
			{
				throw new ArgumentNullException("binaryAttribute");
			}
			SetCustomAttribute(new CustomAttributeBuilder(con, binaryAttribute));
		}

		private void RejectIfCreated()
		{
			if (typeb.is_created)
			{
				throw new InvalidOperationException("Type definition of the method is complete.");
			}
		}
	}
}
