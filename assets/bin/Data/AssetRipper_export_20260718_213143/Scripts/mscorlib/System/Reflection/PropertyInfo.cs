using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Serializable]
	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.None)]
	[ComDefaultInterface(typeof(_PropertyInfo))]
	public abstract class PropertyInfo : MemberInfo, _PropertyInfo
	{
		public abstract PropertyAttributes Attributes { get; }

		public abstract bool CanRead { get; }

		public abstract bool CanWrite { get; }

		public bool IsSpecialName
		{
			get
			{
				return (Attributes & PropertyAttributes.SpecialName) != 0;
			}
		}

		public override MemberTypes MemberType
		{
			get
			{
				return MemberTypes.Property;
			}
		}

		public abstract Type PropertyType { get; }

		void _PropertyInfo.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _PropertyInfo.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _PropertyInfo.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _PropertyInfo.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		public MethodInfo[] GetAccessors()
		{
			return GetAccessors(false);
		}

		public abstract MethodInfo[] GetAccessors(bool nonPublic);

		public MethodInfo GetGetMethod()
		{
			return GetGetMethod(false);
		}

		public abstract MethodInfo GetGetMethod(bool nonPublic);

		public abstract ParameterInfo[] GetIndexParameters();

		public MethodInfo GetSetMethod()
		{
			return GetSetMethod(false);
		}

		public abstract MethodInfo GetSetMethod(bool nonPublic);

		[DebuggerStepThrough]
		[DebuggerHidden]
		public virtual object GetValue(object obj, object[] index)
		{
			return GetValue(obj, BindingFlags.Default, null, index, null);
		}

		public abstract object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture);

		[DebuggerHidden]
		[DebuggerStepThrough]
		public virtual void SetValue(object obj, object value, object[] index)
		{
			SetValue(obj, value, BindingFlags.Default, null, index, null);
		}

		public abstract void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture);

		public virtual Type[] GetOptionalCustomModifiers()
		{
			return Type.EmptyTypes;
		}

		public virtual Type[] GetRequiredCustomModifiers()
		{
			return Type.EmptyTypes;
		}

		[MonoTODO("Not implemented")]
		public virtual object GetConstantValue()
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Not implemented")]
		public virtual object GetRawConstantValue()
		{
			throw new NotImplementedException();
		}

		virtual Type _PropertyInfo.GetType()
		{
			return GetType();
		}
	}
}
