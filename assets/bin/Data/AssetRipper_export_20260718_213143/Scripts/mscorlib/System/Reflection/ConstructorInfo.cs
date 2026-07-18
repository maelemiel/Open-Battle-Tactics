using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Serializable]
	[ComVisible(true)]
	[ComDefaultInterface(typeof(_ConstructorInfo))]
	[ClassInterface(ClassInterfaceType.None)]
	public abstract class ConstructorInfo : MethodBase, _ConstructorInfo
	{
		[ComVisible(true)]
		public static readonly string ConstructorName = ".ctor";

		[ComVisible(true)]
		public static readonly string TypeConstructorName = ".cctor";

		[ComVisible(true)]
		public override MemberTypes MemberType
		{
			get
			{
				return MemberTypes.Constructor;
			}
		}

		void _ConstructorInfo.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _ConstructorInfo.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _ConstructorInfo.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _ConstructorInfo.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		object _ConstructorInfo.Invoke_2(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			return Invoke(obj, invokeAttr, binder, parameters, culture);
		}

		object _ConstructorInfo.Invoke_3(object obj, object[] parameters)
		{
			return Invoke(obj, parameters);
		}

		object _ConstructorInfo.Invoke_4(BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			return Invoke(invokeAttr, binder, parameters, culture);
		}

		object _ConstructorInfo.Invoke_5(object[] parameters)
		{
			return Invoke(parameters);
		}

		[DebuggerHidden]
		[DebuggerStepThrough]
		public object Invoke(object[] parameters)
		{
			if (parameters == null)
			{
				parameters = new object[0];
			}
			return Invoke(BindingFlags.CreateInstance, null, parameters, null);
		}

		public abstract object Invoke(BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture);

		virtual Type _ConstructorInfo.GetType()
		{
			return GetType();
		}
	}
}
