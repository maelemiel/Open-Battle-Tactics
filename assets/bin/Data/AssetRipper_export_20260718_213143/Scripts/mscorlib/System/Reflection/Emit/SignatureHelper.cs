using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[ClassInterface(ClassInterfaceType.None)]
	[ComDefaultInterface(typeof(_SignatureHelper))]
	[ComVisible(true)]
	public sealed class SignatureHelper : _SignatureHelper
	{
		internal enum SignatureHelperType
		{
			HELPER_FIELD = 0,
			HELPER_LOCAL = 1,
			HELPER_METHOD = 2,
			HELPER_PROPERTY = 3
		}

		private ModuleBuilder module;

		private Type[] arguments;

		private SignatureHelperType type;

		private Type returnType;

		private CallingConventions callConv;

		private CallingConvention unmanagedCallConv;

		private Type[][] modreqs;

		private Type[][] modopts;

		internal SignatureHelper(ModuleBuilder module, SignatureHelperType type)
		{
			this.type = type;
			this.module = module;
		}

		void _SignatureHelper.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _SignatureHelper.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _SignatureHelper.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _SignatureHelper.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		public static SignatureHelper GetFieldSigHelper(Module mod)
		{
			if (mod != null && !(mod is ModuleBuilder))
			{
				throw new ArgumentException("ModuleBuilder is expected");
			}
			return new SignatureHelper((ModuleBuilder)mod, SignatureHelperType.HELPER_FIELD);
		}

		public static SignatureHelper GetLocalVarSigHelper(Module mod)
		{
			if (mod != null && !(mod is ModuleBuilder))
			{
				throw new ArgumentException("ModuleBuilder is expected");
			}
			return new SignatureHelper((ModuleBuilder)mod, SignatureHelperType.HELPER_LOCAL);
		}

		public static SignatureHelper GetLocalVarSigHelper()
		{
			return new SignatureHelper(null, SignatureHelperType.HELPER_LOCAL);
		}

		public static SignatureHelper GetMethodSigHelper(CallingConventions callingConvention, Type returnType)
		{
			return GetMethodSigHelper(null, callingConvention, (CallingConvention)0, returnType, null);
		}

		public static SignatureHelper GetMethodSigHelper(CallingConvention unmanagedCallingConvention, Type returnType)
		{
			return GetMethodSigHelper(null, CallingConventions.Standard, unmanagedCallingConvention, returnType, null);
		}

		public static SignatureHelper GetMethodSigHelper(Module mod, CallingConventions callingConvention, Type returnType)
		{
			return GetMethodSigHelper(mod, callingConvention, (CallingConvention)0, returnType, null);
		}

		public static SignatureHelper GetMethodSigHelper(Module mod, CallingConvention unmanagedCallConv, Type returnType)
		{
			return GetMethodSigHelper(mod, CallingConventions.Standard, unmanagedCallConv, returnType, null);
		}

		public static SignatureHelper GetMethodSigHelper(Module mod, Type returnType, Type[] parameterTypes)
		{
			return GetMethodSigHelper(mod, CallingConventions.Standard, (CallingConvention)0, returnType, parameterTypes);
		}

		[MonoTODO("Not implemented")]
		public static SignatureHelper GetPropertySigHelper(Module mod, Type returnType, Type[] parameterTypes)
		{
			throw new NotImplementedException();
		}

		private static int AppendArray(ref Type[] array, Type t)
		{
			if (array != null)
			{
				Type[] array2 = new Type[array.Length + 1];
				Array.Copy(array, array2, array.Length);
				array2[array.Length] = t;
				array = array2;
				return array.Length - 1;
			}
			array = new Type[1];
			array[0] = t;
			return 0;
		}

		private static void AppendArrayAt(ref Type[][] array, Type[] t, int pos)
		{
			int num = Math.Max(pos, (array != null) ? array.Length : 0);
			Type[][] array2 = new Type[num + 1][];
			if (array != null)
			{
				Array.Copy(array, array2, num);
			}
			array2[pos] = t;
			array = array2;
		}

		private static void ValidateParameterModifiers(string name, Type[] parameter_modifiers)
		{
			foreach (Type type in parameter_modifiers)
			{
				if (type == null)
				{
					throw new ArgumentNullException(name);
				}
				if (type.IsArray)
				{
					throw new ArgumentException(Locale.GetText("Array type not permitted"), name);
				}
				if (type.ContainsGenericParameters)
				{
					throw new ArgumentException(Locale.GetText("Open Generic Type not permitted"), name);
				}
			}
		}

		private static void ValidateCustomModifier(int n, Type[][] custom_modifiers, string name)
		{
			if (custom_modifiers == null)
			{
				return;
			}
			if (custom_modifiers.Length != n)
			{
				throw new ArgumentException(Locale.GetText(string.Format("Custom modifiers length `{0}' does not match the size of the arguments")));
			}
			foreach (Type[] array in custom_modifiers)
			{
				if (array != null)
				{
					ValidateParameterModifiers(name, array);
				}
			}
		}

		private static Exception MissingFeature()
		{
			throw new NotImplementedException("Mono does not currently support setting modOpt/modReq through SignatureHelper");
		}

		[MonoTODO("Currently we ignore requiredCustomModifiers and optionalCustomModifiers")]
		public void AddArguments(Type[] arguments, Type[][] requiredCustomModifiers, Type[][] optionalCustomModifiers)
		{
			if (arguments == null)
			{
				throw new ArgumentNullException("arguments");
			}
			if (requiredCustomModifiers != null || optionalCustomModifiers != null)
			{
				throw MissingFeature();
			}
			ValidateCustomModifier(arguments.Length, requiredCustomModifiers, "requiredCustomModifiers");
			ValidateCustomModifier(arguments.Length, optionalCustomModifiers, "optionalCustomModifiers");
			for (int i = 0; i < arguments.Length; i++)
			{
				AddArgument(arguments[i], (requiredCustomModifiers == null) ? null : requiredCustomModifiers[i], (optionalCustomModifiers == null) ? null : optionalCustomModifiers[i]);
			}
		}

		[MonoTODO("pinned is ignored")]
		public void AddArgument(Type argument, bool pinned)
		{
			AddArgument(argument);
		}

		public void AddArgument(Type argument, Type[] requiredCustomModifiers, Type[] optionalCustomModifiers)
		{
			if (argument == null)
			{
				throw new ArgumentNullException("argument");
			}
			if (requiredCustomModifiers != null)
			{
				ValidateParameterModifiers("requiredCustomModifiers", requiredCustomModifiers);
			}
			if (optionalCustomModifiers != null)
			{
				ValidateParameterModifiers("optionalCustomModifiers", optionalCustomModifiers);
			}
			int pos = AppendArray(ref arguments, argument);
			if (requiredCustomModifiers != null)
			{
				AppendArrayAt(ref modreqs, requiredCustomModifiers, pos);
			}
			if (optionalCustomModifiers != null)
			{
				AppendArrayAt(ref modopts, optionalCustomModifiers, pos);
			}
		}

		[MonoTODO("Not implemented")]
		public static SignatureHelper GetPropertySigHelper(Module mod, Type returnType, Type[] requiredReturnTypeCustomModifiers, Type[] optionalReturnTypeCustomModifiers, Type[] parameterTypes, Type[][] requiredParameterTypeCustomModifiers, Type[][] optionalParameterTypeCustomModifiers)
		{
			throw new NotImplementedException();
		}

		public void AddArgument(Type clsArgument)
		{
			if (clsArgument == null)
			{
				throw new ArgumentNullException("clsArgument");
			}
			AppendArray(ref arguments, clsArgument);
		}

		[MonoTODO("Not implemented")]
		public void AddSentinel()
		{
			throw new NotImplementedException();
		}

		private static bool CompareOK(Type[][] one, Type[][] two)
		{
			if (one == null)
			{
				if (two == null)
				{
					return true;
				}
				return false;
			}
			if (two == null)
			{
				return false;
			}
			if (one.Length != two.Length)
			{
				return false;
			}
			for (int i = 0; i < one.Length; i++)
			{
				Type[] array = one[i];
				Type[] array2 = two[i];
				if (array == null)
				{
					if (array2 == null)
					{
						continue;
					}
				}
				else if (array2 == null)
				{
					return false;
				}
				if (array.Length != array2.Length)
				{
					return false;
				}
				for (int j = 0; j < array.Length; j++)
				{
					Type type = array[j];
					Type type2 = array2[j];
					if (type == null)
					{
						if (type2 != null)
						{
							return false;
						}
						continue;
					}
					if (type2 == null)
					{
						return false;
					}
					if (!type.Equals(type2))
					{
						return false;
					}
				}
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			SignatureHelper signatureHelper = obj as SignatureHelper;
			if (signatureHelper == null)
			{
				return false;
			}
			if (signatureHelper.module != module || signatureHelper.returnType != returnType || signatureHelper.callConv != callConv || signatureHelper.unmanagedCallConv != unmanagedCallConv)
			{
				return false;
			}
			if (arguments != null)
			{
				if (signatureHelper.arguments == null)
				{
					return false;
				}
				if (arguments.Length != signatureHelper.arguments.Length)
				{
					return false;
				}
				for (int i = 0; i < arguments.Length; i++)
				{
					if (!signatureHelper.arguments[i].Equals(arguments[i]))
					{
						return false;
					}
				}
			}
			else if (signatureHelper.arguments != null)
			{
				return false;
			}
			return CompareOK(signatureHelper.modreqs, modreqs) && CompareOK(signatureHelper.modopts, modopts);
		}

		public override int GetHashCode()
		{
			return 0;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern byte[] get_signature_local();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern byte[] get_signature_field();

		public byte[] GetSignature()
		{
			switch (type)
			{
			case SignatureHelperType.HELPER_LOCAL:
				return get_signature_local();
			case SignatureHelperType.HELPER_FIELD:
				return get_signature_field();
			default:
				throw new NotImplementedException();
			}
		}

		public override string ToString()
		{
			return "SignatureHelper";
		}

		internal static SignatureHelper GetMethodSigHelper(Module mod, CallingConventions callingConvention, CallingConvention unmanagedCallingConvention, Type returnType, Type[] parameters)
		{
			if (mod != null && !(mod is ModuleBuilder))
			{
				throw new ArgumentException("ModuleBuilder is expected");
			}
			if (returnType == null)
			{
				returnType = typeof(void);
			}
			if (returnType.IsUserType)
			{
				throw new NotSupportedException("User defined subclasses of System.Type are not yet supported.");
			}
			if (parameters != null)
			{
				for (int i = 0; i < parameters.Length; i++)
				{
					if (parameters[i].IsUserType)
					{
						throw new NotSupportedException("User defined subclasses of System.Type are not yet supported.");
					}
				}
			}
			SignatureHelper signatureHelper = new SignatureHelper((ModuleBuilder)mod, SignatureHelperType.HELPER_METHOD);
			signatureHelper.returnType = returnType;
			signatureHelper.callConv = callingConvention;
			signatureHelper.unmanagedCallConv = unmanagedCallingConvention;
			if (parameters != null)
			{
				signatureHelper.arguments = new Type[parameters.Length];
				for (int j = 0; j < parameters.Length; j++)
				{
					signatureHelper.arguments[j] = parameters[j];
				}
			}
			return signatureHelper;
		}
	}
}
