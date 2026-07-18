using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace System.Reflection
{
	internal struct MonoMethodInfo
	{
		private Type parent;

		private Type ret;

		internal MethodAttributes attrs;

		internal MethodImplAttributes iattrs;

		private CallingConventions callconv;

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_method_info(IntPtr handle, out MonoMethodInfo info);

		internal static MonoMethodInfo GetMethodInfo(IntPtr handle)
		{
			MonoMethodInfo info;
			get_method_info(handle, out info);
			return info;
		}

		internal static Type GetDeclaringType(IntPtr handle)
		{
			return GetMethodInfo(handle).parent;
		}

		internal static Type GetReturnType(IntPtr handle)
		{
			return GetMethodInfo(handle).ret;
		}

		internal static MethodAttributes GetAttributes(IntPtr handle)
		{
			return GetMethodInfo(handle).attrs;
		}

		internal static CallingConventions GetCallingConvention(IntPtr handle)
		{
			return GetMethodInfo(handle).callconv;
		}

		internal static MethodImplAttributes GetMethodImplementationFlags(IntPtr handle)
		{
			return GetMethodInfo(handle).iattrs;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ParameterInfo[] get_parameter_info(IntPtr handle, MemberInfo member);

		internal static ParameterInfo[] GetParametersInfo(IntPtr handle, MemberInfo member)
		{
			return get_parameter_info(handle, member);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern UnmanagedMarshal get_retval_marshal(IntPtr handle);

		internal static ParameterInfo GetReturnParameterInfo(MonoMethod method)
		{
			return new ParameterInfo(GetReturnType(method.mhandle), method, get_retval_marshal(method.mhandle));
		}
	}
}
