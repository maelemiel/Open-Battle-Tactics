using System.Diagnostics;
using System.Globalization;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Serializable]
	[ClassInterface(ClassInterfaceType.None)]
	[ComVisible(true)]
	[ComDefaultInterface(typeof(_MethodBase))]
	public abstract class MethodBase : MemberInfo, _MethodBase
	{
		public abstract RuntimeMethodHandle MethodHandle { get; }

		public abstract MethodAttributes Attributes { get; }

		public virtual CallingConventions CallingConvention
		{
			get
			{
				return CallingConventions.Standard;
			}
		}

		public bool IsPublic
		{
			get
			{
				return (Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public;
			}
		}

		public bool IsPrivate
		{
			get
			{
				return (Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Private;
			}
		}

		public bool IsFamily
		{
			get
			{
				return (Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Family;
			}
		}

		public bool IsAssembly
		{
			get
			{
				return (Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Assembly;
			}
		}

		public bool IsFamilyAndAssembly
		{
			get
			{
				return (Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.FamANDAssem;
			}
		}

		public bool IsFamilyOrAssembly
		{
			get
			{
				return (Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.FamORAssem;
			}
		}

		public bool IsStatic
		{
			get
			{
				return (Attributes & MethodAttributes.Static) != 0;
			}
		}

		public bool IsFinal
		{
			get
			{
				return (Attributes & MethodAttributes.Final) != 0;
			}
		}

		public bool IsVirtual
		{
			get
			{
				return (Attributes & MethodAttributes.Virtual) != 0;
			}
		}

		public bool IsHideBySig
		{
			get
			{
				return (Attributes & MethodAttributes.HideBySig) != 0;
			}
		}

		public bool IsAbstract
		{
			get
			{
				return (Attributes & MethodAttributes.Abstract) != 0;
			}
		}

		public bool IsSpecialName
		{
			get
			{
				int attributes = (int)Attributes;
				return (attributes & 0x800) != 0;
			}
		}

		[ComVisible(true)]
		public bool IsConstructor
		{
			get
			{
				int attributes = (int)Attributes;
				return (attributes & 0x1000) != 0 && Name == ".ctor";
			}
		}

		public virtual bool ContainsGenericParameters
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsGenericMethodDefinition
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsGenericMethod
		{
			get
			{
				return false;
			}
		}

		void _MethodBase.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _MethodBase.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _MethodBase.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _MethodBase.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern MethodBase GetCurrentMethod();

		internal static MethodBase GetMethodFromHandleNoGenericCheck(RuntimeMethodHandle handle)
		{
			return GetMethodFromIntPtr(handle.Value, IntPtr.Zero);
		}

		private static MethodBase GetMethodFromIntPtr(IntPtr handle, IntPtr declaringType)
		{
			if (handle == IntPtr.Zero)
			{
				throw new ArgumentException("The handle is invalid.");
			}
			MethodBase methodFromHandleInternalType = GetMethodFromHandleInternalType(handle, declaringType);
			if (methodFromHandleInternalType == null)
			{
				throw new ArgumentException("The handle is invalid.");
			}
			return methodFromHandleInternalType;
		}

		public static MethodBase GetMethodFromHandle(RuntimeMethodHandle handle)
		{
			MethodBase methodFromIntPtr = GetMethodFromIntPtr(handle.Value, IntPtr.Zero);
			Type declaringType = methodFromIntPtr.DeclaringType;
			if (declaringType.IsGenericType || declaringType.IsGenericTypeDefinition)
			{
				throw new ArgumentException("Cannot resolve method because it's declared in a generic class.");
			}
			return methodFromIntPtr;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern MethodBase GetMethodFromHandleInternalType(IntPtr method_handle, IntPtr type_handle);

		[ComVisible(false)]
		public static MethodBase GetMethodFromHandle(RuntimeMethodHandle handle, RuntimeTypeHandle declaringType)
		{
			return GetMethodFromIntPtr(handle.Value, declaringType.Value);
		}

		public abstract MethodImplAttributes GetMethodImplementationFlags();

		public abstract ParameterInfo[] GetParameters();

		internal virtual int GetParameterCount()
		{
			ParameterInfo[] parameters = GetParameters();
			if (parameters == null)
			{
				return 0;
			}
			return parameters.Length;
		}

		[DebuggerHidden]
		[DebuggerStepThrough]
		public object Invoke(object obj, object[] parameters)
		{
			return Invoke(obj, BindingFlags.Default, null, parameters, null);
		}

		public abstract object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture);

		internal virtual int get_next_table_index(object obj, int table, bool inc)
		{
			if (this is MethodBuilder)
			{
				MethodBuilder methodBuilder = (MethodBuilder)this;
				return methodBuilder.get_next_table_index(obj, table, inc);
			}
			if (this is ConstructorBuilder)
			{
				ConstructorBuilder constructorBuilder = (ConstructorBuilder)this;
				return constructorBuilder.get_next_table_index(obj, table, inc);
			}
			throw new Exception("Method is not a builder method");
		}

		[ComVisible(true)]
		public virtual Type[] GetGenericArguments()
		{
			throw new NotSupportedException();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern MethodBody GetMethodBodyInternal(IntPtr handle);

		internal static MethodBody GetMethodBody(IntPtr handle)
		{
			return GetMethodBodyInternal(handle);
		}

		public virtual MethodBody GetMethodBody()
		{
			throw new NotSupportedException();
		}

		virtual Type _MethodBase.GetType()
		{
			return GetType();
		}
	}
}
